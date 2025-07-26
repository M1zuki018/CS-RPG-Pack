using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// Choice - 選択肢表示
    /// </summary>
    [OrderHandler(OrderType.Choice)]
    public class ChoiceOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Choice;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            // TODO
            return null;
        }
    }
}