using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// スクリプトを作成補助用の静的クラス
/// </summary>
public static class ScriptCreator
{
    public static void CreateScript(string savePath, string scriptName, int templateIndex, 
        string[] templates, string templateFolderPath)
    {
        // フォルダパスが空でないかチェック
        if (string.IsNullOrEmpty(savePath))
        {
            Debug.LogError("フォルダパスが空です");
            return;
        }
        
        // テンプレートが選択されているかチェック
        if (templates == null || templates.Length == 0 || templateIndex >= templates.Length)
        {
            Debug.LogError("有効なテンプレートが選択されていません");
            return;
        }
        
        // 選択されたテンプレートを読み込む
        string selectedTemplate = templates[templateIndex];
        string templatePath = $"{templateFolderPath}/{selectedTemplate}.txt";

        if (!File.Exists(templatePath))
        {
            Debug.LogError($"テンプレートファイル {selectedTemplate}.txt が見つかりません");
            return;
        }
        
        // テンプレートファイルの内容を読み込む
        string templateContent = File.ReadAllText(templatePath);

        // テンプレート内容を解析してクラス名を取得
        // NOTE: プレフィックスやサフィックスを指定したテンプレートを作成した場合などに
        // クラス名とファイル名が異なってしまう場合が考えられるため、その対応をしています
        string actualClassName = ExtractClassNameFromTemplate(templateContent, scriptName);
        
        // スクリプト内容をファイルに書き込む（{ClassName}を置き換え）
        string scriptContent = templateContent.Replace("{ClassName}", scriptName);
        
        // ファイル名を決定（実際のクラス名を使用）
        string fileName = string.IsNullOrEmpty(actualClassName) ? scriptName : actualClassName;
        string path = Path.Combine(savePath, $"{fileName}.cs");
        
        // スクリプトが既に存在するか確認
        if (File.Exists(path))
        {
            Debug.LogError($"Script {fileName} は既に存在します！");
            return;
        }

        // スクリプトファイルを作成
        File.WriteAllText(path, scriptContent);
        AssetDatabase.Refresh();
        
        // 作成したスクリプトを選択してハイライト表示
        var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        if (scriptAsset != null)
        {
            Selection.activeObject = scriptAsset;
            EditorGUIUtility.PingObject(scriptAsset);
        }
        
        Debug.Log($"Script {fileName} created at {path}");
    }

    /// <summary>
    /// テンプレート内容からクラス名を抽出する
    /// {ClassName}を置き換えた後の実際のクラス名を取得
    /// </summary>
    private static string ExtractClassNameFromTemplate(string templateContent, string scriptName)
    {
        // {ClassName}をscriptNameに置き換えた後のテンプレート内容を作成
        string replacedContent = templateContent.Replace("{ClassName}", scriptName);
        
        // クラス名を抽出する正規表現パターン
        // public class, public static class, public abstract class などに対応
        string[] patterns = {
            @"public\s+(?:static\s+|abstract\s+)?class\s+(\w+)",
            @"public\s+(?:static\s+|abstract\s+)?interface\s+(\w+)",
            @"public\s+(?:static\s+|abstract\s+)?struct\s+(\w+)",
            @"public\s+enum\s+(\w+)"
        };
        
        foreach (string pattern in patterns)
        {
            Match match = Regex.Match(replacedContent, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }
        
        // クラス名が見つからない場合はnullを返す
        return null;
    }
}