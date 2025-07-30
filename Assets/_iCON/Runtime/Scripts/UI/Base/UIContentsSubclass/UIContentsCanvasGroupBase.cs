using CryStar.Story.Constants;
using DG.Tweening;
using UnityEngine;

namespace CryStar.UI
{
    /// <summary>
    /// CanvasGroupを利用するUIContentsクラスのベースクラス
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIContentsCanvasGroupBase : UIContentsFadeableBase
    {
        /// <summary>
        /// CanvasGroup
        /// </summary>
        protected CanvasGroup _canvasGroup;
        
        /// <summary>
        /// 透明度
        /// </summary>
        public override float Alpha => _canvasGroup.alpha;
        
        /// <summary>
        /// 現在表示されているか
        /// </summary>
        public override bool IsVisible => _canvasGroup != null && _canvasGroup.alpha > 0;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Initialize()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// 表示状態を設定する
        /// </summary>
        public override void SetVisibility(bool isVisible)
        {
            _canvasGroup.alpha = isVisible ? 1 : 0;
            _canvasGroup.interactable = isVisible;
            _canvasGroup.blocksRaycasts = isVisible;
        }

        /// <summary>
        /// 即座にアルファ値を設定
        /// </summary>
        public override void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;   
        }
        
        /// <summary>
        /// 指定したアルファ値までフェード
        /// </summary>
        public override Tween FadeToAlpha(float targetAlpha, float duration, Ease ease = KStoryPresentation.FADE_EASE)
        {
            _currentTween?.Kill();
            _currentTween = _canvasGroup.DOFade(targetAlpha, duration).SetEase(ease);
            return _currentTween;
        }
    }
}
