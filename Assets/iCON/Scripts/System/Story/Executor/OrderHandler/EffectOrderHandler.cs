using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// Effect - エフェクト再生
    /// </summary>
    [OrderHandler(OrderType.Effect)]
    public class EffectOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Effect;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            // TODO
            return null;
        }
    }
}