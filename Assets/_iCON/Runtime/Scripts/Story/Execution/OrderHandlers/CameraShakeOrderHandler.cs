using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.Extensions;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// CameraShake - カメラシェイク
    /// </summary>
    [OrderHandler(OrderType.CameraShake)]
    public class CameraShake : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.CameraShake;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return view.CameraShake(data.Duration, data.OverrideTextSpeed);
        }
    }
}