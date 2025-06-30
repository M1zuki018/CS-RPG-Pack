using System;
using System.Collections.Generic;

namespace iCON.System
{
    /// <summary>
    /// チャプター（章）データ
    /// </summary>
    [Serializable]
    public class ChapterData
    {
        /// <summary>
        /// チャプターの管理ID
        /// </summary>
        public int ChapterId { get; set; }
        
        /// <summary>
        /// このチャプターに属するシーンデータ
        /// </summary>
        public List<SceneData> Scenes { get; set; } = new List<SceneData>();
    }   
}