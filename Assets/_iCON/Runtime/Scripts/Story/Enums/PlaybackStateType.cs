namespace CryStar.Story.Enums
{
    /// <summary>
    /// ストーリーの再生状態の列挙型
    /// </summary>
    public enum PlaybackStateType
    {
        /// <summary>
        /// 待機中
        /// </summary>
        Idle,
        
        /// <summary>
        /// 再生中
        /// </summary>
        Playing,
        
        /// <summary>
        /// 一時停止
        /// </summary>
        Paused,
        
        /// <summary>
        /// UI非表示モード
        /// </summary>
        Immersed,
        
        /// <summary>
        /// 完了
        /// </summary>
        Complete
    }
}