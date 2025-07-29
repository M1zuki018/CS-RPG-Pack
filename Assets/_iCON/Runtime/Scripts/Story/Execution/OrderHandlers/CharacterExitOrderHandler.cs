using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;
using iCON.System;

namespace CryStar.Story.Execution
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