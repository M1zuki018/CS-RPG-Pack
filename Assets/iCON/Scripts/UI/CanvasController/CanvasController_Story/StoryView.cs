using DG.Tweening;
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
        [SerializeField, HighlightIfNull]
        private UIContents_StoryBackground _background;
        
        /// <summary>
        /// キャラクター立ち絵管理クラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private UIContents_StoryCharacters _characters;
        
        /// <summary>
        /// スチル管理クラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private UIContents_StorySteel _steel;
        
        /// <summary>
        /// ダイアログ管理クラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private UIContents_StoryDialog _dialog;
        
        /// <summary>
        /// フェードパネル管理クラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private UIContents_FadePanel _fadePanel;

        /// <summary>
        /// 背景を変更する
        /// </summary>
        public void SetBackground(string fileName)
        {
            Debug.Log(fileName);
            _background.SetImage(fileName);
        }

        public Tween InCharacter(CharacterPositionType position, string fileName, float duration)
        {
            return _characters.Show(position, fileName, duration);
        }

        public void OutCharacter(CharacterPositionType position)
        {
            Debug.Log(position);
            _characters.Hide(position);
        }

        public void HideAllCharacters()
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

        /// <summary>
        /// 会話テキストを更新する
        /// </summary>
        public Tween SetTalk(string name, string dialog, float duration)
        {
            if (!_dialog.IsVisible)
            {
                _dialog.Show();
            }
            
            return _dialog.SetTalk(name, dialog, duration);
        }
        
        /// <summary>
        /// 地の文のテキストを更新する
        /// </summary>
        /// <returns></returns>
        public Tween SetDescription(string description, float duration)
        {
            if (!_dialog.IsVisible)
            {
                _dialog.Show();
            }
            
            return _dialog.SetDescription(description, duration);
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        public Tween FadeIn(float duration)
        {
            return _fadePanel.FadeIn(duration);
        }

        /// <summary>
        /// フェードアウト
        /// </summary>
        public Tween FadeOut(float duration)
        {
            return _fadePanel.FadeOut(duration);
        }

        /// <summary>
        /// フェードパネルの表示/非表示を即座に切り替える
        /// </summary>
        public void FadePanelSetVisible(bool visible)
        {
            _fadePanel.SetVisible(visible);
        }
    }
   
}