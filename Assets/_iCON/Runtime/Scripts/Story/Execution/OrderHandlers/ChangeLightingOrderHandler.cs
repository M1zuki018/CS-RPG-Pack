using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CryStar.Story.Execution
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