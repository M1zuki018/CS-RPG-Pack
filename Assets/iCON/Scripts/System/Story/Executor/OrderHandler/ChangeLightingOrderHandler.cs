using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// ChangeLighting - Global Volume変更処理
    /// </summary>
    [OrderHandler(OrderType.ChangeLighting)]
    public class ChangeLightingOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.ChangeLighting;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            view.ChangeGlobalVolume(data.FilePath).Forget();
            return null;
        }
    }
}