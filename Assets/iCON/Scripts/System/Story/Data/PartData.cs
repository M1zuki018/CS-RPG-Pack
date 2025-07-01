using System;
using System.Collections.Generic;

namespace iCON.System
{
    /// <summary>
    /// パートデータ
    /// </summary>
    [Serializable]
    public class PartData
    {
        /// <summary>
        /// パートの管理ID
        /// </summary>
        public int PartId { get; set; }
        
        /// <summary>
        /// このパートに属するチャプターの数
        /// </summary>
        public List<ChapterData> Chapters { get; set; }
    }
}