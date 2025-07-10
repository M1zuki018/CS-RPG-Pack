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
            
            // ストーリー再生時以外はゲームオブジェクトを非アクティブにしておく
            _storyManager.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F7))
            {
                PlayStory();
            }
        }
        
        [MethodButtonInspector]
        public void PlayStory()
        {
            _storyManager.gameObject.SetActive(true);
            
            // TODO: 仮作成
            var playLine = _storyLine.FirstOrDefault(line => line.SceneName == _playStoryName);
            if (playLine != null)
            {
                _storyManager.PlayStory(playLine.SpreadsheetName, playLine.HeaderRange, playLine.Range,
                    () => _storyManager.gameObject.SetActive(false)).Forget();
            }
                
        }
    }
}