using System;
using CryStar.Story.Constants;
using CryStar.UI;
using CryStar.Utility;
using CryStar.Utility.Enum;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace CryStar.Story.UI
{
    /// <summary>
    /// UIContents ストーリーのスチル表示を管理する
    /// </summary>
    public class UIContents_StorySteel : UIContentsCanvasGroupBase, ISteelController
    {
        /// <summary>
        /// スチル画像
        /// </summary>
        [SerializeField] 
        private CustomImage[] _steelImages = new CustomImage[2];

        /// <summary>
        /// 現在アクティブなスチル画像のインデックス
        /// </summary>
        private int _activeImageIndex = 0;

        /// <summary>
        /// 次に使用するスチル画像のインデックス
        /// </summary>
        private int NextSteelIndex => (_activeImageIndex + 1) % _steelImages.Length;
        
        /// <summary>
        /// 初期化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            InitializeSteelImages();
            
            // 初期状態で非表示にしておく
            SetVisibility(false);
        }
        
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
        public override Tween FadeIn(float duration, Ease ease = KStoryPresentation.FADE_EASE)
        {
            if (!IsVisible)
            {
                // 表示
                SetVisibility(true);
            }
            
            return _steelImages[_activeImageIndex].DOFade(1, duration)
                .SetEase(ease)
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
        public override Tween FadeOut(float duration, Ease ease = KStoryPresentation.FADE_EASE)
        {
            return _steelImages[_activeImageIndex].DOFade(0, duration)
                .SetEase(ease);
        }

        /// <summary>
        /// 指定したアルファ値までフェード
        /// </summary>
        public override Tween FadeToAlpha(float targetAlpha, float duration, Ease ease = KStoryPresentation.FADE_EASE)
        {
            return _steelImages[_activeImageIndex].DOFade(targetAlpha, duration)
                .SetEase(ease);
        }
        
        /// <summary>
        /// 表示状態を設定する
        /// </summary>
        public override void SetVisibility(bool isActive)
        {
            base.SetVisibility(isActive);

            if (!isActive)
            {
                foreach (var steel in _steelImages)
                {
                    // キャンバスを非表示にする時は各画像も非表示にする
                    steel.Hide();
                }
            }
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

        #endregion
    }
}