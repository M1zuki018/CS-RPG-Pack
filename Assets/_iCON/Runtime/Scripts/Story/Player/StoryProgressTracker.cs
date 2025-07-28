namespace iCON.System
{
    /// <summary>
    /// ストーリー進行状態を管理するクラス
    /// NOTE: 現在位置の保持と移動を行う
    /// </summary>
    public class StoryProgressTracker
    {
        /// <summary>
        /// 現在のストーリー位置
        /// </summary>
        public int CurrentPosition { get; private set; } = 0;
        
        /// <summary>
        /// 次のオーダーに進む
        /// </summary>
        public void MoveToNextOrder()
        {
            CurrentPosition++;
        }

        public void AddPosition(int value)
        {
            CurrentPosition += value;
        }
        
        /// <summary>指定位置にジャンプ</summary>
        public void JumpToPosition(int position)
        {
            CurrentPosition = position;
        }
        
        /// <summary>進行状態をリセット</summary>
        public void Reset()
        {
            CurrentPosition = 0;
        }
    }   
}