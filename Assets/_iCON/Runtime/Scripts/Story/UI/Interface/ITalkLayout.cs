using DG.Tweening;

namespace CryStar.UI
{
    /// <summary>
    /// 名前付きダイアログレイアウト
    /// </summary>
    public interface ITalkLayout : ITextDisplayable
    {
        Tween SetTalk(string name, string dialog, float duration = 0f);
        void SetName(string name);
        Tween SetDialog(string dialog, float duration = 0f);
    }
}