using iCON.Enums;
using iCON.UI;
using iCON.Utility;

namespace iCON.Battle
{
    /// <summary>
    /// 行動選択
    /// </summary>
    public class CommandSelectState : BattleStateBase
    {
        private CanvasController_CommandSelector _canvasController;
        
        public override void Enter(BattleManager manager, BattleCanvasManager view)
        {
            base.Enter(manager, view);
            view.ShowCanvas(BattleCanvasType.CommandSelector);

            _canvasController = view.CurrentCanvas as CanvasController_CommandSelector;
            if (_canvasController == null)
            {
                LogUtility.Fatal("CanvasController_CommandSelector が取得できませんでした", LogCategory.Gameplay);
                return;
            }
            
            _canvasController.OnAttack += Attack;
            _canvasController.OnIdea += Idea;
            _canvasController.OnItem += Item;
            _canvasController.OnGuard += Guard;
        }

        public override void Exit()
        {
            base.Exit();
            
            _canvasController.OnAttack -= Attack;
            _canvasController.OnIdea -= Idea;
            _canvasController.OnItem -= Item;
            _canvasController.OnGuard -= Guard;
        }

        /// <summary>
        /// 攻撃コマンドを選択したときの処理
        /// </summary>
        private void Attack()
        {
            // コマンドを記録 TODO: 仮実装
            BattleManager.RecordCommand("攻撃");
            Next();
        }

        private void Idea()
        {
            
        }

        private void Item()
        {
            
        }

        /// <summary>
        /// ガード
        /// </summary>
        private void Guard()
        {
            BattleManager.RecordCommand("ガード");
            Next();
        }
        
        /// <summary>
        /// 次の行動に進める
        /// </summary>
        private void Next()
        {
            // 次のコマンド選択に移れるか確認
            if (BattleManager.CheckNextCommandSelect())
            {
                BattleManager.SetState(BattleSystemState.CommandSelect);
            }
            else
            {
                // バトル実行に移る
                BattleManager.SetState(BattleSystemState.Execute);
            }
        }
    }
}