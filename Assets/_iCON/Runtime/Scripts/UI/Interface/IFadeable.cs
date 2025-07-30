using DG.Tweening;

namespace CryStar.UI
{
    /// <summary>
    /// フェード機能を持つUIContents
    /// </summary>
    public interface IFadeable : IVisibilityControllable
    {
        Tween FadeIn(float duration);
        Tween FadeOut(float duration);
        Tween FadeToAlpha(float targetAlpha, float duration);
    }
}