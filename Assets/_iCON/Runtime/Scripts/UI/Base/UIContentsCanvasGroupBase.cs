using CryStar.Story.Constants;
using DG.Tweening;
using UnityEngine;

namespace CryStar.UI
{
    /// <summary>
    /// CanvasGroupを利用するUIContentsクラスのベースクラス
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIContentsCanvasGroupBase : UIContentsBase, IFadeable
    {
        /// <summary>
        /// CanvasGroup
        /// </summary>
        protected CanvasGroup _canvasGroup;
        
        /// <summary>
        /// 現在表示されているか
        /// </summary>
        public bool IsVisible => _canvasGroup != null && _canvasGroup.alpha > 0;

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
        public virtual void SetVisibility(bool isVisible)
        {
            _canvasGroup.alpha = isVisible ? 1 : 0;
            _canvasGroup.interactable = isVisible;
            _canvasGroup.blocksRaycasts = isVisible;
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
        /// 指定したアルファ値までフェード
        /// </summary>
        public virtual Tween FadeToAlpha(float targetAlpha, float duration, Ease ease = KStoryPresentation.FADE_EASE)
        {
            return _canvasGroup.DOFade(targetAlpha, duration).SetEase(ease);
        }
    }
}
