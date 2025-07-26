using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// HideSteel - スチル画像非表示
    /// </summary>
    [OrderHandler(OrderType.HideSteel)]
    public class HideSteelOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.HideSteel;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return view.HideSteel(data.Duration);
        }
    }
}