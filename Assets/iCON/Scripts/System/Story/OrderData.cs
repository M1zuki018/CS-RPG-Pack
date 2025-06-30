using System;
using iCON.Enums;

namespace iCON.System
{
    /// <summary>
    /// ストーリーのオーダーデータ
    /// </summary>
    [Serializable]
    public class OrderData
    {
        /// <summary>属しているパートの管理ID</summary>
        public int PartId { get; set; }
        
        /// <summary>属しているチャプターの管理ID</summary>
        public int ChapterID { get; set; }
        
        /// <summary>属しているシーンの管理ID</summary>
        public int SceneID { get; set; }
        
        /// <summary>オーダーの管理ID</summary>
        public int OrderID { get; set; }
        
        /// <summary>オーダーの種類</summary>
        public OrderType OrderType { get; set; }
        
        /// <summary>前のオーダーとの連結方法</summary>
        public int Sequence { get; set; }
        
        /// <summary>話し手のキャラクターの管理ID</summary>
        public string SpeakerId { get; set; }
        
        /// <summary>テキスト</summary>
        public string DialogText { get; set; }
        
        /// <summary>表示名を上書きする場合の名前</summary>
        public string OverrideDisplayName { get; set; }
        
        /// <summary>素材のファイルパス</summary>
        public string FilePath { get; set; }
        
        /// <summary>画面上の立ち位置</summary>
        public CharacterPositionType Position { get; set; }
        
        /// <summary>使用する表情差分</summary>
        public FacialExpressionType FacialExpressionType { get; set; }
        
        /// <summary>テキスト表示速度を上書きする場合の値</summary>
        public float OverrideTextSpeed { get; set; }
        
        /// <summary>オーダーにかける時間</summary>
        public float Duration { get; set; }

        /// <summary>表示名を取得（オーバーライドがあればそれを、なければSpeakerIdを使用）</summary>
        public string DisplayName => !string.IsNullOrEmpty(OverrideDisplayName) ? OverrideDisplayName : SpeakerId;
    }
}