using System;
using System.Collections.Generic;
using System.IO;
using CryStar.Network;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Masterデータを取得するウィンドウのベースクラス
/// </summary>
public abstract class BaseMasterGeneratorWindow : EditorWindow
{
    protected string _spreadsheetName = "TestStory";
    protected string _range = "Masterxx!A3:L";
    protected string _className = "Masterxx";
    protected string _outputPath = "Assets/_iCON/Runtime/Scripts/Generated/";
    
    private void OnGUI()
    {
        GUILayout.Label(GetWindowTitle(), EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        _spreadsheetName = EditorGUILayout.TextField("Spreadsheet Name", _spreadsheetName);
        _range = EditorGUILayout.TextField("Range", _range);
        _className = EditorGUILayout.TextField("Class Name", _className);
        _outputPath = EditorGUILayout.TextField("Output Path", _outputPath);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("クラス生成"))
        {
            GenerateClass();
        }
    }
    
    /// <summary>
    /// ウィンドウ名
    /// </summary>
    protected abstract string GetWindowTitle();

    /// <summary>
    /// クラス生成用のメソッド
    /// </summary>
    private async void GenerateClass()
    {
        if (string.IsNullOrEmpty(_spreadsheetName) || string.IsNullOrEmpty(_range))
        {
            EditorUtility.DisplayDialog("エラー", "スプレッドシート名と範囲を入力してください", "OK");
            return;
        }
        
        try
        {
            // 読み込み
            var data = await SheetsDataService.Instance.ReadFromSpreadsheetAsync(_spreadsheetName, _range);
            
            // クラス生成
            GenerateClass(data);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("エラー", $"クラス生成に失敗しました: {e.Message}", "OK");
        }
    }

    protected abstract void GenerateClass(IList<IList<object>> data);
    
    protected void SaveToFile(string content)
    {
        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }
        
        string filePath = Path.Combine(_outputPath, $"{_className}.cs");
        File.WriteAllText(filePath, content);
        
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("完了", $"クラス生成完了: {filePath}", "OK");
    }
}
