using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using iCON.Constants;
using iCON.UI;
using iCON.Utility;
using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// ストーリー全体の進行を管理するマネージャー
    /// </summary>
    public class StoryManager : ViewBase
    {
        /// <summary>
        /// View
        /// </summary>
        [SerializeField] 
        private StoryView _view;
        
        /// <summary>
        /// オーバーレイ
        /// </summary>
        [SerializeField]
        private StoryOverlayController _overlayController;
        
        /// <summary>
        /// ストーリーの現在位置の保持と移動を行う
        /// </summary>
        private StoryProgressTracker _progressTracker;
        
        /// <summary>
        /// データの読み込みとオーダー取得を行う
        /// </summary>
        private StoryOrderProvider _orderProvider;
        
        /// <summary>
        /// オーダーを実行する
        /// </summary>
        private OrderExecutor _orderExecutor;

        /// <summary>
        /// ストーリー終了時のアクション
        /// NOTE: 初期化後すぐにストーリーが進まないようにDefaultはtrueにしておく
        /// </summary>
        private bool _isStoryComplete = true;
        
        /// <summary>
        /// オート再生開始予約済み
        /// </summary>
        private bool _isAutoPlayReserved = false;
        
        /// <summary>
        /// CancellationTokenSource
        /// </summary>
        private CancellationTokenSource _cts = new CancellationTokenSource();

        /// <summary>
        /// 現在のストーリー位置
        /// </summary>
        private StoryPosition CurrentPosition => _progressTracker.CurrentPosition;
        
        /// <summary>
        /// 現在のストーリー位置のオーダーデータ
        /// </summary>
        private OrderData CurrentOrder => _orderProvider.GetOrderAt(CurrentPosition);

        #region Lifecycle
        
        /// <summary>
        /// Awake
        /// </summary>
        public override async UniTask OnAwake()
        {
            await base.OnAwake();
            InitializeComponents();
            _overlayController.Setup(_view, CancelAutoPlay, () => MoveToNextScene()); // TODO: 第三引数のスキップボタンのMethodについては仮
        }
        
        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            if (_overlayController.IsImmerseMode)
            {
                // UI非表示モードの場合ストーリーを進めない
                return;
            }
            
            if (_overlayController.AutoPlayMode && !_orderExecutor.IsExecuting && !_isAutoPlayReserved)
            {
                // フラグを予約済みに切り替える
                _isAutoPlayReserved = true;
                AutoPlay().Forget();
            }
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                ProcessNextOrder();
            }
        }
        
        /// <summary>
        /// Destroy
        /// </summary>
        private void OnDestroy()
        {
            _orderExecutor.Dispose();
            CancelAutoPlay();
        }
        
        #endregion
        
        /// <summary>
        /// 各コンポーネントの初期化
        /// </summary>
        private void InitializeComponents()
        {
            _progressTracker = new StoryProgressTracker();
            _orderProvider = new StoryOrderProvider();
            _orderExecutor = new OrderExecutor(_view);
        }

        /// <summary>
        /// ストーリー再生を開始する
        /// </summary>
        public async UniTask PlayStory(string spreadsheetName, string headerRange, string range, Action endAction)
        {
            // ストーリーの進行位置をリセット
            _progressTracker.Reset();
            
            _orderExecutor.Setup(() =>
            {
                endAction?.Invoke();
                _isStoryComplete = true;
            });

            // ヘッダーデータを読み込む
            await _orderProvider.InitializeAsync(spreadsheetName, headerRange);
            // ストーリーデータを読み込む
            await _orderProvider.LoadSceneDataAsync(spreadsheetName, range);
            
            // ストーリー読了フラグをfalseにして、再生できるようにする
            _isStoryComplete = false;
        }

        #region Orderの進行処理

        /// <summary>
        /// 次のオーダーに進む
        /// </summary>
        private void ProcessNextOrder()
        {
            if (_isAutoPlayReserved)
            {
                // オート再生中に手動でオーダーを進めた場合、オート再生の予約をキャンセルする
                CancelAutoPlay();
            }
            
            if (_orderExecutor.IsExecuting)
            {
                // オーダーが実行中であれば演出をスキップする
                _orderExecutor.Skip();
            }
            else
            {
                // 次のオーダー群を実行
                ExecuteNextOrderSequence();
            }
        }

        /// <summary>
        /// 次のオーダーシーケンスを実行
        /// </summary>
        private void ExecuteNextOrderSequence()
        {
            if (_isStoryComplete)
            {
                // ストーリーを読了していたらreturn
                return;
            }
            
            // オーダーを取得し、進行位置も更新する
            var orders = GetContinuousOrdersAndAdvance();
            
            if (orders.Count > 0)
            {
                // オーダーリストを実行
                ExecuteOrders(orders);
            }
            else
            {
                // オーダーが取得できない場合はログを出す
                LogUtility.Error("次のオーダーが見つかりません", LogCategory.System);
            }
        }
        
        /// <summary>
        /// 連続オーダーを取得し進行位置を更新する
        /// </summary>
        private List<OrderData> GetContinuousOrdersAndAdvance()
        {
            // 指定位置からAppendが出現するまでの連続オーダーを取得
            var orders = _orderProvider.GetContinuousOrdersFrom(CurrentPosition);
            
            if (orders.Count > 1)
            {
                // 取得した最後のオーダーの位置まで現在の進行位置を進める
                _progressTracker.CurrentPosition.OrderIndex += orders.Count;
            }
            else if (orders.Count == 1)
            {
                // 単一オーダーの場合は次に進める
                _progressTracker.MoveToNextOrder();
            }
            
            return orders;
        }
        
        /// <summary>
        /// オーダーリストを実行
        /// </summary>
        private void ExecuteOrders(List<OrderData> orders)
        {
            foreach (var order in orders)
            {
                _orderExecutor.Execute(order);
            }
        }

        /// <summary>
        /// オート再生
        /// </summary>
        private async UniTask AutoPlay()
        {
            // 新しいCancellationTokenSourceを作成
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            
            try
            {
                // 定数で設定しているインターバル分待機してから次のオーダーを実行する
                await UniTask.Delay(TimeSpan.FromSeconds(StoryConstants.AUTO_PLAY_INTERVAL), cancellationToken: token);
                
                // キャンセルされていない場合のみ次のオーダーを実行
                if (!token.IsCancellationRequested)
                {
                    ProcessNextOrder();
                }
            }
            catch (OperationCanceledException)
            {
                // オート再生がキャンセルされた
            }
            finally
            {
                // 予約が実行されたのでフラグを戻す
                _isAutoPlayReserved = false;
            }
        }

        #endregion

        /// <summary>
        /// オート再生を止める処理
        /// </summary>
        private void CancelAutoPlay()
        {
            // オート再生用のUniTaskをキャンセルする
            _cts?.Cancel();
            _cts?.Dispose();
        }
        
        /// <summary>
        /// 次のシーンに進む
        /// </summary>
        public OrderData MoveToNextScene()
        {
            _progressTracker.MoveToNextScene();
            return CurrentOrder;
        }
        
        /// <summary>
        /// 次のチャプターに進む
        /// </summary>
        public OrderData MoveToNextChapter()
        {
            _progressTracker.MoveToNextChapter();
            return CurrentOrder;
        }
        
        /// <summary>
        /// 次のパートに進む
        /// </summary>
        public OrderData MoveToNextPart()
        {
            _progressTracker.MoveToNextPart();
            return CurrentOrder;
        }
        
        /// <summary>
        /// 指定位置にジャンプ
        /// </summary>
        public OrderData JumpToPosition(int partId, int chapterId, int sceneId, int orderIndex = 0)
        {
            _progressTracker.JumpToPosition(partId, chapterId, sceneId, orderIndex);
            return CurrentOrder;
        }
        
        /// <summary>
        /// 指定位置にジャンプ
        /// </summary>
        public OrderData JumpToPosition(StoryPosition position)
        {
            _progressTracker.JumpToPosition(position);
            return CurrentOrder;
        }
        
        /// <summary>
        /// ストーリーをリセット
        /// </summary>
        public void ResetStory()
        {
            _progressTracker.Reset();
        }
    }
}
