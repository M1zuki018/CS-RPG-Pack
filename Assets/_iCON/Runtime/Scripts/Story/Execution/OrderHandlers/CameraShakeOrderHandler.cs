using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;
using iCON.System;

namespace CryStar.Story.Execution
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