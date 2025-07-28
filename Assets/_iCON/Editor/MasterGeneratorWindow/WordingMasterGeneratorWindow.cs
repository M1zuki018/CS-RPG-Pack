using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

/// <summary>
/// Wordingの辞書を生成するウィンドウ
/// </summary>
public class WordingMasterGeneratorWindow : BaseMasterGeneratorWindow
{
    [MenuItem("Tools/Wording Master Generator")]
    public static void ShowWindow()
    {
        GetWindow<WordingMasterGeneratorWindow>("Wording Master Generator");
    }
    
    protected override string GetWindowTitle() => "Wording Master Generator";
    
    protected override void GenerateClass(IList<IList<object>> data)
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
        sb.AppendLine("        return _data.GetValueOrDefault(key, null);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        
        // ファイル出力
        SaveToFile(sb.ToString());
    }
}