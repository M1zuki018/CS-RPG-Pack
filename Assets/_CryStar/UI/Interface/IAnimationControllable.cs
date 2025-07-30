namespace CryStar.UI
{
    /// <summary>
    /// アニメーション制御の必要があるUIContentsが継承すべきインターフェース
    /// Tweenなどと一緒に利用する想定
    /// </summary>
    public interface IAnimationControllable
    {
        /// <summary>
        /// アニメーション中か
        /// </summary>
        bool IsAnimating { get; }
        
        /// <summary>
        /// 現在のアニメーションを停止
        /// </summary>
        void StopAnimation();
    }
}