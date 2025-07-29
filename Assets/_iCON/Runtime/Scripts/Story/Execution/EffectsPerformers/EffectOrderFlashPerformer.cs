using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;
using iCON.System;
using UnityEngine;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// Flash - フラッシュエフェクト
    /// </summary>
    [EffectOrderHandler(EffectOrderType.Flash)]
    public class EffectOrderFlashPerformer : EffectOrderPerformerBase
    {
        public override EffectOrderType SupportedEffectType => EffectOrderType.Flash;
        
        public override Tween HandlePerformance(OrderData data, StoryView view)
        {
            ColorUtility.TryParseHtmlString(data.OverrideDisplayName, out var color);
            return view.Flash(data.Duration, color);
        }
    }
}
