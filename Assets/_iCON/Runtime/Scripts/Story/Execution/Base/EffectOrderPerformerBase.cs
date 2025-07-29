using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
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

