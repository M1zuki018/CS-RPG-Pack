using iCON.System;
using UnityEngine;

namespace iCON.Field.System
{
    /// <summary>
    /// ストーリー再生を行うアクションイベント
    /// </summary>
    public class ActionEvent_Story : ActionEventBase
    {
        /// <summary>
        /// ストーリー開始位置
        /// </summary>
        [SerializeField]
        private StoryLine _storyLine;

        protected override void OnPlayerEnter(Collider2D playerCollider)
        {
            var storyManager = ServiceLocator.GetLocal<InGameManager>();
            storyManager.PlayStory(_storyLine.SpreadsheetName, _storyLine.HeaderRange, _storyLine.Range);
        }
    }
}