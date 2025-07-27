using DG.Tweening;
using iCON.Enums;
using iCON.System;
using UnityEngine;

namespace iCON.UI
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
            // TODO: 仮実装
            return view.Flash(data.Duration, Color.white);
        }
    }

}
