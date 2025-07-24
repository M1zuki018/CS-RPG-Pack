using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// StoryCharacterMasterの辞書を生成するウィンドウ
/// </summary>
public class StoryCharacterMasterGeneratorWindow : EditorWindow
{
    private string _spreadsheetName = "TestStory";
    private string _range = "StoryCharacterMaster!A3:L";
    private string _className = "StoryCharacterMaster";
    private string _outputPath = "Assets/iCON/Scripts/Generated/";
    
    [MenuItem("Tools/Story Character Master Generator")]
    public static void ShowWindow()
    {
        GetWindow<StoryCharacterMasterGeneratorWindow>("Story Character Master Generator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Story Character Master Generator", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        _spreadsheetName = EditorGUILayout.TextField("Spreadsheet Name", _spreadsheetName);
        _range = EditorGUILayout.TextField("Range", _range);
        _className = EditorGUILayout.TextField("Class Name", _className);
        _outputPath = EditorGUILayout.TextField("Output Path", _outputPath);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "想定するスプレッドシートの列構成:\n" +
            "A: ID, B: FullName, C: DisplayName, D: CharacterColor, E: TextSpeed\n" +
            "F: Default, G: Nervous, H: Sigh, I: Surprised, J: Smile, K: Blush, L: Embarrassed", 
            MessageType.Info);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("クラス生成"))
        {
            GenerateCharacterClass();
        }
    }
    
    private async void GenerateCharacterClass()
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
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using iCON.System;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// ストーリーキャラクター情報の定数クラス");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public static class {_className}");
        sb.AppendLine("{");
        
        // データ辞書の生成
        sb.AppendLine("    private static readonly Dictionary<int, CharacterData> _characterData = new Dictionary<int, CharacterData>");
        sb.AppendLine("    {");

        foreach (var row in data)
        {
            if (row.Count < 5) continue; // 最低限の列数チェック
            
            var id = int.Parse(row[0].ToString());
            var fullName = row[1].ToString();
            var displayName = row[2].ToString();
            var colorString = row[3].ToString();
            var textSpeed = float.Parse(row[4].ToString());
            
            // Color解析（#8B0000形式を想定）
            sb.AppendLine($"        {{");
            sb.AppendLine($"            {id}, new CharacterData({id}, \"{fullName}\", \"{displayName}\", ");
            
            if (ColorUtility.TryParseHtmlString(colorString, out Color color))
            {
                sb.AppendLine($"                new Color({color.r:F3}f, {color.g:F3}f, {color.b:F3}f, {color.a:F3}f), {textSpeed:F2}f,");
            }
            else
            {
                sb.AppendLine($"                Color.white, {textSpeed:F2}f,");
            }
            
            sb.AppendLine($"                new Dictionary<FacialExpressionType, string>");
            sb.AppendLine($"                {{");
            
            // 表情パスの追加（列インデックス5〜11）
            var expressions = new[]
            {
                "Default", "Nervous", "Sigh", "Surprised", "Smile", "Blush", "Embarrassed"
            };
            
            for (int i = 0; i < expressions.Length; i++)
            {
                var columnIndex = i + 5;
                if (row.Count > columnIndex && !string.IsNullOrEmpty(row[columnIndex].ToString()))
                {
                    sb.AppendLine($"                    {{ FacialExpressionType.{expressions[i]}, \"{row[columnIndex]}\" }},");
                }
            }
            
            sb.AppendLine($"                }})");
            sb.AppendLine($"        }},");
        }
        
        sb.AppendLine("    };");
        sb.AppendLine();
        
        // メソッド群の追加
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// IDからキャラクターデータを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static CharacterData GetCharacter(int id)");
        sb.AppendLine("    {");
        sb.AppendLine("        return _characterData.GetValueOrDefault(id, null);");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// フルネームからキャラクターデータを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static CharacterData GetCharacterByName(string fullName)");
        sb.AppendLine("    {");
        sb.AppendLine("        foreach (var kvp in _characterData)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (kvp.Value.FullName == fullName)");
        sb.AppendLine("                return kvp.Value;");
        sb.AppendLine("        }");
        sb.AppendLine("        return null;");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// キャラクターの表情パスを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static string GetExpressionPath(int characterId, FacialExpressionType expression)");
        sb.AppendLine("    {");
        sb.AppendLine("        var character = GetCharacter(characterId);");
        sb.AppendLine("        if (character?.ExpressionPaths?.ContainsKey(expression) == true)");
        sb.AppendLine("        {");
        sb.AppendLine("            return character.ExpressionPaths[expression];");
        sb.AppendLine("        }");
        sb.AppendLine("        return null;");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 全キャラクターのIDリストを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static IEnumerable<int> GetAllCharacterIds()");
        sb.AppendLine("    {");
        sb.AppendLine("        return _characterData.Keys;");
        sb.AppendLine("    }");
        sb.AppendLine();
        
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 全キャラクターデータを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static IEnumerable<CharacterData> GetAllCharacters()");
        sb.AppendLine("    {");
        sb.AppendLine("        return _characterData.Values;");
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