using UnityEngine;
using System;

/// <summary>
/// 変数名をコメントに書き換えるカスタム属性
/// NOTE: この属性はMonoBehaviourやScriptableObjectの直接のフィールドにのみ適用されます
/// [Serializable]クラス内のフィールドには適用されません
/// 使用例: [SerializeField, Comment("プレイヤーの体力")] private int _health;
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class CommentAttribute : PropertyAttribute
{
    public string Text { get; }

    public CommentAttribute(string text)
    {
        Text = text;
    }
}