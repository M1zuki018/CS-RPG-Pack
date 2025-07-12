using Cysharp.Threading.Tasks;
using iCON.Battle;
using iCON.Enums;
using iCON.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace iCON.UI
{
    /// <summary>
    /// CanvasController_FirstSelect
    /// </summary>
    public partial class CanvasController_FirstSelect : WindowBase
    {
        /// <summary>
        /// たたかうボタン
        /// </summary>
        [SerializeField, HighlightIfNull] 
        private Button _battle;
        
        /// <summary>
        /// にげるボタン
        /// </summary>
        [SerializeField, HighlightIfNull] 
        private Button _escape;
                
        public override UniTask OnAwake()
        {
            // イベント登録
            _battle.onClick.SafeReplaceListener(OnStartBattle);
            _escape.onClick.SafeReplaceListener(OnEscape);
            return base.OnAwake();
        }
        
        /// <summary>
        /// バトル開始を通知する
        /// </summary>
        private void OnStartBattle()
        {
            // 行動選択に移る
            ServiceLocator.GetLocal<BattleManager>().SetState(BattleSystemState.CommandSelect);
        }

        /// <summary>
        /// 逃げるボタンが呼ばれたことを通知する
        /// </summary>
        private void OnEscape()
        {
            ServiceLocator.GetLocal<BattleManager>().SetState(BattleSystemState.TryEscape);
        }
        
        private void OnDestroy()
        {
            _battle.onClick.SafeRemoveAllListeners();
            _escape.onClick.SafeRemoveAllListeners();
        }
    }
}