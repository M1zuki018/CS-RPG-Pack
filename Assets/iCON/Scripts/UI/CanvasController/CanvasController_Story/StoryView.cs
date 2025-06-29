using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// Story View
    /// </summary>
    public class StoryView : MonoBehaviour
    {
        /// <summary>
        /// 背景管理クラス
        /// </summary>
        [SerializeField]
        private UIContents_StoryBackground _background;
        
        /// <summary>
        /// キャラクター立ち絵管理クラス
        /// </summary>
        [SerializeField]
        private UIContents_StoryCharacters _characters;
        
        /// <summary>
        /// スチル管理クラス
        /// </summary>
        [SerializeField]
        private UIContents_StorySteel _steel;
        
        /// <summary>
        /// ダイアログ管理クラス
        /// </summary>
        [SerializeField]
        private UIContents_StoryDialog _dialog;
        
        
    }
   
}