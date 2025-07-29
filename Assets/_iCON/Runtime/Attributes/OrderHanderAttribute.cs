using System;
using CryStar.Story.Enums;

namespace iCON.System
{
    /// <summary>
    /// ハンドラーを自動登録するための属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OrderHandlerAttribute : Attribute
    {
        /// <summary>
        /// オーダーの種類
        /// </summary>
        public OrderType OrderType { get; }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderHandlerAttribute(OrderType orderType)
        {
            OrderType = orderType;
        }
    }
}

