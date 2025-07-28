using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
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