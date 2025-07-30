using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;
using UnityEngine;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// Flash - フラッシュエフェクト
    /// </summary>
    [EffectPerformer(EffectOrderType.Flash)]
    public class FlashEffectPerformer : EffectPerformerBase
    {
        public override EffectOrderType SupportedEffectType => EffectOrderType.Flash;
        
        public override Tween HandlePerformance(OrderData data, StoryView view)
        {
            ColorUtility.TryParseHtmlString(data.OverrideDisplayName, out var color);
            return view.Flash(data.Duration, color);
        }
    }
}
