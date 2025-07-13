using iCON.Enums;
using iCON.UI;
using iCON.Utility;
using NotImplementedException = System.NotImplementedException;

namespace iCON.Battle
{
    /// <summary>
    /// たたかう・にげるを選択する
    /// </summary>
    public class FirstSelectState : BattleStateBase
    {
        private CanvasController_FirstSelect _canvasController;

        public override void Enter(BattleManager manager, BattleCanvasManager view)
        {
            base.Enter(manager, view);
            view.ShowCanvas(BattleCanvasType.FirstSelect);

            _canvasController = view.CurrentCanvas as CanvasController_FirstSelect;
            if (_canvasController == null)
            {
                LogUtility.Fatal("CanvasController_FirstSelect が取得できませんでした", LogCategory.Gameplay);
                return;
            }

            _canvasController.OnStartBattle += StartBattle;
            _canvasController.OnTryEscape += TryEscape;
        }

        public override void Exit()
        {
            base.Exit();
            _canvasController.OnStartBattle -= StartBattle;
            _canvasController.OnTryEscape -= TryEscape;
        }

        /// <summary>
        /// バトルを開始してコマンド選択に移る
        /// </summary>
        private void StartBattle()
        {
            BattleManager.SetState(BattleSystemState.CommandSelect);
        }
        
        /// <summary>
        /// 逃走チェック
        /// </summary>
        private void TryEscape()
        {
            BattleManager.SetState(BattleSystemState.TryEscape);
        }
    }

}