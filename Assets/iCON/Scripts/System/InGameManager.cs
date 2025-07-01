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
        /// スプレッドシート名
        /// </summary>
        [SerializeField] 
        private string _spreadsheetName = "TestStory";

        /// <summary>
        /// 読み込む範囲
        /// </summary>
        [SerializeField]
        private string _range = "TestStory!A2:N15";

        public override async UniTask OnStart()
        {
            await base.OnStart();
            
            // ストーリー再生時以外はゲームオブジェクトを非アクティブにしておく
            _storyManager.gameObject.SetActive(false);
        }
        
        [MethodButtonInspector]
        public void PlayStory()
        {
            _storyManager.gameObject.SetActive(true);
            _storyManager.PlayStory(_spreadsheetName, _range, () => _storyManager.gameObject.SetActive(false)).Forget();
        }
    }
}