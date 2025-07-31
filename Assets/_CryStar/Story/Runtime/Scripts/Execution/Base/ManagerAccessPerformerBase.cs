using CryStar.Core;
using CryStar.Effects;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// EffectManagerを利用するEffectPerformer用のベースクラス
    /// </summary>
    public abstract class ManagerAccessPerformerBase : EffectPerformerBase
    {
        /// <summary>
        /// EffectManagerのインスタンス
        /// </summary>
        protected EffectManager EffectManager { get; private set; }
        
        /// <summary>
        /// このハンドラが担当するエフェクトの種類
        /// </summary>
        public override EffectOrderType SupportedEffectType { get; }

        /// <summary>
        /// 初回実行時にEffectManagerの参照をServiceLocatorから取得する
        /// </summary>
        protected void EnsureEffectManager()
        {
            EffectManager ??= ServiceLocator.GetLocal<EffectManager>();
        }
        
        /// <summary>
        /// エフェクトの演出を実行
        /// NOTE: 継承クラスで具体的な処理を実装する必要があります
        /// </summary>
        public abstract override Tween HandlePerformance(OrderData data, StoryView view);
    }
}