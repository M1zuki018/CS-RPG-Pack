using System;
using System.Collections.Generic;

namespace iCON.System
{
    /// <summary>
    /// パート（編）データ
    /// </summary>
    [Serializable]
    public class PartData
    {
        /// <summary>
        /// パートの管理ID
        /// </summary>
        public int PartId { get; set; }
        
        /// <summary>
        /// このパートに属するチャプターデータ
        /// </summary>
        public List<ChapterData> Chapters { get; set; }
    }
}