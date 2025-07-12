using iCON.Enums;
using iCON.UI;

namespace iCON.Battle
{
    public class ActionSelectState : BattleStateBase
    {
        public override void Enter(BattleManager manager, BattleCanvasManager view)
        {
            base.Enter(manager, view);
            view.ShowCanvas(BattleCanvasType.ActionSelect);
        }
    }
}