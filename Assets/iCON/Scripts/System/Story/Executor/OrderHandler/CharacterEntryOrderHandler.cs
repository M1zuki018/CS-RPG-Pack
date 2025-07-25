using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// CharacterEntry - キャラクター登場
    /// </summary>
    [OrderHandler(OrderType.CharacterEntry)]
    public class CharacterEntryOrderHandler : BaseOrderHandler
    {
        public override OrderType SupportedOrderType => OrderType.CharacterEntry;

        public override Tween Handler(OrderData data, StoryView view)
        {
            return view.CharacterEntry(data.Position, data.FacialExpressionPath, data.Duration);
        }
    }
}