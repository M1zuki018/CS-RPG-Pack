using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
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