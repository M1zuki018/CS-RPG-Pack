using CryStar.UI;
using DG.Tweening;
using UnityEngine;

namespace CryStar.Story.UI
{
    /// <summary>
    /// UIContents ダイアログ
    /// 会話ダイアログと地の文ダイアログのそれぞれのUIContentsの参照を持っている
    /// ベースクラス内でCanvasGroupでのフェード処理にも対応している
    /// </summary>
    public class UIContents_StoryDialog : UIContentsCanvasGroupBase, IDialogController
    {
        /// <summary>
        /// 名前の表示を行わないパターンのレイアウトがあるオブジェクト
        /// </summary>
        [SerializeField] 
        private UIContents_DialogDescriptionLayout _descriptionLayout;
        
        /// <summary>
        /// 名前の表示を行うパターンのレイアウト
        /// </summary>
        [SerializeField]
        private UIContents_DialogTalkLayout _talkLayout;
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            SetVisibility(false);
            
            // 非表示にする・テキストをクリアする
            _talkLayout.SetVisibility(false);
            _talkLayout.ClearText();
            _descriptionLayout.SetVisibility(false);
            _descriptionLayout.ClearText();
        }
        
        #region 会話ダイアログ

        /// <summary>
        /// 会話ダイアログのテキストを書き換える
        /// </summary>
        public Tween SetTalk(string name, string dialog, float duration = 0)
        {
            if (_descriptionLayout.IsVisible)
            {
                // 地の文ダイアログが表示されていたら非表示にする
                _descriptionLayout.SetVisibility(false);
            }
            
            return _talkLayout.SetTalk(name, dialog, duration);
        }
        
        /// <summary>
        /// 会話ダイアログをリセット
        /// </summary>
        public void ResetTalk()
        {
            _talkLayout.ClearText();
        }

        #endregion

        #region 地の文ダイアログ

        /// <summary>
        /// 地の文ダイアログのテキストを書き換える
        /// </summary>
        public Tween SetDescription(string description, float duration)
        {
            if (_talkLayout.IsVisible)
            {
                // 会話ダイアログが表示されていたら非表示にする
                _talkLayout.SetVisibility(false);
            }
            
            return _descriptionLayout.SetText(description, duration);
        }
        
        /// <summary>
        /// 地の文ダイアログをリセット
        /// </summary>
        public void ResetDescription()
        {
            _descriptionLayout.ClearText();
        }

        #endregion
    }
}