using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

/// <summary>
/// CustomButtonコンポーネントのInspectorの拡張
/// </summary>
[CustomEditor(typeof(CustomButton))]
public class CustomButtonEditor : ButtonEditor
{
    private SerializedProperty _assetNameProp;
    private SerializedProperty _wordingKeyProp;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _assetNameProp = serializedObject.FindProperty("_assetName");
        _wordingKeyProp = serializedObject.FindProperty("_wordingKey");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // カスタムフィールドを表示
        EditorGUILayout.PropertyField(_assetNameProp, new GUIContent("Asset Name"));
        EditorGUILayout.PropertyField(_wordingKeyProp, new GUIContent("Wording Key"));
        
        // 元のプロパティを表示
        base.OnInspectorGUI();
        
        serializedObject.ApplyModifiedProperties();
    }
}
