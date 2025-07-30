using Cysharp.Threading.Tasks;

namespace CryStar.UI
{
    /// <summary>
    /// 非同期での画像設定機能を持つUIContentsが継承すべきインターフェース
    /// </summary>
    public interface IImageControllable
    {
        /// <summary>
        /// 非同期で画像をロードし設定する
        /// </summary>
        UniTask SetImageAsync(string fileName);
    }
}