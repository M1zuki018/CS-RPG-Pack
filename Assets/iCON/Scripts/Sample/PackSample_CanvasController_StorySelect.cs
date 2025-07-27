using System;
using CryStar.Attribute;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// PackSample_CanvasController_StorySelect
    /// </summary>
    public partial class PackSample_CanvasController_StorySelect : WindowBase
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