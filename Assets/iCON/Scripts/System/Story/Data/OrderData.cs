using System;
using iCON.Enums;

namespace iCON.System
{
    /// <summary>
    /// オーダーデータ
    /// </summary>
    [Serializable]
    public class OrderData
    {
        /// <summary>属しているパートの管理ID</summary>
        public int PartId { get; set; }
        
        /// <summary>属しているチャプターの管理ID</summary>
        public int ChapterId { get; set; }
        
        /// <summary>属しているシーンの管理ID</summary>
        public int SceneId { get; set; }
        
        /// <summary>オーダーの管理ID</summary>
        public int OrderId { get; set; }
        
        /// <summary>オーダーの種類</summary>
        public OrderType OrderType { get; set; }
        
        /// <summary>前のオーダーとの連結方法</summary>
        public int Sequence { get; set; }
        
        /// <summary>話し手のキャラクターの管理ID</summary>
        public string SpeakerId { get; set; }
        
        /// <summary>テキスト</summary>
        public string DialogText { get; set; }
        
        /// <summary>OverrideDisplayName</summary>
        public string OverrideDisplayName { get; set; }
        
        /// <summary>FilePath</summary>
        public string FilePath { get; set; }
        
        /// <summary>CharacterPositionType</summary>
        public CharacterPositionType Position { get; set; }
        
        /// <summary>使用する表情差分</summary>
        public FacialExpressionType FacialExpressionType { get; set; }
        
        /// <summary>OverrideTextSpeed</summary>
        public float OverrideTextSpeed { get; set; }
        
        /// <summary>Duration</summary>
        public float Duration { get; set; }

        /// <summary>表示名を取得（オーバーライドがあればそれを、なければSpeakerIdを使用）</summary>
        public string DisplayName => !string.IsNullOrEmpty(OverrideDisplayName) ? OverrideDisplayName : SpeakerId;
    }
}