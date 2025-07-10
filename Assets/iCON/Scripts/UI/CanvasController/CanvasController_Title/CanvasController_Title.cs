using System;
using Cysharp.Threading.Tasks;
using iCON.System;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// CanvasController_Title
    /// </summary>
    public partial class CanvasController_Title : WindowBase
    {
        [SerializeField, HighlightIfNull] private CustomButton _startButton;
        
        public event Action OnStartButtonClicked;
        
        public override UniTask OnAwake()
        {
            // イベント登録
            if(_startButton != null) _startButton.onClick.AddListener(HandleStartButtonClicked);
            return base.OnAwake();
        }

        private void HandleStartButtonClicked()
        {
            ServiceLocator.GetGlobal<SceneLoader>().LoadSceneAsync(new SceneTransitionData(SceneType.InGame, true)).Forget();
            OnStartButtonClicked?.Invoke();
        }

        private void OnDestroy()
        {
            if(_startButton != null) _startButton.onClick?.RemoveAllListeners();
        }
    }
}