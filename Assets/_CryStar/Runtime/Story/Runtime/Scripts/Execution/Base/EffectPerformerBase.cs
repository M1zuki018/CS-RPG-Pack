using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.Factory;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// ストーリー中の特定のエフェクト演出を処理するための基底クラス
    /// NOTE: このクラスを継承することで、新しいエフェクト演出を簡単に追加できます
    /// </summary>
    public abstract class EffectPerformerBase : IHandlerBase, IEffectPerformer
    {
        /// <summary>
        /// このハンドラが担当するエフェクトの種類
        /// </summary>
        public abstract EffectOrderType SupportedEffectType { get; }

        /// <summary>
        /// エフェクトの演出を実行
        /// NOTE: 継承クラスで具体的な処理を実装する必要があります
        /// </summary>
        /// <param name="data">命令のデータ</param>
        /// <param name="view">ストーリー表示用のUI</param>
        /// <returns>演出用のTweenアニメーション</returns>
        public abstract Tween HandlePerformance(OrderData data, StoryView view);
    }
}

