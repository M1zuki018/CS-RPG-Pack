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