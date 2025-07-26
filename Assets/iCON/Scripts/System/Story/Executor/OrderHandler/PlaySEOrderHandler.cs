using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// PlaySE - SEを再生する
    /// </summary>
    [OrderHandler(OrderType.PlaySE)]
    public class PlaySEOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.PlaySE;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            AudioManager.Instance.PlaySE(data.FilePath, data.OverrideTextSpeed).Forget();
            return null;
        }
    }
}