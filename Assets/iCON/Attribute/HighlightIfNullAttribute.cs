using System;
using UnityEngine;

namespace CryStar.Attribute
{
    /// <summary>
    /// 参照が割り当てられていないフィールドをハイライトする属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HighlightIfNullAttribute : PropertyAttribute { }
}