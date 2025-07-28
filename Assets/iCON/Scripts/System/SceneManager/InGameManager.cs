using System.Linq;
using CryStar.Attribute;
using CryStar.Enums;
using Cysharp.Threading.Tasks;
using iCON.UI;
using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// インゲームのGameManager
    /// </summary>
    public class InGameManager : ViewBase
    {
        /// <summary>
        /// ストーリーマネージャー
        /// </summary>
        [SerializeField, HighlightIfNull]
        private StoryManager _storyManager;

        [SerializeField]
        private PackSample_CanvasController_StorySelect _canvasController;

        public override async UniTask OnStart()
        {
            await base.OnStart();
            
            ServiceLocator.Resister(this, ServiceType.Local);
            
            // ストーリー再生時以外はゲームオブジェクトを非アクティブにしておく
            _storyManager.gameObject.SetActive(false);
        }

        private async void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F8))
            {
                await ServiceLocator.GetGlobal<SceneLoader>().LoadSceneAsync(new SceneTransitionData(SceneType.Title));
            }
        }
        
        public void PlayStory(SceneDataSO sceneDataSo)
        {
            _storyManager.gameObject.SetActive(true);
            _storyManager.PlayStory(sceneDataSo,
                () =>
                {
                    _storyManager.gameObject.SetActive(false);
                    _canvasController.Setup();
                }).Forget();
        }
    }
}