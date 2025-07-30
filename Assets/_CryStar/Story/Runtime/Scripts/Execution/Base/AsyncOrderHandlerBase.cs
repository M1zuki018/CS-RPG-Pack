using System.Threading;
using CryStar.Story.Data;
using CryStar.Story.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// 非同期処理をサポートするストーリー命令ハンドラの基底クラス
    /// NOTE: UniTaskを利用して、時間のかかる処理（例：Addressableでのアセットロード）を非同期で実行します。
    /// </summary>
    public abstract class AsyncOrderHandlerBase : OrderHandlerBase, IAsyncOrderHandler
    {
        /// <summary>
        /// 同期的に命令を実行
        /// </summary>
        /// <remarks>
        /// 下位互換性のために提供されていますが、内部では非同期メソッドを同期的に待機させています。
        /// この方法はデッドロックを引き起こす可能性があるため、可能な限り <see cref="HandleOrderAsync"/> の使用を推奨します
        /// </remarks>
        /// <param name="data">命令のデータ</param>
        /// <param name="view">ストーリー表示用のUI</param>
        /// <returns>処理に関連するTweenアニメーション</returns>
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            // NOTE: 非同期版を同期的に実行（推奨されないが下位互換性のため）
            return HandleOrderAsync(data, view, CancellationToken.None).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 非同期的に命令を実行
        /// NOTE: 継承クラスで具体的な処理を実装する必要があります。
        /// </summary>
        /// <param name="data">命令のデータ</param>
        /// <param name="view">ストーリー表示用のUI</param>
        /// <param name="cancellationToken">非同期処理のキャンセル用トークン</param>
        /// <returns>処理に関連するTweenアニメーションを返すUniTask</returns>
        public abstract UniTask<Tween> HandleOrderAsync(OrderData data, StoryView view,
            CancellationToken cancellationToken);
    }
}
