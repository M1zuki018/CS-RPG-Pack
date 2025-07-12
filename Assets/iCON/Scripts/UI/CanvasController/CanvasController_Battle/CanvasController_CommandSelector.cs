using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// CanvasController_CommandSelector
    /// </summary>
    public partial class CanvasController_CommandSelector : WindowBase
    {
        [SerializeField, HighlightIfNull] private CustomButton _button;
                
        public event Action OnButtonClicked;
                
        public override UniTask OnAwake()
        {
            // イベント登録
            if(_button != null) _button.onClick.AddListener(Temporary);
            return base.OnAwake();
        }
        
        private void Temporary()
        {
        }
        
        private void OnDestroy()
        {
            if(_button != null) _button.onClick?.RemoveAllListeners();
        }
    }
}