using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// CharacterEntry - キャラクター登場
    /// </summary>
    [OrderHandler(OrderType.CharacterEntry)]
    public class CharacterEntryOrderHandler : AsyncOrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.CharacterEntry;

        public override async UniTask<Tween> HandleOrderAsync(OrderData data, StoryView view, CancellationToken cancellationToken)
        {
            return await view.CharacterEntry(data.Position, data.FacialExpressionPath, data.Duration);
        }
    }
}