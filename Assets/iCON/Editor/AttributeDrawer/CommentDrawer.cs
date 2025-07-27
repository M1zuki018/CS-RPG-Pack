#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// 変数名の代わりに任意のコメントを表示する
/// </summary>
[CustomPropertyDrawer(typeof(CommentAttribute))]
public class CommentDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // CommentAttributeを取得
        CommentAttribute commentAttribute = (CommentAttribute)attribute;
        
        EditorGUI.BeginProperty(position, label, property);
        
        // カスタムラベルでGUIContentを作成
        GUIContent customLabel = new GUIContent(commentAttribute.Text, label.tooltip);
        
        // 描画を行う
        EditorGUI.PropertyField(position, property, customLabel, true);
        
        EditorGUI.EndProperty();
    }
    
    /// <summary>
    /// プロパティの高さを取得
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif