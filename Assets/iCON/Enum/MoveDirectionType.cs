namespace iCON.Enums
{
    /// <summary>
    /// 移動方向の列挙型
    /// </summary>
    public enum MoveDirectionType
    {
        /// <summary>移動なし・停止状態</summary>
        None = 0,
        
        /// <summary>左方向への移動</summary>
        Left = 1,
        
        /// <summary>上方向への移動</summary>
        Up = 2,
        
        /// <summary>右方向への移動</summary>
        Right = 3,
        
        /// <summary>下方向への移動</summary>
        Down = 4,
        
        /// <summary>左上方向への移動</summary>
        LeftUp = 5,

        /// <summary>右上方向への移動</summary>
        RightUp = 6,

        /// <summary>左下方向への移動</summary>
        LeftDown = 7,

        /// <summary>右下方向への移動</summary>
        RightDown = 8,
    }
}
