using DG.Tweening;
using iCON.Utility;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// UIContents 名前なしの地の文のダイアログ
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIContents_DialogDescriptionLayout : MonoBehaviour
    {
        /// <summary>
        /// 地の文のText
        /// </summary>
        [SerializeField] 
        private CustomText _description;
        
        /// <summary>
        /// CanvasGroup
        /// </summary>
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// 初期化済みか
        /// </summary>
        private bool _isInitialized;
        
        /// <summary>
        /// 現在表示されているかどうか
        /// </summary>
        public bool IsVisible => _canvasGroup != null && _canvasGroup.alpha > 0f;

        #region Lifecycle

        private void Awake()
        {
            InitializeComponents();
        }

        #endregion

        /// <summary>
        /// 表示テキストを変更する
        /// </summary>
        public Tween SetText(string description, float duration = 0)
        {
            if (!_isInitialized)
            {
                // NOTE: 初期化が正しく行われていない場合はコンポーネントがnullになるためreturnしておく
                return null;    
            }

            if (!IsVisible)
            {
                SetVisibility(true);
            }

            // 一度テキストボックスを空にする
            _description.text = string.Empty;
            return _description.DOText(description ?? string.Empty, duration).SetEase(Ease.Linear);
        }
        
        /// <summary>
        /// テキストをクリアする
        /// </summary>
        public void ClearText()
        {
            SetText(string.Empty);
        }
        
        /// <summary>
        /// 表示状態を設定する
        /// </summary>
        public void SetVisibility(bool isActive)
        {
            _canvasGroup.alpha = isActive ? 1 : 0;
            _canvasGroup.interactable = isActive;
            _canvasGroup.blocksRaycasts = isActive;
        }

        #region Private Methods

        /// <summary>
        /// 初期化
        /// </summary>
        private void InitializeComponents()
        {
            // エラーがあるか、この変数に記録する
            bool hasError = false;

            if (_description == null)
            {
                LogUtility.Error($"_description が null です。割り当てを行ってください", LogCategory.UI, this);
                hasError = true;
            }
            
            _canvasGroup = GetComponent<CanvasGroup>();
            _isInitialized = !hasError;
        }

        #endregion
    }
}