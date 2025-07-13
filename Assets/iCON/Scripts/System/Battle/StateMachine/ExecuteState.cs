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

            await manager.ExecuteBattle();
            
            manager.SetState(BattleSystemState.FirstSelect);
        }
    }

}