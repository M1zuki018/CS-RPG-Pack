using iCON.UI;

namespace iCON.Battle
{
    public abstract class BattleStateBase
    {
        protected BattleManager BattleManager;
        protected BattleCanvasManager View;
        
        public virtual void Enter(BattleManager manager, BattleCanvasManager view)
        {
            BattleManager = manager;
            View = view;
        }
        
        public virtual void Exit() { }
    }
}