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
    /// ShowSteel - スチル画像表示
    /// </summary>
    [OrderHandler(OrderType.ShowSteel)]
    public class ShowSteelOrderHandler : AsyncOrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.ShowSteel;
        
        public override async UniTask<Tween> HandleOrderAsync(OrderData data, StoryView view, CancellationToken cancellationToken)
        {
            return await view.SetSteel(data.FilePath, data.Duration);
        }
    }
}