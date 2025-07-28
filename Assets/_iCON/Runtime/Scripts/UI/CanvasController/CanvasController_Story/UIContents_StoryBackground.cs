using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Constants;
using iCON.Utility;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// UIContents ストーリーの背景
    /// </summary>
    public class UIContents_StoryBackground : MonoBehaviour
    {
        /// <summary>
        /// 背景画像
        /// </summary>
        [SerializeField] 
        private CustomImage[] _bgImages = new CustomImage[2];

        /// <summary>
        /// 現在アクティブな背景画像のインデックス
        /// </summary>
        private int _activeImageIndex = 0;

        /// <summary>
        /// 次に使用する背景画像のインデックス
        /// </summary>
        private int NextImageIndex => (_activeImageIndex + 1) % _bgImages.Length;

        #region Lifecycle

        private void Awake()
        {
            InitializeBackgroundImages();
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
                int nextIndex = NextImageIndex;
                await _bgImages[nextIndex].ChangeSpriteAsync(fileName);

                // オブジェクトをhierarchyの末尾に移動させて、最前面に表示されるようにする
                _bgImages[nextIndex].transform.SetAsLastSibling();

                // アクティブインデックスを更新
                _activeImageIndex = nextIndex;
            }
            catch (Exception ex)
            {
                LogUtility.Error($"画像設定中にエラーが発生しました: {ex.Message}", LogCategory.UI, _bgImages[_activeImageIndex]);
            }
        }
        
        /// <summary>
        /// フェードイン
        /// </summary>
        public Tween FadeIn(float duration)
        {
            return _bgImages[_activeImageIndex].DOFade(1, duration)
                .SetEase(KStoryPresentation.FADE_EASE)
                .OnComplete(() =>
                {
                    // 前面のオブジェクトが表示されたら、裏面のオブジェクトの透明度をゼロにしておく
                    int prevIndex = _activeImageIndex == 0 ? _bgImages.Length - 1 : _activeImageIndex - 1;
                    _bgImages[prevIndex].Hide();
                });
        }

        #region Private Method

        /// <summary>
        /// 背景画像コンポーネントの初期化
        /// </summary>
        private void InitializeBackgroundImages()
        {
            for (int i = 0; i < _bgImages.Length; ++i)
            {
                if (_bgImages[i] == null)
                {
                    // 配列がnullなら子オブジェクトから取得する
                    _bgImages[i] = transform.GetChild(i).GetComponent<CustomImage>();
                }
            }
        }

        #endregion
    }
}