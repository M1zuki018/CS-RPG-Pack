using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.Factory;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// ストーリーの命令（オーダー）を処理するための基底クラス
    /// NOTE: このクラスを継承することで、新しい種類の命令を処理するハンドラを定義できます
    /// </summary>
    public abstract class OrderHandlerBase : IHandlerBase, IOrderHandler
    {
        /// <summary>
        /// このハンドラが担当するオーダーの種類
        /// </summary>
        public abstract OrderType SupportedOrderType { get; }

        /// <summary>
        /// オーダーを実行する
        /// NOTE: 継承クラスで具体的な処理を実装する必要があります
        /// </summary>
        /// <param name="data">命令のデータ</param>
        /// <param name="view">ストーリー表示用のUI</param>
        /// <returns>処理に関連するTweenアニメーション</returns>
        public abstract Tween HandleOrder(OrderData data, StoryView view);
    }
}
