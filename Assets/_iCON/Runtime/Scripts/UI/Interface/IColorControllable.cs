using UnityEngine;

namespace CryStar.UI
{
    /// <summary>
    /// カラー制御が可能なUIContentsが継承すべきインターフェース
    /// </summary>
    public interface IColorControllable
    {
        /// <summary>
        /// 色
        /// </summary>
        Color Color { get; set; }
        
        /// <summary>
        /// カラーとアルファ値を同時に設定
        /// </summary>
        void SetColorWithAlpha(Color color, float alpha);
        
        /// <summary>
        /// アルファを保持してカラーのみ変更する
        /// </summary>
        void SetColorKeepAlpha(Color color);
    }
}