using System;
using Cysharp.Threading.Tasks;
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
        
        /// <summary>
        /// バトル開始
        /// </summary>
        public event Action OnStartBattle;
        
        /// <summary>
        /// 逃走
        /// </summary>
        public event Action OnTryEscape;
                
        public override UniTask OnAwake()
        {
            // イベント登録
            _battle.onClick.SafeReplaceListener(() => OnStartBattle?.Invoke());
            _escape.onClick.SafeReplaceListener(() => OnTryEscape?.Invoke());
            return base.OnAwake();
        }
        
        private void OnDestroy()
        {
            _battle.onClick.SafeRemoveAllListeners();
            _escape.onClick.SafeRemoveAllListeners();
        }
    }
}