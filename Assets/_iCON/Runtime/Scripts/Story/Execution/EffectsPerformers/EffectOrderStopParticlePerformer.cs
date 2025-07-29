using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;
using iCON.System;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// StopParticle - ParticleSystemのエフェクトを停止
    /// </summary>
    [EffectOrderHandler(EffectOrderType.StopParticle)]
    public class EffectOrderStopParticlePerformer : EffectOrderPerformerBase
    {
        /// <summary>
        /// ParticleManager
        /// </summary>
        private ParticleManager _particleManager;
        
        public override EffectOrderType SupportedEffectType => EffectOrderType.StopParticle;
        
        public override Tween HandlePerformance(OrderData data, StoryView view)
        {
            if (_particleManager == null)
            {
                // 参照がない場合、サービスロケーターから取得する
                _particleManager = ServiceLocator.GetLocal<ParticleManager>();
            }
            
            // NOTE: 配列のインデックスとして扱うために-1してゼロオリジンに変換
            _particleManager.StopParticle((int)data.OverrideTextSpeed - 1);
            return null;
        }
    }

}
