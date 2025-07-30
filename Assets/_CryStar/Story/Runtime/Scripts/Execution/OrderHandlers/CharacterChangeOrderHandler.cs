using System.Threading;
using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CryStar.Story.Execution
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