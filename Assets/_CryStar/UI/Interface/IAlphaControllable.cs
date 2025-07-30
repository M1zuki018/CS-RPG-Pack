namespace CryStar.UI
{
    /// <summary>
    /// アルファ値制御があるUIContentsが継承すべきインターフェース
    /// </summary>
    public interface IAlphaControllable
    {
        /// <summary>
        /// 透明度
        /// </summary>
        float Alpha { get; }
        
        /// <summary>
        /// アルファ値を設定する
        /// </summary>
        void SetAlpha(float alpha);
    }
}