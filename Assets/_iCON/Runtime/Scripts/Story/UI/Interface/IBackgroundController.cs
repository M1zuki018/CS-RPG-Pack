using CryStar.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CryStar.Story.UI
{
    /// <summary>
    /// 背景制御機能
    /// </summary>
    public interface IBackgroundController : IImageControllable, IFadeable
    {
        UniTask<Tween> SetBackgroundAsync(string fileName, float duration);
    }
}