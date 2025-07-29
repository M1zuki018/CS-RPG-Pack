using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;
using iCON.System;
using iCON.Utility;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// Start - ストーリー開始処理 
    /// </summary>
    [OrderHandler(OrderType.Start)]
    public class StartOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Start;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            // ログを流す
            LogUtility.Verbose("Story started", LogCategory.System);
            
            // フェードイン処理
            return view.FadeIn(data.Duration);
        }
    }
}