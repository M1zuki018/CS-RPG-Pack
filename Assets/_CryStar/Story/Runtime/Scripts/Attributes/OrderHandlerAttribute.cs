using System;
using CryStar.Story.Enums;
using CryStar.Story.Factory;

namespace CryStar.Story.Attributes
{
    /// <summary>
    /// ハンドラーを自動登録するための属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OrderHandlerAttribute : System.Attribute, IHandlerAttribute<OrderType>
    {
        /// <summary>
        /// オーダーの種類
        /// </summary>
        public OrderType HandlerType { get; }
        
        /// <summary>
        /// ハンドラーの優先度（低い値ほど優先される デフォルト: 0）
        /// </summary>
        public int Priority { get; set; } = 0;
        
        /// <summary>
        /// このハンドラーが有効かどうか（デフォルト: true）
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        
        /// <summary>
        /// オーダーハンドラー属性の初期化
        /// </summary>
        public OrderHandlerAttribute(OrderType handlerType)
        {
            HandlerType = handlerType;
        }
    }
}

