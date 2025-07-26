using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// CharacterChange - キャラクター切り替え
    /// </summary>
    [OrderHandler(OrderType.CharacterChange)]
    public class CharacterChangeOrderHandler : AsyncOrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.CharacterChange;

        public override async UniTask<Tween> HandleOrderAsync(OrderData data, StoryView view, CancellationToken cancellationToken)
        {
            return await view.ChangeCharacter(data.Position, data.FacialExpressionPath, data.Duration);
        }
    }
}