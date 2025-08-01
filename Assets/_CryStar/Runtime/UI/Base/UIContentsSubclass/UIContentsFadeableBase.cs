using CryStar.Story.Constants;
using DG.Tweening;

namespace CryStar.UI
{
    /// <summary>
    /// フェード機能を持つUIContentsのベースクラス
    /// </summary>
    public abstract class UIContentsFadeableBase : UIContentsBase, IFadeable, IAlphaControllable, IAnimationControllable
    {
        protected Tween _currentTween;
        
        /// <summary>
        /// 透明度
        /// </summary>
        public abstract float Alpha { get; }
        
        /// <summary>
        /// 表示状態かどうか
        /// </summary>
        public abstract bool IsVisible { get; }

        /// <summary>
        /// アニメーション中か
        /// </summary>
        public bool IsAnimating => _currentTween != null && _currentTween.IsActive();
        
        /// <summary>
        /// 表示/非表示を即座に切り替え
        /// </summary>
        public abstract void SetVisibility(bool isVisible);

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void Dispose()
        {
            StopAnimation();
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        public virtual Tween FadeIn(float duration, Ease ease = KStoryPresentation.FADE_EASE)
        {
            return FadeToAlpha(1, duration, ease);
        }

        /// <summary>
        /// フェードアウト
        /// </summary>
        public virtual Tween FadeOut(float duration, Ease ease = KStoryPresentation.FADE_EASE)
        {
            return FadeToAlpha(0, duration, ease);
        }

        /// <summary>
        /// 即座にアルファ値を設定（派生クラスで実装）
        /// </summary>
        public abstract void SetAlpha(float alpha);
        
        /// <summary>
        /// 指定したアルファ値までフェード（派生クラスで実装）
        /// </summary>
        public abstract Tween FadeToAlpha(float targetAlpha, float duration, Ease ease = KStoryPresentation.FADE_EASE);
        
        /// <summary>
        /// 現在のアニメーションを停止
        /// </summary>
        public void StopAnimation()
        {
            _currentTween?.Kill();
            _currentTween = null;
        }
    }
}
