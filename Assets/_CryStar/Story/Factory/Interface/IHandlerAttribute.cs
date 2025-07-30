using System;

namespace CryStar.Story.Factory
{
    /// <summary>
    /// ハンドラーの属性情報を提供するインターフェース
    /// </summary>
    /// <typeparam name="TEnum">ハンドラータイプを表すEnum型</typeparam>
    public interface IHandlerAttribute<TEnum> where TEnum : struct, Enum
    {
        /// <summary>
        /// ハンドラーのタイプ
        /// </summary>
        TEnum HandlerType { get; }
    }
}