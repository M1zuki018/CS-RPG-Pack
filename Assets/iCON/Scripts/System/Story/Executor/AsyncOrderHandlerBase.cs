using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// 非同期対応のオーダーハンドラーベースクラス
    /// </summary>
    public abstract class AsyncOrderHandlerBase : OrderHandlerBase, IAsyncOrderHandler
    {
        /// <summary>
        /// 同期版（デフォルト実装は非同期版を呼び出す）
        /// </summary>
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            // NOTE: 非同期版を同期的に実行（推奨されないが下位互換性のため）
            return HandleOrderAsync(data, view, CancellationToken.None).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 非同期版の実装を継承クラスで定義
        /// </summary>
        public abstract UniTask<Tween> HandleOrderAsync(OrderData data, StoryView view,
            CancellationToken cancellationToken);
    }
}
