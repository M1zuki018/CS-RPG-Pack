using CryStar.UI;
using DG.Tweening;
using UnityEngine;

namespace CryStar.Story.UI
{
    /// <summary>
    /// UIContents ダイアログ
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIContents_StoryDialog : UIContentsBase, IDialog
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
        /// CanvasGroup
        /// </summary>
        private CanvasGroup _canvasGroup;
        
        /// <summary>
        /// 現在表示中かどうか
        /// </summary>
        public bool IsVisible => _canvasGroup != null && _canvasGroup.alpha > 0;

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
        
        /// <summary>
        /// ダイアログを表示する
        /// </summary>
        public Tween FadeIn(float duration)
        {
            if (!IsVisible)
            {
                SetVisibility(true);
            }

            return _canvasGroup.DOFade(1, duration);
        }

        /// <summary>
        /// ダイアログを非表示にする
        /// </summary>
        public Tween FadeOut(float duration)
        {
            return _canvasGroup.DOFade(0, duration);
        }
        
        /// <summary>
        /// 指定したアルファ値までフェード
        /// </summary>
        public Tween FadeToAlpha(float targetAlpha, float duration)
        {
            return _canvasGroup.DOFade(targetAlpha, duration);
        }
        
        /// <summary>
        /// 表示/非表示を切り替える
        /// </summary>
        public void SetVisibility(bool isVisible)
        {
            _canvasGroup.alpha = isVisible ? 1 : 0;
            _canvasGroup.interactable = isVisible;
            _canvasGroup.blocksRaycasts = isVisible;
        }
        
        public override void Initialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            
            SetVisibility(false);
            
            // 非表示にする・テキストをクリアする
            _talkLayout.SetVisibility(false);
            _talkLayout.ClearText();
            _descriptionLayout.SetVisibility(false);
            _descriptionLayout.ClearText();
        }
    }
}