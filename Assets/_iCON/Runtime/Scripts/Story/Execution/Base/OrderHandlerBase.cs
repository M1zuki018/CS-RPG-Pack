using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// オーダーを実行するHandlerのベースクラス
    /// </summary>
    public abstract class OrderHandlerBase
    {
        public abstract OrderType SupportedOrderType { get; }
        public abstract Tween HandleOrder(OrderData data, StoryView view);
    }
}
