using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
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