using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// ShowSteel - スチル画像表示
    /// </summary>
    [OrderHandler(OrderType.ShowSteel)]
    public class ShowSteelOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.ShowSteel;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return view.SetSteel(data.FilePath, data.Duration);
        }
    }
}