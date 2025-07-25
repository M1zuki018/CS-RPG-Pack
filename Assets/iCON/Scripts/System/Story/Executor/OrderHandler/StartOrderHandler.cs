using DG.Tweening;
using iCON.Enums;
using iCON.UI;
using iCON.Utility;

namespace iCON.System
{
    /// <summary>
    /// Start - ストーリー開始処理 
    /// </summary>
    [OrderHandler(OrderType.Start)]
    public class StartOrderHandler : BaseOrderHandler
    {
        public override OrderType SupportedOrderType => OrderType.Start;
        
        public override Tween Handler(OrderData data, StoryView view)
        {
            // ログを流す
            LogUtility.Verbose("Story started", LogCategory.System);
            
            // フェードイン処理
            return view.FadeIn(data.Duration);
        }
    }
}