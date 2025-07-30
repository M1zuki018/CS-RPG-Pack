using CryStar.Story.Constants;
using DG.Tweening;

namespace CryStar.UI
{
    /// <summary>
    /// フェード機能を持つUIContents
    /// </summary>
    public interface IFadeable : IVisibilityControllable
    {
        Tween FadeIn(float duration, Ease ease = KStoryPresentation.FADE_EASE);
        Tween FadeOut(float duration, Ease ease = KStoryPresentation.FADE_EASE);
        Tween FadeToAlpha(float targetAlpha, float duration, Ease ease = KStoryPresentation.FADE_EASE);
    }
}