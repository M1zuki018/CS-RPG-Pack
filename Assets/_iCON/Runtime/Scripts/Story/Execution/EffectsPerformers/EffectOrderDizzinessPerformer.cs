using CryStar.Effects;
using CryStar.Story.Attribute;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
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
