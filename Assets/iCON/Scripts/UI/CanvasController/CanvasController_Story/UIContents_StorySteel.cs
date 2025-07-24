using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Constants;
using iCON.Utility;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// UIContents ストーリーのスチル表示を管理する
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIContents_StorySteel : MonoBehaviour
    {
        /// <summary>
        /// スチル画像
        /// </summary>
        [SerializeField] 
        private CustomImage[] _steelImages = new CustomImage[2];
        
        /// <summary>
        /// CanvasGroup
        /// </summary>
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// 現在アクティブなスチル画像のインデックス
        /// </summary>
        private int _activeImageIndex = 0;

        /// <summary>
        /// 次に使用するスチル画像のインデックス
        /// </summary>
        private int NextSteelIndex => (_activeImageIndex + 1) % _steelImages.Length;
        
        /// <summary>
        /// 現在表示中かどうか
        /// </summary>
        public bool IsVisible => _canvasGroup != null && _canvasGroup.alpha > 0;
        
        #region Lifecycle

        private void Awake()
        {
            InitializeSteelImages();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            // 初期状態で非表示にしておく
            SetVisibility(false);
        }

        #endregion

        /// <summary>
        /// ファイル名を元に画像を変更する
        /// </summary>
        public async UniTask SetImageAsync(string fileName)
        {
            try
            {
                // 次の画像を準備
                int nextIndex = NextSteelIndex;
                await _steelImages[nextIndex].ChangeSpriteAsync(fileName);

                // オブジェクトをhierarchyの末尾に移動させて、最前面に表示されるようにする
                _steelImages[nextIndex].transform.SetAsLastSibling();

                // アクティブインデックスを更新
                _activeImageIndex = nextIndex;
            }
            catch (Exception ex)
            {
                LogUtility.Error($"画像設定中にエラーが発生しました: {ex.Message}", LogCategory.UI, _steelImages[_activeImageIndex]);
            }
        }
        
        /// <summary>
        /// フェードイン
        /// </summary>
        public Tween FadeIn(float duration)
        {
            if (!IsVisible)
            {
                Show();
            }
            
            return _steelImages[_activeImageIndex].DOFade(1, duration)
                .SetEase(KStoryPresentation.FADE_EASE)
                .OnComplete(() =>
                {
                    // 前面のスチルが表示されたら、裏面のスチルの透明度をゼロにしておく
                    int prevIndex = _activeImageIndex == 0 ? _steelImages.Length - 1 : _activeImageIndex - 1;
                    _steelImages[prevIndex].Hide();
                });
        }

        /// <summary>
        /// フェードアウト
        /// </summary>
        public Tween FadeOut(float duration)
        {
            return _steelImages[_activeImageIndex].DOFade(0, duration)
                .SetEase(KStoryPresentation.FADE_EASE);
        }

        /// <summary>
        /// 表示
        /// </summary>
        public void Show()
        {
            SetVisibility(true);
        }
        
        /// <summary>
        /// 非表示
        /// </summary>
        public void Hide()
        {
            SetVisibility(false);
        }
        
        #region Private Methods

        /// <summary>
        /// スチル画像コンポーネントの初期化
        /// </summary>
        private void InitializeSteelImages()
        {
            for (int i = 0; i < _steelImages.Length; ++i)
            {
                if (_steelImages[i] == null)
                {
                    // 配列がnullなら子オブジェクトから取得する
                    _steelImages[i] = transform.GetChild(i).GetComponent<CustomImage>();
                }
            }
        }
        
        /// <summary>
        /// 表示状態を設定する
        /// </summary>
        private void SetVisibility(bool isActive)
        {
            _canvasGroup.alpha = isActive ? 1 : 0;
            _canvasGroup.interactable = isActive;
            _canvasGroup.blocksRaycasts = isActive;

            if (!isActive)
            {
                foreach (var steel in _steelImages)
                {
                    // キャンバスを非表示にする時は各画像も非表示にする
                    steel.Hide();
                }
            }
        }

        #endregion
    }
}