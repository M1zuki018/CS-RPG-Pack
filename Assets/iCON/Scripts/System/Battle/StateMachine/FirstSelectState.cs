using iCON.Enums;
using iCON.UI;

namespace iCON.Battle
{
    /// <summary>
    /// たたかう・にげるを選択する
    /// </summary>
    public class FirstSelectState : BattleStateBase
    {
        public override void Enter(BattleManager manager, BattleCanvasManager view)
        {
            base.Enter(manager, view);
            view.ShowCanvas(BattleCanvasType.FirstSelect);
        }
    }

}