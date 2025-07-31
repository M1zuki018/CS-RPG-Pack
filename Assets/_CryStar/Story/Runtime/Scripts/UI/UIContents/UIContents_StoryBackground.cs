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
    /// UIContents ストーリーの背景
    /// </summary>
    public class UIContents_StoryBackground : UIContentsFadeableBase, IBackgroundController
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
        
        /// <summary>
        /// 現在アクティブな背景画像
        /// </summary>
        private CustomImage ActiveImage => _bgImages[_activeImageIndex];

        /// <summary>
        /// 透明度
        /// </summary>
        public override float Alpha => ActiveImage?.color.a ?? 0f;
        
        /// <summary>
        /// 表示中か
        /// </summary>
        public override bool IsVisible => Alpha > 0;

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Initialize()
        {
            InitializeBackgroundImages();
        }

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
        public override Tween FadeIn(float duration, Ease ease = KStoryPresentation.FADE_EASE)
        {
            _currentTween?.Kill();
            _currentTween = FadeToAlpha(1, duration, ease)
                .OnComplete(() =>
                {
                    // 前面のオブジェクトが表示されたら、裏面のオブジェクトの透明度をゼロにしておく
                    int prevIndex = _activeImageIndex == 0 ? _bgImages.Length - 1 : _activeImageIndex - 1;
                    _bgImages[prevIndex].Hide();
                });
            
            return _currentTween;
        }
        
        /// <summary>
        /// 透明度を指定してフェード処理を行う
        /// </summary>
        public override Tween FadeToAlpha(float targetAlpha, float duration, Ease ease = KStoryPresentation.FADE_EASE)
        {
            _currentTween?.Kill();
            _currentTween = _bgImages[_activeImageIndex].DOFade(targetAlpha, duration).SetEase(ease);
            
            return _currentTween;
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
        
        public override void SetAlpha(float alpha) { }
        
        public override void SetVisibility(bool isVisible) { }
    }
}