using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// HideDialog - ダイアログを非表示にする
    /// </summary>
    [OrderHandler(OrderType.HideDialog)]
    public class HideDialogOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.HideDialog;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return view.HideDialog(data.Duration);
        }
    }
}