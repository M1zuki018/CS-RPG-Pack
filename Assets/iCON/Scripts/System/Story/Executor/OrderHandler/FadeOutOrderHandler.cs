using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.Extensions;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// FadeOut - フェードアウト
    /// </summary>
    [OrderHandler(OrderType.FadeOut)]
    public class FadeOutOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.FadeOut;
        
        public override UniTask<Tween> HandleAsync(OrderData data, StoryView view)
        {
            return view.FadeOut(data.Duration).ToUniTaskWithResult();
        }
    }
}