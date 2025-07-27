using DG.Tweening;
using iCON.Enums;
using iCON.System;

namespace iCON.UI
{
    /// <summary>
    /// エフェクトオーダーを受けて演出するベースクラス
    /// </summary>
    public abstract class EffectOrderPerformerBase
    {
        public abstract EffectOrderType SupportedEffectType { get; }
        public abstract Tween HandlePerformance(OrderData data, StoryView view);
    }
}

