using System;
using System.Collections.Generic;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;
using iCON.Utility;

namespace iCON.System
{
    /// <summary>
    /// ストーリーのオーダーを実行するクラス
    /// </summary>
    public class OrderExecutor : IDisposable
    {
        /// <summary>
        /// Viewを操作するクラス
        /// </summary>
        private StoryView _view;
        
        /// <summary>
        /// オーダーを実行中か
        /// </summary>
        private bool _isExecuting;
        
        /// <summary>
        /// 実行中のオーダーのSequence
        /// </summary>
        private Sequence _currentSequence;

        /// <summary>
        /// ストーリー終了時に実行するアクション
        /// </summary>
        private Action _endAction;
        
        /// <summary>
        /// 各オーダーの列挙型と処理を行うHandlerのインスタンスのkvp
        /// </summary>
        private Dictionary<OrderType, OrderHandlerBase> _handlers;
        
        /// <summary>
        /// オーダーを実行中か
        /// </summary>
        public bool IsExecuting => _isExecuting;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderExecutor(StoryView view)
        {
            _view = view;
            
            // 各オーダーの列挙型と処理を行うHandlerのインスタンスの辞書を作成
            _handlers = OrderHandlerFactory.CreateAllHandlers(_view, null);
        }

        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(Action endAction)
        {
            _endAction = endAction;
            
            // EndHandlerに終了アクションを設定
            if (_handlers.TryGetValue(OrderType.End, out var endHandler) && endHandler is EndOrderHandler endOrderHandler)
            {
                endOrderHandler.SetEndAction(_endAction);
            }
        }

        /// <summary>
        /// オーダーを実行する
        /// </summary>
        public void Execute(OrderData data)
        {
            if (data.Sequence == SequenceType.Append)
            {
                // 念のため実行中のシーケンスがあればキルする
                _currentSequence?.Kill(true);
                _currentSequence = DOTween.Sequence();
            }
            
            _isExecuting = true;
            
            try
            {
                if (_handlers.TryGetValue(data.OrderType, out var handler))
                {
                    var tween = handler.HandleOrder(data, _view);
                    if (tween != null)
                    {
                        _currentSequence.AddTween(data.Sequence, tween);
                    }
                }
                else
                {
                    LogUtility.Warning($"未登録のオーダータイプです: {data.OrderType}", LogCategory.System);
                }
            }
            catch (Exception ex)
            {
                LogUtility.Error($"{data.OrderType} オーダー実行中にエラーが発生: {ex.Message}", LogCategory.System);
            }
            finally
            {
                if (data.Sequence == SequenceType.Append)
                {
                    _currentSequence.OnComplete(() => _isExecuting = false);
                }
            }
        }

        /// <summary>
        /// オーダーの演出をスキップする
        /// </summary>
        public void Skip()
        {
            if (_currentSequence != null && _isExecuting)
            {
                // 演出実行中であれば、シーケンスをキルしてコンプリートの状態にする
                _currentSequence.Kill(true);
                _isExecuting = false;
            }
        }

        public void Dispose()
        {
            if (_endAction != null)
            {
                // アクションが登録されていたら破棄する
                _endAction = null;
            }
            
            // EndHandlerに登録した終了アクションの購読を解除
            if (_handlers.TryGetValue(OrderType.End, out var endHandler) && endHandler is EndOrderHandler endOrderHandler)
            {
                endOrderHandler.Dispose();
            }
        }
    }
}
