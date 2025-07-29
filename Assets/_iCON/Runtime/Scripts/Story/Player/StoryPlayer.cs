using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using iCON.System;
using iCON.UI;
using iCON.Utility;
using UnityEngine;

namespace CryStar.Story.Player
{
    /// <summary>
    /// ストーリー全体の進行を管理するマネージャー
    /// </summary>
    public class StoryPlayer : ViewBase
    {
        /// <summary>
        /// View
        /// </summary>
        [SerializeField] 
        private StoryView _view;
        
        /// <summary>
        /// データの読み込みとオーダー取得を行う
        /// </summary>
        private StoryOrderProvider _orderProvider;
        
        /// <summary>
        /// オーダーを実行する
        /// </summary>
        private OrderExecutor _orderExecutor;
        
        /// <summary>
        /// オート再生を担当
        /// </summary>
        private StoryAutoPlayController _autoPlayController;

        /// <summary>
        /// 現在のストーリー位置
        /// </summary>
        private int _currentOrder = 0;
        
        /// <summary>
        /// ストーリー終了時のアクション
        /// NOTE: 初期化後すぐにストーリーが進まないようにDefaultはtrueにしておく
        /// </summary>
        private bool _isStoryComplete = true;
        
        /// <summary>
        /// UI非表示モード
        /// </summary>
        private bool _isImmerseMode = false;

        /// <summary>
        /// 選択肢表示中などによる一時停止
        /// </summary>
        private bool _isStopRequested;

        #region Lifecycle
        
        /// <summary>
        /// Awake
        /// </summary>
        public override async UniTask OnAwake()
        {
            await base.OnAwake();
            InitializeComponents();
        }
        
        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            if (_isImmerseMode || _isStopRequested || _isStoryComplete)
            {
                // UI非表示モード/選択肢表示中/既に読了していた場合は処理を行わない
                return;
            }
            
            if (!_orderExecutor.IsExecuting && _autoPlayController.NotYetRequest)
            {
                // オート再生のフラグを予約済みに切り替える
                _autoPlayController.AutoPlay().Forget();
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
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
            _autoPlayController.Dispose();
        }
        
        #endregion

        /// <summary>
        /// UI非表示モードの管理
        /// </summary>
        private void HandleClickImmersiveMode()
        {
            // UI非表示状態かフラグを切り替える
            _isImmerseMode = !_isImmerseMode;
            _view.ImmersiveMode(_isImmerseMode);
        }

        /// <summary>
        /// オート再生モードの管理
        /// </summary>
        private void HandleClickAutoPlay()
        {
            bool isAutoPlay = _autoPlayController.HandleClickAutoPlay();
            _view.AutoPlayMode(isAutoPlay);
        }
        
        /// <summary>
        /// ストーリー再生を開始する
        /// </summary>
        public void PlayStory(StorySceneData sceneData, IReadOnlyList<OrderData> orders, Action endAction)
        {
            _orderProvider.Setup(orders);
            
            // ストーリーの進行位置をリセット
            _currentOrder = 0;
            
            _orderExecutor.Setup(() =>
            {
                endAction?.Invoke();
                _isStoryComplete = true;
            });
            
            // キャラクター立ち絵のSetup
            _view.SetupCharacter(sceneData.CharacterScale, sceneData.PositionCorrection);
            
            // ストーリー読了フラグをfalseにして、再生できるようにする
            _isStoryComplete = false;
        }

        #region Orderの進行処理

        /// <summary>
        /// 次のオーダーに進む
        /// </summary>
        private void ProcessNextOrder()
        {
            if (_autoPlayController.IsAutoPlayReserved)
            {
                // オート再生中に手動でオーダーを進めた場合、オート再生の予約をキャンセルする
                _autoPlayController.CancelAutoPlay();
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
            var orders = _orderProvider.GetContinuousOrdersFrom(_currentOrder);
            
            // オーダーの位置を変更
            _currentOrder += orders.Count;
            
            return orders;
        }
        
        /// <summary>
        /// オーダーリストを実行
        /// </summary>
        private void ExecuteOrders(List<OrderData> orders)
        {
            _orderExecutor.Execute(orders).Forget();
        }
        
        #endregion

        /// <summary>
        /// 各コンポーネントの初期化
        /// </summary>
        private void InitializeComponents()
        {
            _orderProvider = new StoryOrderProvider();
            _orderExecutor = new OrderExecutor(_view, ExecuteChoiceBranch);
            _autoPlayController = new StoryAutoPlayController(ProcessNextOrder);
            _view.InitializeChoice(HandleStop);
            _view.SetupOverlay(MoveToEndOrder, HandleClickImmersiveMode, HandleClickAutoPlay);
        }

        private void HandleStop()
        {
            _isStopRequested = true;
        }
        
        /// <summary>
        /// スキップ機能
        /// </summary>
        private void MoveToEndOrder()
        {
            // Endオーダーの1つ前のオーダーに移動
            _currentOrder = _orderProvider.GetOrderCount() - 1;
            
            if (_orderExecutor.IsExecuting)
            {
                // オーダーが実行中であれば演出をスキップする
                _orderExecutor.Skip();
            }
            
            // すぐにEndオーダーを実行
            ExecuteNextOrderSequence();
        }

        /// <summary>
        /// 選択肢による分岐機能
        /// </summary>
        private void ExecuteChoiceBranch(int orderIndex = -1)
        {
            // オーダーのインデックスがデフォルトであれば現在の地点を
            // その他のインデックスの場合は引数で指定されたオーダーに移動する
            _currentOrder = orderIndex == -1 ? _currentOrder : orderIndex;

            // 一時停止解除
            _isStopRequested = false;
            
            // オーダーを実行
            ProcessNextOrder();
        }
    }
}
