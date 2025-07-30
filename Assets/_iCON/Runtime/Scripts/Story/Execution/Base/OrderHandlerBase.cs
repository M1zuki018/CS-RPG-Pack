using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// オーダーを実行するFactory用のベースクラス
    /// </summary>
    public abstract class OrderHandlerBase : IOrderHandler
    {
        public abstract OrderType SupportedOrderType { get; }
        public abstract Tween HandleOrder(OrderData data, StoryView view);
    }
}
