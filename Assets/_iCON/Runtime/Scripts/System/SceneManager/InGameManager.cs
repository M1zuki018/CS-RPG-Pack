using CryStar.Attribute;
using CryStar.Core;
using CryStar.Enums;
using CryStar.Story.Core;
using CryStar.Story.Orchestrators;
using Cysharp.Threading.Tasks;
using iCON.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace iCON.System
{
    /// <summary>
    /// インゲームのGameManager
    /// </summary>
    public class InGameManager : CustomBehaviour
    {
        /// <summary>
        /// ストーリーオーケストレーター
        /// </summary>
        [SerializeField, HighlightIfNull]
        private StoryOrchestrator _storyOrchestrator;

        [SerializeField]
        private PackSample_CanvasController_StorySelect _canvasController;

        public override async UniTask OnStart()
        {
            await base.OnStart();
            
            ServiceLocator.Resister(this, ServiceType.Local);
            
            // ストーリー再生時以外はゲームオブジェクトを非アクティブにしておく
            _storyOrchestrator.gameObject.SetActive(false);
        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                await ServiceLocator.GetGlobal<SceneLoader>().LoadSceneAsync(new SceneTransitionData(SceneType.Title));
            }
        }
        
        public void PlayStory(int storyId)
        {
            _storyOrchestrator.gameObject.SetActive(true);
            _storyOrchestrator.PlayStoryAsync(storyId,
                () =>
                {
                    _storyOrchestrator.gameObject.SetActive(false);
                    _canvasController.Setup();
                }).Forget();
        }
    }
}