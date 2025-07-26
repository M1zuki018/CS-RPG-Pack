using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// ChangeBGM - BGM変更
    /// </summary>
    [OrderHandler(OrderType.ChangeBGM)]
    public class ChangeBGMOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.ChangeBGM;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            AudioManager.Instance.CrossFadeBGM(data.FilePath, data.Duration).Forget();
            return null;
        }
    }
}