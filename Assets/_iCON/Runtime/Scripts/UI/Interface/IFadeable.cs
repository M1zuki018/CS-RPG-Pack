using DG.Tweening;

namespace CryStar.UI
{
    /// <summary>
    /// フェード機能を持つUIContentsが継承すべきインターフェース
    /// </summary>
    public interface IFadeable : IVisibilityControllable
    {
        /// <summary>
        /// フェードイン
        /// </summary>
        Tween FadeIn(float duration, Ease ease = Ease.Linear);
        
        /// <summary>
        /// フェードアウト
        /// </summary>
        Tween FadeOut(float duration, Ease ease = Ease.Linear);
        
        /// <summary>
        /// 指定した透明度にフェードを行う
        /// </summary>
        Tween FadeToAlpha(float targetAlpha, float duration, Ease ease = Ease.Linear);
    }
}