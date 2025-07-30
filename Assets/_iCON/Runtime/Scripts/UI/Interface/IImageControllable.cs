using Cysharp.Threading.Tasks;

namespace CryStar.UI
{
    /// <summary>
    /// 画像設定機能
    /// </summary>
    public interface IImageControllable
    {
        UniTask SetImageAsync(string fileName);
    }
}