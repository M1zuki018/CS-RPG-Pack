using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        /// </summary>
        private bool _isStoryComplete;

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
            await InitializeComponents();
        }
        
        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
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
        }
        
        #endregion
        
        /// <summary>
        /// 各コンポーネントの初期化
        /// </summary>
        private async UniTask InitializeComponents()
        {
            _progressTracker = new StoryProgressTracker();
            _orderProvider = new StoryOrderProvider();
            _orderExecutor = new OrderExecutor(_view, () => _isStoryComplete = true);

            await _orderProvider.InitializeAsync();
        }

        #region Orderの進行処理

        /// <summary>
        /// 次のオーダーに進む
        /// </summary>
        private void ProcessNextOrder()
        {
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

        #endregion

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
