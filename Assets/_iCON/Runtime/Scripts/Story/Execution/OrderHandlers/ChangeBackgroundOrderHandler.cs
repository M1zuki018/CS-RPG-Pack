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
    /// ChangeBackground - 背景変更
    /// </summary>
    [OrderHandler(OrderType.ChangeBackground)]
    public class ChangeBackgroundOrderHandler : AsyncOrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.ChangeBackground;
        
        public override async UniTask<Tween> HandleOrderAsync(OrderData data, StoryView view, CancellationToken cancellationToken)
        {
            return await view.SetBackground(data.FilePath, data.Duration);
        }
    }
}