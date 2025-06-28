using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

/// <summary>
/// CustomImageコンポーネントのInspectorの拡張
/// </summary>
[CustomEditor(typeof(CustomImage))]
public class CustomImageEditor : ImageEditor
{
    private SerializedProperty _assetNameProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        _assetNameProp = serializedObject.FindProperty("_assetName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // カスタムフィールドを表示
        EditorGUILayout.PropertyField(_assetNameProp, new GUIContent("Asset Name"));
        
        // 元のプロパティを表示
        base.OnInspectorGUI();
        
        serializedObject.ApplyModifiedProperties();
    }
}
