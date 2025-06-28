using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタンの子にあるTextオブジェクトの名前を自動リネームする拡張
/// </summary>
public class ButtonTextRenamer : EditorWindow
{
    private bool _includeInactiveObjects = true;
    private bool _showDebugLog = true;
    
    [MenuItem("Tools/Button Text Renamer")]
    public static void ShowWindow()
    {
        GetWindow<ButtonTextRenamer>("Button Text Renamer");
    }
    private void OnGUI()
    {
        GUILayout.Label("Button Text Renamer", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "このツールはシーン内のすべてのButtonコンポーネントを検索し、" +
            "その子オブジェクトにあるTextコンポーネントの名前を親ボタンのGameObject名に合わせてリネームします。",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        // オプション設定
        _includeInactiveObjects = EditorGUILayout.Toggle("非アクティブオブジェクトも含む", _includeInactiveObjects);
        _showDebugLog = EditorGUILayout.Toggle("ログを表示", _showDebugLog);
        
        GUILayout.Space(15);
        
        // 実行ボタン
        if (GUILayout.Button("シーン内の全ボタンテキストをリネーム", GUILayout.Height(30)))
        {
            RenameAllButtonTexts();
        }
        
        GUILayout.Space(10);
        
        // 選択されたオブジェクトのみ処理するボタン
        if (GUILayout.Button("選択されたボタンのテキストをリネーム", GUILayout.Height(25)))
        {
            RenameSelectedButtonTexts();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "使用方法:\n" +
            "1. 'シーン内の全ボタンテキストをリネーム'でシーン内すべてのボタンを処理\n" +
            "2. '選択されたボタンのテキストをリネーム'で選択したボタンのみを処理",
            MessageType.None
        );
    }
    
    /// <summary>
    /// シーン内のすべてのボタンのテキストをリネーム
    /// </summary>
    private void RenameAllButtonTexts()
    {
        Button[] buttons = FindObjectsOfType<Button>(_includeInactiveObjects);
        int renamedCount = 0;
        
        Undo.RecordObjects(GetAllTextComponents(buttons), "Rename Button Texts");
        
        foreach (Button button in buttons)
        {
            if (RenameButtonText(button))
            {
                renamedCount++;
            }
        }
        
        if (_showDebugLog)
        {
            Debug.Log($"ButtonTextRenamer: {renamedCount}個のボタンのテキストをリネームしました。（検索対象: {buttons.Length}個）");
        }
        
        EditorUtility.DisplayDialog("完了", $"{renamedCount}個のボタンのテキストをリネームしました。", "OK");
    }
    
    /// <summary>
    /// 選択されたボタンのテキストをリネーム
    /// </summary>
    private void RenameSelectedButtonTexts()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        int renamedCount = 0;
        
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("エラー", "ボタンオブジェクトを選択してください。", "OK");
            return;
        }
        
        Button[] selectedButtons = Array
            .FindAll(selectedObjects, obj => obj.GetComponent<Button>() != null)
            .Select(obj => obj.GetComponent<Button>())
            .ToArray();
        
        if (selectedButtons.Length == 0)
        {
            EditorUtility.DisplayDialog("エラー", "選択されたオブジェクトにButtonコンポーネントが見つかりません。", "OK");
            return;
        }
        
        Undo.RecordObjects(GetAllTextComponents(selectedButtons), "Rename Selected Button Texts");
        
        foreach (Button button in selectedButtons)
        {
            if (RenameButtonText(button))
            {
                renamedCount++;
            }
        }
        
        if (_showDebugLog)
        {
            Debug.Log($"ButtonTextRenamer: {renamedCount}個の選択されたボタンのテキストをリネームしました。");
        }
        
        EditorUtility.DisplayDialog("完了", $"{renamedCount}個のボタンのテキストをリネームしました。", "OK");
    }
    
    /// <summary>
    /// 指定されたボタンの子テキストオブジェクトをリネーム
    /// </summary>
    private bool RenameButtonText(Button button)
    {
        if (button == null) return false;
        
        // ボタンの子オブジェクトからTextコンポーネントを検索
        Text[] childTexts = button.GetComponentsInChildren<Text>(_includeInactiveObjects);
        bool renamed = false;
        
        foreach (Text text in childTexts)
        {
            // 直接の子オブジェクトまたは孫オブジェクト（1階層下まで）を対象
            if (text.transform.parent == button.transform || 
                text.transform.parent.parent == button.transform)
            {
                string newName = $"{button.gameObject.name}_Text";
                
                if (text.gameObject.name != newName)
                {
                    text.gameObject.name = newName;
                    renamed = true;
                    
                    if (_showDebugLog)
                    {
                        Debug.Log($"リネーム: {text.gameObject.name} → {newName}");
                    }
                }
            }
        }
        
        return renamed;
    }
    
    /// <summary>
    /// 指定されたボタン配列からすべてのTextコンポーネントのGameObjectを取得
    /// </summary>
    private GameObject[] GetAllTextComponents(Button[] buttons)
    {
        var textObjects = new List<GameObject>();
        
        foreach (Button button in buttons)
        {
            Text[] texts = button.GetComponentsInChildren<Text>(_includeInactiveObjects);
            foreach (Text text in texts)
            {
                textObjects.Add(text.gameObject);
            }
        }
        
        return textObjects.ToArray();
    }
}
