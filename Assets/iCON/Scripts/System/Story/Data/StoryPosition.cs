using System;

namespace iCON.System
{   
    /// <summary>
    /// ストーリー内の位置を表すデータクラス
    /// </summary>
    public class StoryPosition
    {
        /// <summary>パートの管理ID</summary>
        public int PartId { get; }
        
        /// <summary>チャプターの管理ID</summary>
        public int ChapterId { get; }
        
        /// <summary>シーンの管理ID</summary>
        public int SceneId { get; }
        
        /// <summary>現在のオーダーのインデックス</summary>
        public int OrderIndex { get; }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StoryPosition(int partId, int chapterId, int sceneId, int orderIndex)
        {
            PartId = partId;
            ChapterId = chapterId;
            SceneId = sceneId;
            OrderIndex = orderIndex;
        }
        
        /// <summary>
        /// 複製
        /// </summary>
        public StoryPosition Clone()
        {
            return new StoryPosition(PartId, ChapterId, SceneId, OrderIndex);
        }
        
        /// <summary>
        /// 文字列に変換
        /// </summary>
        public override string ToString()
        {
            return $"Part:{PartId}, Chapter:{ChapterId}, Scene:{SceneId}, Order:{OrderIndex}";
        }
        
        /// <summary>
        /// 同値か判定
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is StoryPosition other)
            {
                return PartId == other.PartId && 
                       ChapterId == other.ChapterId && 
                       SceneId == other.SceneId && 
                       OrderIndex == other.OrderIndex;
            }
            return false;
        }
        
        /// <summary>
        /// HashCodeを生成
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(PartId, ChapterId, SceneId, OrderIndex);
        }
    }
}