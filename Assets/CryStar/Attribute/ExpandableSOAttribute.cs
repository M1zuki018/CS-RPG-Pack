using System;
using UnityEngine;

namespace CryStar.Attribute
{
    /// <summary>
    /// ScriptableObjectをInspector上で展開して編集できるようにする属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ExpandableSOAttribute : PropertyAttribute
    {
        /// <summary>
        /// 展開表示するかどうかの初期状態
        /// </summary>
        public bool DefaultExpanded { get; }

        /// <summary>
        /// 背景スタイルを使用するかどうか
        /// </summary>
        public bool UseBackground { get; }

        public ExpandableSOAttribute(bool defaultExpanded = true, bool useBackground = true)
        {
            DefaultExpanded = defaultExpanded;
            UseBackground = useBackground;
        }
    }
}