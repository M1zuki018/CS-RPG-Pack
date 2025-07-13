using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Wordingの辞書を生成するウィンドウ
/// </summary>
public class WordingMasterGeneratorWindow : EditorWindow
{
    private string _spreadsheetName = "";
    private string _range = "";
    private string _className = "WordingMaster";
    private string _outputPath = "Assets/iCON/Scripts/Generated/";
    
    [MenuItem("Tools/Wording Master Generator")]
    public static void ShowWindow()
    {
        GetWindow<WordingMasterGeneratorWindow>("Wording Master Generator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Wording Master Generator", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        _spreadsheetName = EditorGUILayout.TextField("Spreadsheet Name", _spreadsheetName);
        _range = EditorGUILayout.TextField("Range", _range);
        _className = EditorGUILayout.TextField("Class Name", _className);
        _outputPath = EditorGUILayout.TextField("Output Path", _outputPath);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("クラス生成"))
        {
            GenerateWordingClass();
        }
    }
    
    private async void GenerateWordingClass()
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
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("エラー", $"クラス生成に失敗しました: {e.Message}", "OK");
        }
    }
    
    private void GenerateClass(IList<IList<object>> data)
    {
        var sb = new StringBuilder();
        
        // クラス定義の開始
        sb.AppendLine("// ============================================================================");
        sb.AppendLine("// AUTO GENERATED - DO NOT MODIFY");
        sb.AppendLine($"// Generated at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("// ============================================================================");
        sb.AppendLine();
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// ワーディングキー定数クラス");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public static class {_className}");
        sb.AppendLine("{");
        sb.AppendLine("    private static readonly Dictionary<string, string> _data = new Dictionary<string, string>");
        sb.AppendLine("    {");

        foreach (var row in data)
        {
            var key = row[0].ToString();
            var comment = row.Count > 1 && !string.IsNullOrEmpty(row[1].ToString()) ? row[1].ToString() : key.ToString();
            sb.AppendLine($"        {{ \"{key}\", \"{comment}\" }},");
        }
        
        sb.AppendLine("    };");
        sb.AppendLine();
        sb.AppendLine("    public static string GetText(string key)");
        sb.AppendLine("    {");
        sb.AppendLine("        return _data.GetValueOrDefault(key, key);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        // ファイル出力
        SaveToFile(sb.ToString());
    }
    
    private void SaveToFile(string content)
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