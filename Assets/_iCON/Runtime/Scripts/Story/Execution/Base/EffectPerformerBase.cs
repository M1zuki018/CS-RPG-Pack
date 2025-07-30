using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// エフェクトの演出を実行するFactory用のベースクラス
    /// </summary>
    public abstract class EffectPerformerBase : IEffectPerformer
    {
        public abstract EffectOrderType SupportedEffectType { get; }
        public abstract Tween HandlePerformance(OrderData data, StoryView view);
    }
}

