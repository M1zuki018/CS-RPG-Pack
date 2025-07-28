using System.Collections.Generic;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// Effect - エフェクト再生
    /// </summary>
    [OrderHandler(OrderType.Effect)]
    public class EffectOrderHandler : OrderHandlerBase
    {
        /// <summary>
        /// 各エフェクトの列挙型と処理を行うPerformerのインスタンスのkvp
        /// </summary>
        private Dictionary<EffectOrderType, EffectOrderPerformerBase> _performers;
        
        public override OrderType SupportedOrderType => OrderType.Effect;

        /// <summary>
        /// Setup
        /// </summary>
        public void SetupPerformerCache(StoryView view)
        {
            _performers = EffectOrderPerformerFactory.CreateAllHandlers(view);
        }
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return _performers[(EffectOrderType)data.SpeakerId].HandlePerformance(data, view);
        }
    }
}