namespace CryStar.UI
{
    /// <summary>
    /// 表示/非表示制御可能なUIContentsが継承すべきインターフェース
    /// </summary>
    public interface IVisibilityControllable
    {
        /// <summary>
        /// 表示中か
        /// </summary>
        bool IsVisible { get; }
        
        /// <summary>
        /// 表示/非表示を切り替える
        /// </summary>
        void SetVisibility(bool isVisible);
    }
}