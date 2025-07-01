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

        private void OnEnable()
        {
            _storyManager.enabled = false;
        }
        
        [MethodButtonInspector]
        public void PlayStory()
        {
            _storyManager.enabled = true;
            _storyManager.PlayStory("TestStory", "TestStory!A2:N15", () => Debug.Log("End")).Forget();
        }
    }
}