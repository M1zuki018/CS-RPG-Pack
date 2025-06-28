using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタン作成時の自動リネーム機能
/// </summary>
[InitializeOnLoad]
public class ButtonTextAutoRenamer
{
    static ButtonTextAutoRenamer()
    {
        // Hierarchyの変更を監視
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }
    
    private static void OnHierarchyChanged()
    {
        // 新しく作成されたボタンの検出と自動リネーム(パフォーマンス考慮のため、必要に応じて有効化する）
        // AutoRenameNewButtons();
    }
    
    /// <summary>
    /// 新しく作成されたボタンを自動でリネーム（オプション機能）
    /// </summary>
    private static void AutoRenameNewButtons()
    {
        Button[] buttons = Object.FindObjectsOfType<Button>();
        
        foreach (Button button in buttons)
        {
            Text[] childTexts = button.GetComponentsInChildren<Text>();
            foreach (Text text in childTexts)
            {
                if (text.gameObject.name == "Text" && text.transform.parent == button.transform)
                {
                    text.gameObject.name = $"{button.gameObject.name}_Text";
                }
            }
        }
    }
}