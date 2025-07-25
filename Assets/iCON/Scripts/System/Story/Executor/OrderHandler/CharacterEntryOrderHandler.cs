using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.Extensions;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// CharacterEntry - キャラクター登場
    /// </summary>
    [OrderHandler(OrderType.CharacterEntry)]
    public class CharacterEntryOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.CharacterEntry;

        public override UniTask<Tween> HandleAsync(OrderData data, StoryView view)
        {
            return view.CharacterEntry(data.Position, data.FacialExpressionPath, data.Duration).ToUniTaskWithResult();
        }
    }
}