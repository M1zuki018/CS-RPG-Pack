using System;
using System.Collections.Generic;

namespace iCON.System
{
    /// <summary>
    /// シーンデータ
    /// </summary>
    [Serializable]
    public class SceneData
    {
        /// <summary>
        /// 自分が属しているチャプターのID
        /// </summary>
        public int ChapterID { get; set; }
        
        /// <summary>
        /// シーンの管理ID
        /// </summary>
        public int SceneNum { get; set; }
        
        /// <summary>
        /// このシーンに属しているオーダーデータ
        /// </summary>
        public List<OrderData> Orders { get; set; } = new List<OrderData>();
    }
}