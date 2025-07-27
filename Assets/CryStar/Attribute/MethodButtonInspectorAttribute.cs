using UnityEngine;
using System;

namespace CryStar.Attribute
{
    /// <summary>
    /// インスペクターにボタンを表示しメソッドを実行できるようにする属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class MethodButtonInspectorAttribute : PropertyAttribute
    {
        public string Label { get; }

        public MethodButtonInspectorAttribute(string label = null)
        {
            Label = label;
        }
    }
}