namespace iCON.Enums
{
    /// <summary>
    /// フィールドインタラクションオブジェクトの振る舞いの列挙型
    /// </summary>
    public enum InteractionBehaviorType
    {
        /// <summary>一回で消える</summary>
        OneTime,
       
        /// <summary>回数で消える</summary>
        LimitedUse,
       
        /// <summary>消えない</summary>
        Permanent,
       
        /// <summary>時間経過で復活</summary>
        Respawnable,
       
        /// <summary>条件付きで復活</summary>
        Conditional,
       
        /// <summary>段階的変化</summary>
        Progressive,
       
        /// <summary>一時的無効化</summary>
        Temporary,
       
        /// <summary>プレイヤー状態依存</summary>
        PlayerDependent
    }
}