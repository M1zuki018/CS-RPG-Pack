using System;

namespace iCON.System
{
    /// <summary>
    /// チャプターデータ
    /// </summary>
    [Serializable]
    public class ChapterData
    {
        /// <summary>
        /// チャプターの管理ID
        /// </summary>
        public int ChapterId { get; set; }
        
        /// <summary>
        /// このチャプターに属するシーンの数
        /// </summary>
        public int SceneCount { get; set; }
    }   
}