using System.Threading;
using CryStar.Story.Data;
using CryStar.Story.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// 非同期対応のオーダーハンドラーインターフェース
    /// </summary>
    public interface IAsyncOrderHandler
    {
        /// <summary>
        /// 非同期的に命令を実行
        /// </summary>
        UniTask<Tween> HandleOrderAsync(OrderData data, StoryView view, CancellationToken cancellationToken);
    }
}