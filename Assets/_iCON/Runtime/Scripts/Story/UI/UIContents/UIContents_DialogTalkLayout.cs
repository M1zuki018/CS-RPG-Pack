using DG.Tweening;
using iCON.Utility;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// UIContents 名前つきのダイアログ
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIContents_DialogTalkLayout : MonoBehaviour
    {
        /// <summary>
        /// 名前のText
        /// </summary>
        [SerializeField]
        private CustomText _name;

        /// <summary>
        /// 会話文のText
        /// </summary>
        [SerializeField] 
        private CustomText _dialog;
        
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
        public Tween SetText(string name, string dialog, float duration = 0)
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
            
            SetName(name);
            return SetDialog(dialog, duration);
        }
        
        /// <summary>
        /// 名前のみを設定する
        /// </summary>
        public void SetName(string name)
        {
            if (!_isInitialized) return;
            
            if (!IsVisible)
            {
                SetVisibility(true);
            }
            
            _name.text = name ?? string.Empty;
        }
        
        /// <summary>
        /// 会話文のみを設定する
        /// </summary>
        public Tween SetDialog(string dialog, float duration = 0)
        {
            if (!_isInitialized)
            {
                return null;
            }
            
            if (!IsVisible)
            {
                SetVisibility(true);
            }

            // テキストボックスを空にしてから始める
            _dialog.text = string.Empty;
            return _dialog.DOText(dialog ?? string.Empty, duration).SetEase(Ease.Linear);
        }
        
        /// <summary>
        /// テキストをクリアする
        /// </summary>
        public void ClearText()
        {
            SetText(string.Empty, string.Empty);
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
            
            if (_name == null)
            {
                LogUtility.Error($"_name が null です。割り当てを行ってください", LogCategory.UI, this);
                hasError = true;
            }

            if (_dialog == null)
            {
                LogUtility.Error($"_dialog が null です。割り当てを行ってください", LogCategory.UI, this);
                hasError = true;
            }
            
            _canvasGroup = GetComponent<CanvasGroup>();
            _isInitialized = !hasError;
        }

        #endregion
    }
}