using Cysharp.Threading.Tasks;
using iCON.Enums;
using iCON.System;
using iCON.UI;

namespace iCON.Battle
{
    /// <summary>
    /// バトル勝利
    /// </summary>
    public class WinState : BattleStateBase
    {
        public override async void Enter(BattleManager manager, BattleCanvasManager view)
        {
            base.Enter(manager, view);
            view.ShowCanvas(BattleCanvasType.Win);
            
            await UniTask.Delay(1000);
            
            await ServiceLocator.GetGlobal<SceneLoader>().LoadSceneAsync(new SceneTransitionData(SceneType.InGame));
        }
    }
}
