using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// FadeOut - フェードアウト
    /// </summary>
    [OrderHandler(OrderType.FadeOut)]
    public class FadeOutOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.FadeOut;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return view.FadeOut(data.Duration);
        }
    }
}