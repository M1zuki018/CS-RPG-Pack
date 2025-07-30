using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// FadeIn - フェードイン
    /// </summary>
    [OrderHandler(OrderType.FadeIn)]
    public class FadeInOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.FadeIn;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return view.FadeIn(data.Duration);
        }
    }
}