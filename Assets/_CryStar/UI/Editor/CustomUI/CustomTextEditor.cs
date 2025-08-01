using UnityEditor;
using UnityEngine;
using TextEditor = UnityEditor.UI.TextEditor;

/// <summary>
/// CustomTextコンポーネントのInspectorの拡張
/// </summary>
[CustomEditor(typeof(CustomText))]
public class CustomTextEditor : TextEditor
{
    private SerializedProperty _wordingKeyProp;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _wordingKeyProp = serializedObject.FindProperty("_wordingKey");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // カスタムフィールドを表示
        EditorGUILayout.PropertyField(_wordingKeyProp, new GUIContent("Wording Key"));
        
        serializedObject.ApplyModifiedProperties();
        
        // 元のプロパティを表示
        base.OnInspectorGUI();
    }
}
