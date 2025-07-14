using iCON.Enums;
using iCON.UI;
using iCON.Utility;

namespace iCON.Battle
{
    /// <summary>
    /// Idea選択
    /// </summary>
    public class IdeaState : BattleStateBase
    {
        private CanvasController_Idea _canvasController;
        
        public override void Enter(BattleManager manager, BattleCanvasManager view)
        {
            base.Enter(manager, view);
            view.ShowCanvas(BattleCanvasType.Idea);
            
            _canvasController = view.CurrentCanvas as CanvasController_Idea;
            if (_canvasController == null)
            {
                LogUtility.Fatal("CanvasController_FirstSelect が取得できませんでした", LogCategory.Gameplay);
                return;
            }
            
            _canvasController.OnIdeaSelected += HandleIdeaSelected;
        }

        public override void Exit()
        {
            base.Exit();
            _canvasController.OnIdeaSelected -= HandleIdeaSelected;
        }

        /// <summary>
        /// Ideaが選択されたときの処理
        /// </summary>
        private void HandleIdeaSelected(int selectedIdeaId)
        {
            // 選択されたアイデアをコマンドリストに追加
            BattleManager.AddCommandList(CommandType.Idea);
            Next();
        }
        
        /// <summary>
        /// 次の行動に進める
        /// </summary>
        private void Next()
        {
            // 次のコマンド選択があるかチェックし、適切な状態に遷移
            BattleManager.SetState(BattleManager.CheckNextCommandSelect()
                ? BattleSystemState.CommandSelect : BattleSystemState.Execute);
        }
    }
}