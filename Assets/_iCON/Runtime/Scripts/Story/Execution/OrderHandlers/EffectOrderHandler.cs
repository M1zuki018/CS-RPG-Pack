using System.Collections.Generic;
using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.Factory;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
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
        private Dictionary<EffectOrderType, EffectPerformerBase> _performers;
        
        public override OrderType SupportedOrderType => OrderType.Effect;

        /// <summary>
        /// Setup
        /// </summary>
        public void SetupPerformerCache(StoryView view)
        {
            _performers = EffectPerformerFactory.CreateAllHandlers(view);
        }
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            return _performers[(EffectOrderType)data.SpeakerId].HandlePerformance(data, view);
        }
    }
}