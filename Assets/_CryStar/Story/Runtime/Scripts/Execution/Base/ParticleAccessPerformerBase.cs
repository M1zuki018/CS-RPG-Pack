using CryStar.Core;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// ParticleManagerを利用するEffectPerformer用のベースクラス
    /// </summary>
    public abstract class ParticleAccessPerformerBase : EffectPerformerBase
    {
        /// <summary>
        /// PerticleManagerのインスタンス
        /// </summary>
        protected ParticleManager ParticleManager { get; private set; }

        /// <summary>
        /// このハンドラが担当するエフェクトの種類
        /// </summary>
        public override EffectOrderType SupportedEffectType { get; }

        /// <summary>
        /// 初回実行時にParticleManagerの参照をServiceLocatorから取得する
        /// </summary>
        protected void EnsureParticleManager()
        {
            ParticleManager ??= ServiceLocator.GetLocal<ParticleManager>();
        }
        
        /// <summary>
        /// エフェクトの演出を実行
        /// NOTE: 継承クラスで具体的な処理を実装する必要があります
        /// </summary>
        public abstract override Tween HandlePerformance(OrderData data, StoryView view);
    }
}