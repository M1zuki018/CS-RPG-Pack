using System;
using iCON.Enums;

namespace iCON.System
{
    /// <summary>
    /// エフェクトのハンドラーを自動登録するための属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EffectOrderHandlerAttribute : Attribute
    {
        /// <summary>
        /// エフェクトオーダーの種類
        /// </summary>
        public EffectOrderType EffectType { get; }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectOrderHandlerAttribute(EffectOrderType effectType)
        {
            EffectType = effectType;
        }
    }
}

