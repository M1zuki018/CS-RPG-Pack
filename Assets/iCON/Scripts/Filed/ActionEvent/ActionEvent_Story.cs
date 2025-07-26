using iCON.System;
using UnityEngine;
using UnityEngine.Serialization;

namespace iCON.Field.System
{
    /// <summary>
    /// ストーリー再生を行うアクションイベント
    /// </summary>
    public class ActionEvent_Story : ActionEventBase
    {
        /// <summary>
        /// ストーリー再生用データ
        /// </summary>
        [SerializeField]
        private StoryExecuteDataSO _storyExecuteData;

        protected override void OnPlayerEnter(Collider2D playerCollider)
        {
            var storyManager = ServiceLocator.GetLocal<InGameManager>();
            storyManager.PlayStory(_storyExecuteData);
        }
    }
}