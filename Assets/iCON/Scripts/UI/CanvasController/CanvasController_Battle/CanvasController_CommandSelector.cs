using System;
using CryStar.Attribute;
using Cysharp.Threading.Tasks;
using iCON.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace iCON.UI
{
    /// <summary>
    /// CanvasController_CommandSelector
    /// </summary>
    public partial class CanvasController_CommandSelector : WindowBase
    {
        [SerializeField, HighlightIfNull] private Button _attack;
        [SerializeField, HighlightIfNull] private Button _idea;
        [SerializeField, HighlightIfNull] private Button _item;
        [SerializeField, HighlightIfNull] private Button _guard;
        
        public event Action OnAttack;
        public event Action OnIdea;
        public event Action OnItem;
        public event Action OnGuard;
                
        public override UniTask OnAwake()
        {
            _attack.onClick.SafeReplaceListener(() => OnAttack?.Invoke());
            _idea.onClick.SafeReplaceListener(() => OnIdea?.Invoke());
            _item.onClick.SafeReplaceListener(() => OnItem?.Invoke());
            _guard.onClick.SafeReplaceListener(() => OnGuard?.Invoke());
            return base.OnAwake();
        }
        
        private void OnDestroy()
        {
            _attack.onClick.SafeRemoveAllListeners();
            _idea.onClick.SafeRemoveAllListeners();
            _item.onClick.SafeRemoveAllListeners();
            _guard.onClick.SafeRemoveAllListeners();
        }
    }
}