using DG.Tweening;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// UIContents ダイアログ
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIContents_StoryDialog : MonoBehaviour
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

        #region Lifecycle

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Hide();
        }

        #endregion

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
        /// 会話ダイアログのテキストを書き換える
        /// </summary>
        public Tween SetTalk(string name, string dialog, float duration = 0)
        {
            if (_descriptionLayout.IsVisible)
            {
                // 地の文ダイアログが表示されていたら非表示にする
                _descriptionLayout.SetVisibility(false);
            }
            
            return _talkLayout.SetText(name, dialog, duration);
        }
        
        /// <summary>
        /// ダイアログを表示する
        /// </summary>
        public void Show()
        {
            SetVisibility(true);
        }
        
        /// <summary>
        /// ダイアログを非表示にする
        /// </summary>
        public void Hide()
        {
            SetVisibility(false);
            
            // 非表示にする・テキストをクリアする
            _talkLayout.SetVisibility(false);
            _talkLayout.ClearText();
            _descriptionLayout.SetVisibility(false);
            _descriptionLayout.ClearText();
        }

        #region Private Methods

        /// <summary>
        /// 自身の表示状態を設定する
        /// </summary>
        private void SetVisibility(bool isActive)
        {
            _canvasGroup.alpha = isActive ? 1 : 0;
            _canvasGroup.interactable = isActive;
            _canvasGroup.blocksRaycasts = isActive;
        }
        
        #endregion
    }
}