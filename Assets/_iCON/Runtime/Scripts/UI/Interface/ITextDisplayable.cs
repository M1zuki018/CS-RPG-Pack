using DG.Tweening;

namespace CryStar.UI
{
    /// <summary>
    /// Tweenを使ったテキスト表示機能を持つUIContents
    /// </summary>
    public interface ITextDisplayable
    {
        Tween SetText(string text, float duration = 0f);
        void ClearText();
    }
}