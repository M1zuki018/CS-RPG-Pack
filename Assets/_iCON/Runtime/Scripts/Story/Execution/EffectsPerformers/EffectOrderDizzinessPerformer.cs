using DG.Tweening;
using iCON.Enums;
using iCON.System;

namespace iCON.UI
{
    /// <summary>
    /// Dizziness - めまいエフェクト
    /// </summary>
    [EffectOrderHandler(EffectOrderType.Dizziness)]
    public class EffectOrderDizzinessPerformer : EffectOrderPerformerBase
    {
        public override EffectOrderType SupportedEffectType => EffectOrderType.Dizziness;
        
        public override Tween HandlePerformance(OrderData data, StoryView view)
        {
            EffectsManager.Instance.DizzinessEffect((int)data.OverrideTextSpeed == 1);
            return null;
        }
    }
}
