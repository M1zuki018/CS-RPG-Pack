namespace iCON.System
{
    /// <summary>
    /// 画面遷移の状態
    /// </summary>
    public enum LoadingStateType
    {
        /// <summary>
        /// 遷移が行われていない状態
        /// </summary>
        None,
        
        /// <summary>
        /// 遷移中
        /// </summary>
        Loading,
        
        /// <summary>
        /// 遷移完了
        /// </summary>
        Completed,
        
        /// <summary>
        /// 遷移失敗
        /// </summary>
        Failed
    }
}