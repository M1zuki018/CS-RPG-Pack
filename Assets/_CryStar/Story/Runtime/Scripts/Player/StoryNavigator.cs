using System;
using System.Collections.Generic;
using CryStar.Story.Data;
using CryStar.Utility;
using CryStar.Utility.Enum;

namespace CryStar.Story.Player
{
    /// <summary>
    /// ストーリーの進行管理と進行状況を元にオーダー取得を行うクラス
    /// </summary>
    public class StoryNavigator : IDisposable
    {
        /// <summary>
        /// ストーリーが最後に到達した際に発生するイベント
        /// </summary>
        private event Action _onStoryCompleted;
        
        /// <summary>
        /// ストーリーオーダーの取得を専門とするクラス
        /// </summary>
        private StoryOrderProvider _orderProvider = new StoryOrderProvider();
        
        /// <summary>
        /// 現在のオーダーの位置
        /// </summary>
        private int _currentOrderIndex = 0;

        /// <summary>
        /// 現在のオーダーの位置
        /// </summary>
        public int CurrentOrderIndex => _currentOrderIndex;
        
        /// <summary>
        /// 次のIndexが存在するか
        /// </summary>
        public bool HasNextOrder => _currentOrderIndex < _orderProvider.GetOrderCount();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StoryNavigator(Action onStoryCompleted)
        {
            _onStoryCompleted = onStoryCompleted;
        }
        
        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(IReadOnlyList<OrderData> orders)
        {
            if (orders == null)
            {
                // オーダーのリストがnullであれば例外をスロー
                throw new ArgumentNullException(nameof(orders));
            }
            
            _orderProvider.Setup(orders);
            
            // インデックスをリセット
            _currentOrderIndex = 0;
        }

        /// <summary>
        /// OrderProviderから次のオーダー群を取得する
        /// </summary>
        public IReadOnlyList<OrderData> GetNextOrderSequence()
        {
            if (!HasNextOrder)
            {
                LogUtility.Warning($"[{typeof(StoryNavigator)}] 次のオーダーがありません", LogCategory.System);
                return Array.Empty<OrderData>();
            }

            var orders = _orderProvider.GetContinuousOrdersFrom(_currentOrderIndex);
            
            // 取得したオーダーの数だけインデックスを進める
            _currentOrderIndex += orders.Count;
            
            return orders.AsReadOnly();
        }

        /// <summary>
        /// 引数に渡したIndexの1つ前のオーダーへ移動する
        /// NOTE: 次に実行したときにIndexが1つ進むので、指定のオーダーを再生できる
        /// </summary>
        public void JumpToOrder(int orderIndex)
        {
            _currentOrderIndex = Math.Max(0, Math.Min(orderIndex - 1, _orderProvider.GetOrderCount() - 1));
        }

        /// <summary>
        /// 最後から1つ前のオーダーへ移動する
        /// </summary>
        public void JumpToEnd()
        {
            _currentOrderIndex = Math.Max(0, _orderProvider.GetOrderCount() - 1);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _onStoryCompleted = null;
        }
    }
}

