using iCON.Enums;
using iCON.UI;

namespace iCON.Battle
{
    /// <summary>
    /// 逃げられるかチェックする
    /// </summary>
    public class TryEscapeState : BattleStateBase
    {
        public override void Enter(BattleManager manager, BattleCanvasManager view)
        {
            base.Enter(manager, view);
            view.ShowCanvas(BattleCanvasType.TryEscape);
        }
    }
}