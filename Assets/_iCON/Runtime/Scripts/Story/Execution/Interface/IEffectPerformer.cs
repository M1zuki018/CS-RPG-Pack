using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// EffectPerformerが継承すべきインターフェース
    /// </summary>
    public interface IEffectPerformer
    {
        /// <summary>
        /// このパフォーマーがサポートするエフェクトタイプ
        /// </summary>
        EffectOrderType SupportedEffectType { get; }
        
        /// <summary>
        /// 演出を実行する
        /// </summary>
        Tween HandlePerformance(OrderData data, StoryView view);
    }
}