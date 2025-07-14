using iCON.Enums;
using iCON.UI;

namespace iCON.Battle
{
    /// <summary>
    /// バトル実行
    /// </summary>
    public class ExecuteState : BattleStateBase
    {
        public override async void Enter(BattleManager manager, BattleCanvasManager view)
        {
            base.Enter(manager, view);
            view.ShowCanvas(BattleCanvasType.Execute);

            // バトル実行を待つ
            var result = await manager.ExecuteBattle();

            if (result.isFinish)
            {
                // バトルが終了している場合は勝敗に合わせてステートを変更
                manager.SetState(result.isWin ? BattleSystemState.Win : BattleSystemState.Lose);
            }
            else
            {
                // バトルが終了していなければ最初の選択に戻る
                manager.SetState(BattleSystemState.FirstSelect);
            }
        }
    }

}