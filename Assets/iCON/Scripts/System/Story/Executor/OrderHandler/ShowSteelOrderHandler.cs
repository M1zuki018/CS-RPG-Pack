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
    public class ShowSteelOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.ShowSteel;
        
        public override async UniTask<Tween> HandleAsync(OrderData data, StoryView view)
        {
            return await view.SetSteel(data.FilePath, data.Duration);
        }
    }
}