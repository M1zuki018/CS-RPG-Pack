using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// 非同期対応のオーダーハンドラーインターフェース
    /// </summary>
    public interface IAsyncOrderHandler
    {
        /// <summary>
        /// 非同期でオーダーを処理する
        /// </summary>
        UniTask<Tween> HandleOrderAsync(OrderData data, StoryView view, CancellationToken cancellationToken);
    }
}