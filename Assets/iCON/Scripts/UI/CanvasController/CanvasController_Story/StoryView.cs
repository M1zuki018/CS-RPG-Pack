using iCON.Enums;
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

        /// <summary>
        /// 背景を変更する
        /// </summary>
        public void SetBackground(string fileName)
        {
            Debug.Log(fileName);
            _background.SetImage(fileName);
        }

        public void InCharacter(CharacterPositionType position, string fileName)
        {
            Debug.Log(fileName);
            _characters.Show(position, fileName);
        }

        public void OutCharacter(CharacterPositionType position)
        {
            Debug.Log(position);
            _characters.Hide(position);
        }

        public void HideAll()
        {
            _characters.HideAll();
        }

        public void ChangeCharacter(CharacterPositionType position, string fileName)
        {
            _characters.ChangeSprite(position, fileName);
        }

        public void SetSteel(string fileName)
        {
            Debug.Log(fileName);
            _steel.SetImage(fileName);
        }

        public void HideSteel()
        {
            Debug.Log("HideSteel");
            _steel.Hide();
        }

        public void SetTalk(string name, string dialog)
        {
            Debug.Log(dialog);
            if (!_dialog.IsVisible)
            {
                _dialog.Show();
            }
            _dialog.SetTalk(name, dialog);
        }
        
        public void SetDescription(string description)
        {
            Debug.Log(description);
            if (!_dialog.IsVisible)
            {
                _dialog.Show();
            }
            _dialog.SetDescription(description);
        }
    }
   
}