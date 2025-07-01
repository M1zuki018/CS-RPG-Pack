using System;

namespace iCON.System
{
    /// <summary>
    /// ストーリー進行状態
    /// </summary>
    [Serializable]
    public class StoryProgress
    {
        /// <summary>現在のパートID</summary>
        public int CurrentPart { get; set; } = 1;
        
        /// <summary>現在のチャプターID</summary>
        public int CurrentChapterId { get; set; } = 1;
        
        /// <summary>現在のシーンID</summary>
        public int CurrentSceneId { get; set; } = 1;
        
        /// <summary>現在のオーダー</summary>
        public int CurrentOrderIndex { get; set; } = 0;
        
        /// <summary>オーダーを読み切ったか</summary>
        public bool IsCompleted { get; set; } = false;
    
        /// <summary>
        /// 次のオーダーに進む
        /// </summary>
        public void NextOrder()
        {
            CurrentOrderIndex++;
        }
    
        /// <summary>
        /// 次のシーンに進む
        /// </summary>
        public void NextScene()
        {
            CurrentSceneId++;
            CurrentOrderIndex = 0;
        }
    
        /// <summary>
        /// 次のチャプターに進む
        /// </summary>
        public void NextChapter()
        {
            CurrentChapterId++;
            CurrentSceneId = 1;
            CurrentOrderIndex = 0;
        }

        /// <summary>
        /// 次のパートに進む
        /// </summary>
        public void NextPart()
        {
            CurrentPart++;
            CurrentChapterId = 1;
            CurrentSceneId = 1;
            CurrentOrderIndex = 0;
        }
    
        /// <summary>
        /// 特定の位置にジャンプ
        /// </summary>
        public void JumpTo(int partId, int chapterId, int sceneId, int orderIndex = 0)
        {
            CurrentPart = partId;
            CurrentChapterId = chapterId;
            CurrentSceneId = sceneId;
            CurrentOrderIndex = orderIndex;
        }
    }   
}