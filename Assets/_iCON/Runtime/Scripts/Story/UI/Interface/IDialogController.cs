using DG.Tweening;

namespace CryStar.Story.UI
{
    /// <summary>
    /// ストーリーの会話ダイアログと地の文ダイアログの両方を統括して管理するクラスが継承すべきインターフェース
    /// </summary>
    public interface IDialogController
    {
        /// <summary>
        /// 会話ダイアログのテキストを書き換える
        /// </summary>
        Tween SetTalk(string name, string dialog, float duration);
        
        /// <summary>
        /// 会話ダイアログをリセット
        /// </summary>
        void ResetTalk();
        
        /// <summary>
        /// 地の文ダイアログのテキストを書き換える
        /// </summary>
        Tween SetDescription(string description, float duration);
        
        /// <summary>
        /// 地の文ダイアログをリセット
        /// </summary>
        void ResetDescription();
    }
}