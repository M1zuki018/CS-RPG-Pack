using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
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