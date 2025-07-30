using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// StopParticle - ParticleSystemのエフェクトを停止
    /// </summary>
    [EffectPerformer(EffectOrderType.StopParticle)]
    public class StopParticleEffectPerformer : ParticleAccessPerformerBase
    {
        public override EffectOrderType SupportedEffectType => EffectOrderType.StopParticle;
        
        public override Tween HandlePerformance(OrderData data, StoryView view)
        {
            EnsureParticleManager();
            
            // NOTE: 配列のインデックスとして扱うために-1してゼロオリジンに変換
            ParticleManager.StopParticle((int)data.OverrideTextSpeed - 1);
            return null;
        }
    }

}
