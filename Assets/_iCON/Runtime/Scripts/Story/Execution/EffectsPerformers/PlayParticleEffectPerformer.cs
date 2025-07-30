using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// PlayParticle - ParticleSystemでエフェクトを再生
    /// </summary>
    [EffectPerformer(EffectOrderType.PlayParticle)]
    public class PlayParticleEffectPerformer : ParticleAccessPerformerBase
    {
        public override EffectOrderType SupportedEffectType => EffectOrderType.PlayParticle;
        
        public override Tween HandlePerformance(OrderData data, StoryView view)
        {
            EnsureParticleManager();
            
            // NOTE: 配列のインデックスとして扱うために-1してゼロオリジンに変換
            ParticleManager.PlayParticle((int)data.OverrideTextSpeed - 1);
            return null;
        }
    }
}