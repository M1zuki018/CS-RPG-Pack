using DG.Tweening;

namespace CryStar.UI
{
    /// <summary>
    /// Tweenを使ったテキスト表示機能を持つUIContentsが継承すべきインターフェース
    /// </summary>
    public interface ITextDisplayable
    {
        /// <summary>
        /// テキストをセットする
        /// </summary>
        Tween SetText(string text, float duration = 0f);
        
        /// <summary>
        /// テキストをクリアする
        /// </summary>
        void ClearText();
    }
}