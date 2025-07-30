namespace CryStar.UI
{
    /// <summary>
    /// 表示/非表示制御可能なUIContents
    /// </summary>
    public interface IVisibilityControllable
    {
        bool IsVisible { get; }
        void SetVisibility(bool isVisible);
    }
}