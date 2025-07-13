using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// TextとCustomTextの一括置換ツール
/// </summary>
public class TextReplacementWindow : EditorWindow
{
    private Vector2 _scrollPosition;
    private List<Text> _textComponents = new List<Text>();
    private List<CustomText> _customTextComponents = new List<CustomText>();
    private bool _showTextComponents = true;
    private bool _showCustomTextComponents = true;
    
    [MenuItem("Tools/Text Replacement Tool")]
    public static void ShowWindow()
    {
        GetWindow<TextReplacementWindow>("Text Replacement Tool");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Text Component Replacement Tool", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        // 検索ボタン
        if (GUILayout.Button("Find All Text Components in Scene"))
        {
            FindAllTextComponents();
        }
        
        EditorGUILayout.Space();
        
        // 統計情報
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"Text Components: {_textComponents.Count}");
        GUILayout.Label($"CustomText Components: {_customTextComponents.Count}");
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 一括置換ボタン
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Replace All Text → CustomText"))
        {
            ReplaceAllTextWithCustomText();
        }
        if (GUILayout.Button("Replace All CustomText → Text"))
        {
            ReplaceAllCustomTextWithText();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // リスト表示
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        
        // Text Components
        _showTextComponents = EditorGUILayout.Foldout(_showTextComponents, "Text Components");
        if (_showTextComponents)
        {
            EditorGUI.indentLevel++;
            foreach (var textComponent in _textComponents)
            {
                if (textComponent == null) continue;
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(textComponent, typeof(Text), true);
                if (GUILayout.Button("Replace", GUILayout.Width(80)))
                {
                    ReplaceTextWithCustomText(textComponent);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
        
        // CustomText Components
        _showCustomTextComponents = EditorGUILayout.Foldout(_showCustomTextComponents, "CustomText Components");
        if (_showCustomTextComponents)
        {
            EditorGUI.indentLevel++;
            foreach (var customTextComponent in _customTextComponents)
            {
                if (customTextComponent == null) continue;
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(customTextComponent, typeof(CustomText), true);
                if (GUILayout.Button("Replace", GUILayout.Width(80)))
                {
                    ReplaceCustomTextWithText(customTextComponent);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void FindAllTextComponents()
    {
        _textComponents.Clear();
        _customTextComponents.Clear();
        
        // シーン内のすべてのTextコンポーネントを検索
        _textComponents.AddRange(FindObjectsOfType<Text>().Where(t => !(t is CustomText)));
        _customTextComponents.AddRange(FindObjectsOfType<CustomText>());
        
        // nullチェック
        _textComponents.RemoveAll(t => t == null);
        _customTextComponents.RemoveAll(t => t == null);
        
        Debug.Log($"Found {_textComponents.Count} Text components and {_customTextComponents.Count} CustomText components");
    }
    
    private void ReplaceAllTextWithCustomText()
    {
        if (_textComponents.Count == 0)
        {
            Debug.LogWarning("No Text components found to replace");
            return;
        }
        
        Undo.RegisterCompleteObjectUndo(_textComponents.Select(t => t.gameObject).ToArray(), "Replace All Text with CustomText");
        
        foreach (var textComponent in _textComponents.ToArray())
        {
            if (textComponent != null)
            {
                ReplaceTextWithCustomText(textComponent);
            }
        }
        
        FindAllTextComponents(); // リストを更新
        Debug.Log($"Replaced {_textComponents.Count} Text components with CustomText");
    }
    
    private void ReplaceAllCustomTextWithText()
    {
        if (_customTextComponents.Count == 0)
        {
            Debug.LogWarning("No CustomText components found to replace");
            return;
        }
        
        Undo.RegisterCompleteObjectUndo(_customTextComponents.Select(t => t.gameObject).ToArray(), "Replace All CustomText with Text");
        
        foreach (var customTextComponent in _customTextComponents.ToArray())
        {
            if (customTextComponent != null)
            {
                ReplaceCustomTextWithText(customTextComponent);
            }
        }
        
        FindAllTextComponents(); // リストを更新
        Debug.Log($"Replaced {_customTextComponents.Count} CustomText components with Text");
    }
    
    private void ReplaceTextWithCustomText(Text originalText)
    {
        if (originalText == null) return;
        
        GameObject gameObject = originalText.gameObject;
        
        // 設定を保存
        var settings = CopyTextSettings(originalText);
        
        // コンポーネントを置換
        DestroyImmediate(originalText);
        CustomText customText = gameObject.AddComponent<CustomText>();
        
        // 設定を復元
        ApplyTextSettings(customText, settings);
        
        EditorUtility.SetDirty(gameObject);
    }
    
    private void ReplaceCustomTextWithText(CustomText originalCustomText)
    {
        if (originalCustomText == null) return;
        
        GameObject gameObject = originalCustomText.gameObject;
        
        // 設定を保存
        var settings = CopyTextSettings(originalCustomText);
        
        // コンポーネントを置換
        DestroyImmediate(originalCustomText);
        Text newText = gameObject.AddComponent<Text>();
        
        // 設定を復元
        ApplyTextSettings(newText, settings);
        
        EditorUtility.SetDirty(gameObject);
    }
    
    private TextSettings CopyTextSettings(Text text)
    {
        return new TextSettings
        {
            text = text.text,
            color = text.color,
            font = text.font,
            fontSize = text.fontSize,
            fontStyle = text.fontStyle,
            lineSpacing = text.lineSpacing,
            supportRichText = text.supportRichText,
            alignment = text.alignment,
            alignByGeometry = text.alignByGeometry,
            horizontalOverflow = text.horizontalOverflow,
            verticalOverflow = text.verticalOverflow,
            resizeTextForBestFit = text.resizeTextForBestFit,
            resizeTextMinSize = text.resizeTextMinSize,
            resizeTextMaxSize = text.resizeTextMaxSize,
            material = text.material,
            raycastTarget = text.raycastTarget
        };
    }
    
    private void ApplyTextSettings(Text text, TextSettings settings)
    {
        text.text = settings.text;
        text.color = settings.color;
        text.font = settings.font;
        text.fontSize = settings.fontSize;
        text.fontStyle = settings.fontStyle;
        text.lineSpacing = settings.lineSpacing;
        text.supportRichText = settings.supportRichText;
        text.alignment = settings.alignment;
        text.alignByGeometry = settings.alignByGeometry;
        text.horizontalOverflow = settings.horizontalOverflow;
        text.verticalOverflow = settings.verticalOverflow;
        text.resizeTextForBestFit = settings.resizeTextForBestFit;
        text.resizeTextMinSize = settings.resizeTextMinSize;
        text.resizeTextMaxSize = settings.resizeTextMaxSize;
        text.material = settings.material;
        text.raycastTarget = settings.raycastTarget;
    }
    
    private class TextSettings
    {
        public string text;
        public Color color;
        public Font font;
        public int fontSize;
        public FontStyle fontStyle;
        public float lineSpacing;
        public bool supportRichText;
        public TextAnchor alignment;
        public bool alignByGeometry;
        public HorizontalWrapMode horizontalOverflow;
        public VerticalWrapMode verticalOverflow;
        public bool resizeTextForBestFit;
        public int resizeTextMinSize;
        public int resizeTextMaxSize;
        public Material material;
        public bool raycastTarget;
    }
}