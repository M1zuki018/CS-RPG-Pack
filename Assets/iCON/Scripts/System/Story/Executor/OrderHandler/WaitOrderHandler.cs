using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.Extensions;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// Wait - 待機処理
    /// </summary>
    [OrderHandler(OrderType.Wait)]
    public class WaitOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Wait;
        
        public override UniTask<Tween> HandleAsync(OrderData data, StoryView view)
        {
            return DOTween.To(() => 0f, _ => { }, 1f, data.Duration).ToUniTaskWithResult();
        }
    }
}