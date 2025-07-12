using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using iCON.Enums;
using iCON.UI;

namespace iCON.Battle
{
    /// <summary>
    /// 逃げられるかチェックする
    /// </summary>
    public class TryEscapeState : BattleStateBase
    {
        /// <summary>
        /// 乱数生成器（staticで再利用できるようにする）
        /// </summary>
        private static readonly Random _random = new Random();
        
        /// <summary>
        /// CancellationTokenSource
        /// </summary>
        private CancellationTokenSource _cts;
        
        /// <summary>
        /// 逃走成功確率DEFAULT_ESCAPE_RATE
        /// TODO: プレイヤーの逃走成功率の情報を読み取ったりして、適切な値を取得できるようにする
        /// </summary>
        private const int DEFAULT_ESCAPE_RATE = 3;

        /// <summary>
        /// 逃走失敗キャンバスを表示しておく時間（秒）
        /// </summary>
        private const float FAILURE_DISPLAY_INTERVAL = 3f;
        
        public override async void Enter(BattleManager manager, BattleCanvasManager view)
        {
            base.Enter(manager, view);
            await ExecuteEscapeAttemptAsync(manager, view);
        }
        
        public override void Exit()
        {
            base.Exit();
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        /// <summary>
        /// 逃走できるか試す
        /// </summary>
        private async UniTask ExecuteEscapeAttemptAsync(BattleManager manager, BattleCanvasManager view)
        {
            // 逃走判定を行う
            bool isEscapeSuccessful = RollEscapeAttempt();
            
            // UI表示を行う
            // TODO: 表示タイミングは後々検討
            view.ShowCanvas(BattleCanvasType.TryEscape);

            if (isEscapeSuccessful)
            {
                // TODO: 逃走が成功した場合の処理
            }
            else
            {
                // 逃走失敗時の処理を行う
                await HandleEscapeFailureAsync(manager);
            }
        }

        /// <summary>
        /// 逃走失敗時の処理
        /// </summary>
        private async UniTask HandleEscapeFailureAsync(BattleManager manager)
        {
            // 念のためキャンセルトークンソースをクリーンアップしておく
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            
            _cts = new CancellationTokenSource();

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(FAILURE_DISPLAY_INTERVAL), cancellationToken: _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // 何かキーを押したら待機時間をスキップされるようにするので例外は特に出さない
            }
            finally
            {
                // 選択に戻る
                manager.SetState(BattleSystemState.FirstSelect);
            }
        }

        /// <summary>
        /// 逃走判定を行う
        /// </summary>
        private bool RollEscapeAttempt()
        {
            // 1-101の範囲で乱数を生成し、逃走成功率と比較する
            int roll = _random.Next(1, 101);
            return roll < DEFAULT_ESCAPE_RATE;
        }
    }
}