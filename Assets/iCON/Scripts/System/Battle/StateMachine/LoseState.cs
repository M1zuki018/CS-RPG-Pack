using iCON.Enums;
using iCON.UI;

namespace iCON.Battle
{
    /// <summary>
    /// バトル敗北
    /// </summary>
    public class LoseState : BattleStateBase
    {
        public override async void Enter(BattleManager manager, BattleCanvasManager view)
        {
            base.Enter(manager, view);
            view.ShowCanvas(BattleCanvasType.Lose);
        }
    }
}