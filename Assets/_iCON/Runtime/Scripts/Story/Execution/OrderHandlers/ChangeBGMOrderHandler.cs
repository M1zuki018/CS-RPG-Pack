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