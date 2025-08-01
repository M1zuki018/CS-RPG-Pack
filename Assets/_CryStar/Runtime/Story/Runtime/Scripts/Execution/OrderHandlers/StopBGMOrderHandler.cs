using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.System;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// StopBGM - BGM再生を止める
    /// </summary>
    [OrderHandler(OrderType.StopBGM)]
    public class StopBGMOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.StopBGM;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            AudioManager.Instance.FadeOutBGM(data.Duration).Forget();
            return null;
        }
    }
}