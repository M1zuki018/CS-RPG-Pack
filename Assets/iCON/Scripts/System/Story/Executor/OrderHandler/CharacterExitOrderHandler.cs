using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// CharacterExit - キャラクター退場
    /// </summary>
    [OrderHandler(OrderType.CharacterExit)]
    public class CharacterExitOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.CharacterExit;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return view.CharacterExit(data.Position, data.Duration);
        }
    }
}