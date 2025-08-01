using CryStar.UI;
using DG.Tweening;

namespace CryStar.Story.UI
{
    /// <summary>
    /// 名前付きダイアログのUIContentsが継承すべきインターフェース
    /// </summary>
    public interface ITalkLayout : ITextDisplayable
    {
        /// <summary>
        /// 表示テキストを変更する
        /// </summary>
        Tween SetTalk(string name, string dialog, float duration = 0f);
        
        /// <summary>
        /// 名前のみを設定する
        /// </summary>
        void SetName(string name);
        
        /// <summary>
        /// 会話文のみを設定する
        /// </summary>
        Tween SetDialog(string dialog, float duration = 0f);
    }
}