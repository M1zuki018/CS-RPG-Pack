using DG.Tweening;
using iCON.Enums;
using iCON.UI;
using iCON.Utility;
using NUnit.Framework;

namespace iCON.System
{
    /// <summary>
    /// Talk - キャラクターのセリフ表示
    /// </summary>
    [OrderHandler(OrderType.Talk)]
    public class TalkOrderHandler : BaseOrderHandler
    {
        public override OrderType SupportedOrderType => OrderType.Talk;
        
        public override Tween Handler(OrderData data, StoryView view)
        {
            // 名前付きのダイアログを表示する
            return view.SetTalk(data.DisplayName, data.DialogText, data.Duration);
        }
    }
}