using DG.Tweening;

namespace CryStar.UI
{
    /// <summary>
    /// 会話ダイアログ機能
    /// </summary>
    public interface IDialog  : IFadeable
    {
        Tween SetTalk(string name, string dialog, float duration);
        Tween SetDescription(string description, float duration);
        void ResetTalk();
        void ResetDescription();
    }
}