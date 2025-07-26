using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// CharacterChange - キャラクター切り替え
    /// </summary>
    [OrderHandler(OrderType.CharacterChange)]
    public class CharacterChangeOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.CharacterChange;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return view.ChangeCharacter(data.Position, data.FacialExpressionPath, data.Duration);
        }
    }
}