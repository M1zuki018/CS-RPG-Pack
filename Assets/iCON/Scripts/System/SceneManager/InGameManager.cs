using System.Linq;
using Cysharp.Threading.Tasks;
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

        /// <summary>
        /// 再生したいストーリーの名前
        /// </summary>
        [SerializeField]
        private string _playStoryName;
        
        /// <summary>
        /// データを読み込む際に必要なデータ
        /// </summary>
        [SerializeField]
        private StoryLine[] _storyLine;

        public override async UniTask OnStart()
        {
            await base.OnStart();
            
            ServiceLocator.Resister(this, ServiceType.Local);
            
            // ストーリー再生時以外はゲームオブジェクトを非アクティブにしておく
            _storyManager.gameObject.SetActive(false);
        }

        private async void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F7))
            {
                var playLine = _storyLine.FirstOrDefault(line => line.SceneName == _playStoryName);
                if (playLine != null)
                {
                    PlayStory(playLine.SpreadsheetName, playLine.HeaderRange, playLine.Range);
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F8))
            {
                await ServiceLocator.GetGlobal<SceneLoader>().LoadSceneAsync(new SceneTransitionData(SceneType.Title));
            }
        }
        
        [MethodButtonInspector]
        public void PlayStory(string spreadsheetName, string headerRange, string range)
        {
            _storyManager.gameObject.SetActive(true);
            _storyManager.PlayStory(spreadsheetName, headerRange, range, 
                () => _storyManager.gameObject.SetActive(false)).Forget();
        }
    }
}