using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.Extensions;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// HideSteel - スチル画像非表示
    /// </summary>
    [OrderHandler(OrderType.HideSteel)]
    public class HideSteelOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.HideSteel;
        
        public override UniTask<Tween> HandleAsync(OrderData data, StoryView view)
        {
            return view.HideSteel(data.Duration).ToUniTaskWithResult();
        }
    }
}