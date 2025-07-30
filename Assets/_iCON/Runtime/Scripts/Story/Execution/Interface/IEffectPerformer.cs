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
        /// このハンドラが担当するエフェクトの種類
        /// </summary>
        EffectOrderType SupportedEffectType { get; }
        
        /// <summary>
        /// エフェクトの演出を実行
        /// </summary>
        Tween HandlePerformance(OrderData data, StoryView view);
    }
}