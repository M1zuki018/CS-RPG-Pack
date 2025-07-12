using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// CanvasController_InGame
    /// </summary>
    public partial class CanvasController_InGame : WindowBase
    {
        [SerializeField, HighlightIfNull] private CustomButton _button;
        [SerializeField, HighlightIfNull] private StoryView _storyView;
        
        public event Action OnButtonClicked;
                
        public override UniTask OnAwake()
        {
            // イベント登録
            if(_button != null) _button.onClick.AddListener(Temporary);
            
            return base.OnAwake();
        }

        public override UniTask OnUIInitialize()
        {
            if(_storyView != null) _storyView.SetActive(false); // 非表示からスタート
            return base.OnUIInitialize();
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