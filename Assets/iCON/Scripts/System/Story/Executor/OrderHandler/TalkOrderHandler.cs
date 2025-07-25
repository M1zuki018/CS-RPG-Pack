using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// Talk - キャラクターのセリフ表示
    /// </summary>
    [OrderHandler(OrderType.Talk)]
    public class TalkOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Talk;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            // 名前付きのダイアログを表示する
            return view.SetTalk(data.DisplayName, data.DialogText, data.Duration);
        }
    }
}