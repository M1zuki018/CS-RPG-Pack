using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// Wait - 待機処理
    /// </summary>
    [OrderHandler(OrderType.Wait)]
    public class WaitOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Wait;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return DOTween.To(() => 0f, _ => { }, 1f, data.Duration);
        }
    }
}