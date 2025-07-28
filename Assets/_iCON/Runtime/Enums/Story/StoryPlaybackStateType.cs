namespace iCON.System
{
    /// <summary>
    /// ストーリー再生状態
    /// </summary>
    public enum StoryPlaybackStateType
    {
        /// <summary>
        /// 停止中（デフォルト）
        /// </summary>
        Stopped,
        
        /// <summary>
        /// 再生中
        /// </summary>
        Playing,
        
        /// <summary>
        /// 一時停止
        /// </summary>
        Paused,
        
        /// <summary>
        /// 入力待ち
        /// </summary>
        WaitingInput,
        
        /// <summary>
        /// 遷移中
        /// </summary>
        Transitioning
    }
}