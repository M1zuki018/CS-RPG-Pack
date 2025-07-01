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
        public StoryPosition CurrentPosition { get; private set; } = new StoryPosition(1, 1, 1, 0);
        
        /// <summary>
        /// 次のオーダーに進む
        /// </summary>
        public void MoveToNextOrder()
        {
            CurrentPosition = CurrentPosition.NextOrder();
        }
        
        /// <summary>次のシーンに進む</summary>
        public void MoveToNextScene()
        {
            CurrentPosition = CurrentPosition.NextScene();
        }
        
        /// <summary>
        /// 次のチャプターに進む
        /// </summary>
        public void MoveToNextChapter()
        {
            CurrentPosition = CurrentPosition.NextChapter();
        }
        
        /// <summary>
        /// 次のパートに進む
        /// </summary>
        public void MoveToNextPart()
        {
            CurrentPosition = CurrentPosition.NextPart();
        }
        
        /// <summary>指定位置にジャンプ</summary>
        public void JumpToPosition(int partId, int chapterId, int sceneId, int orderIndex = 0)
        {
            CurrentPosition = new StoryPosition(partId, chapterId, sceneId, orderIndex);
        }
        
        /// <summary>指定位置にジャンプ</summary>
        public void JumpToPosition(StoryPosition position)
        {
            CurrentPosition = position.Clone();
        }
        
        /// <summary>進行状態をリセット</summary>
        public void Reset()
        {
            CurrentPosition = new StoryPosition(1, 1, 1, 0);
        }
    }   
}