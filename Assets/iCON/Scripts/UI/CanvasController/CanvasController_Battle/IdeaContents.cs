using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace iCON.Battle
{
    /// <summary>
    /// Idea（スキル）のコンテンツ
    /// </summary>
    public class IdeaContents : MonoBehaviour
    {
        /// <summary>
        /// ボタンリスト
        /// </summary>
        [SerializeField]
        private List<Button> _ideaButtons;
        
        /// <summary>
        /// CanvasGroup
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        /// <summary>
        /// アイデア選択用のボタンリスト
        /// </summary>
        public List<Button> IdeaButtons => _ideaButtons;
        
        /// <summary>
        /// CanvasGroup
        /// </summary>
        public CanvasGroup CanvasGroup => _canvasGroup;
    }
}
