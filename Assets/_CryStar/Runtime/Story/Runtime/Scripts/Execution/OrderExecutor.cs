using System;
using System.Collections.Generic;
using System.Threading;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.Factory;
using CryStar.Story.UI;
using CryStar.Utility;
using CryStar.Utility.Enum;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CryStar.Story.Execution
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
        /// Sequenceに追加予定のTweenリスト
        /// </summary>
        private List<(SequenceType sequenceType, Tween tween)> _pendingTweens = new List<(SequenceType sequenceType, Tween tween)>();
        
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

        private Action<int> _jampAction;
        
        /// <summary>
        /// 各オーダーの列挙型と処理を行うHandlerのインスタンスのkvp
        /// </summary>
        private Dictionary<OrderType, OrderHandlerBase> _handlers;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// オーダーを実行中か
        /// </summary>
        public bool IsExecuting => _isExecuting;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderExecutor(StoryView view, Action<int> jampAction)
        {
            _view = view;
            _jampAction = jampAction;
            
            // 各オーダーの列挙型と処理を行うHandlerのインスタンスの辞書を作成
            _handlers = OrderHandlerFactory.CreateAllHandlers(_view, null);
        }

        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(Action endAction)
        {
            _endAction = endAction;

            if (_handlers.TryGetValue(OrderType.Choice, out var choiceHandler) && choiceHandler is ChoiceOrderHandler choiceOrderHandler)
            {
                choiceOrderHandler.SetChoiceAction(_jampAction);
            }
            
            // EndHandlerに終了アクションを設定
            if (_handlers.TryGetValue(OrderType.End, out var endHandler) && endHandler is EndOrderHandler endOrderHandler)
            {
                endOrderHandler.SetEndAction(_endAction);
            }

            // エフェクト処理のファクトリーのSetup
            if (_handlers.TryGetValue(OrderType.Effect, out var effectHandler) && effectHandler is EffectOrderHandler effectOrderHandler)
            {
                effectOrderHandler.SetupPerformerCache(_view);
            }
        }

        /// <summary>
        /// オーダーを実行する
        /// </summary>
        public async UniTask Execute(IReadOnlyList<OrderData> orders)
        {
            try
            {
                if (orders == null || orders.Count == 0)
                {
                    _isExecuting = false;
                    return;
                }
                
                foreach (var data in orders)
                {
                    if (data.Sequence == SequenceType.Append)
                    {
                        // 念のため実行中のシーケンスがあればキルする
                        _currentSequence?.Kill(true);
                        _pendingTweens?.Clear();
                
                        // Sequenceの作成は全ての非同期処理完了後に行う
                        _isExecuting = true;
                    }
                    
                    if (_handlers.TryGetValue(data.OrderType, out var handler))
                    {
                        Tween tween = null;
                        
                        // ハンドラーが非同期対応かチェック
                        if (handler is IAsyncOrderHandler asyncHandler)
                        {
                            // 非同期処理を実行
                            tween = await asyncHandler.HandleOrderAsync(data, _view, _cancellationTokenSource.Token);
                        }
                        else
                        {
                            // 従来の同期処理
                            tween = handler.HandleOrder(data, _view);
                        }

                        if (tween != null)
                        {
                            _pendingTweens.Add((data.Sequence, tween));
                        }
                    }
                    else
                    {
                        LogUtility.Warning($"未登録のオーダータイプです: {data.OrderType}", LogCategory.System);
                    }
                }

                BuildAndPlaySequence().Forget();
            }
            catch (Exception ex)
            {
                LogUtility.Error($"オーダー実行中にエラーが発生: {ex.Message}", LogCategory.System);
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

        /// <summary>
        /// Dispose
        /// </summary>
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
        
        /// <summary>
        /// Sequenceを構築して再生する
        /// </summary>
        private async UniTask BuildAndPlaySequence()
        {
            if (_pendingTweens.Count == 0)
            {
                _isExecuting = false;
                return;
            }

            _currentSequence = DOTween.Sequence();
            
            // 保存されたTweenをSequenceに追加
            foreach (var (sequenceType, tween) in _pendingTweens)
            {
                if (tween != null && tween.IsActive() && !tween.IsComplete())
                {
                    _currentSequence.AddTween(sequenceType, tween);   
                }
                else if (tween != null)
                {
                    LogUtility.Warning($"無効なTweenをSequenceに追加しようとしました: {tween.GetType()}", LogCategory.System);
                }
            }
            
            // 有効なTweenがない場合の処理
            if (_currentSequence.Duration() <= 0)
            {
                _currentSequence.Kill();
                _isExecuting = false;
                return;
            }
            
            _currentSequence.OnComplete(() => _isExecuting = false);
            
            // Sequenceを開始
            _currentSequence.Play();
            
            _pendingTweens.Clear();
        }
    }
}
