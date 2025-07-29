using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// StorySceneMasterの辞書を生成するウィンドウ
/// </summary>
public class StorySceneMasterGeneratorWindow : BaseMasterGeneratorWindow
{
    [MenuItem("Tools/Story Scene Master Generator")]
    public static void ShowWindow()
    {
        GetWindow<StorySceneMasterGeneratorWindow>("Story Scene Master Generator");
    }

    protected override string GetWindowTitle() => "Story Scene Master Generator";

    /// <summary>
    /// StorySceneMasterの辞書を生成するGenerateClassメソッド
    /// </summary>
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
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using CryStar.Story.Data;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// ストーリーシーン情報の定数クラス");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public static class {_className}");
        sb.AppendLine("{");

        // データ辞書の生成
        sb.AppendLine(
            "    private static readonly Dictionary<int, StorySceneData> _sceneData = new Dictionary<int, StorySceneData>");
        sb.AppendLine("    {");

        foreach (var row in data)
        {
            if (row.Count < 8) continue; // 最低限の列数チェック

            var id = int.Parse(row[0].ToString());
            var sceneName = row[1].ToString();
            var partId = int.Parse(row[2].ToString());
            var chapterId = int.Parse(row[3].ToString());
            var sceneId = int.Parse(row[4].ToString());
            var range = row[5].ToString();
            var characterScale = float.Parse(row[6].ToString());
            var positionCorrection = row[7].ToString();
            var prerequisiteStory = row.Count > 8 && !string.IsNullOrEmpty(row[8].ToString())
                ? int.Parse(row[8].ToString())
                : (int?)null;

            // 位置補正量をVector3に変換（例: "180-60-0" -> Vector3(180, 60, 0)）
            Vector3 positionVector = ParsePositionCorrection(positionCorrection);

            sb.AppendLine("        {");
            sb.Append($"            {id}, new StorySceneData(");
            sb.Append($"{id}, ");
            sb.Append($"\"{sceneName}\", ");
            sb.Append($"{partId}, ");
            sb.Append($"{chapterId}, ");
            sb.Append($"{sceneId}, ");
            sb.AppendLine();
            sb.Append($"                \"{range}\", ");
            sb.Append($"{characterScale:F1}f, ");
            sb.Append($"new Vector3({positionVector.x:F1}f, {positionVector.y:F1}f, {positionVector.z:F1}f)");

            if (prerequisiteStory.HasValue)
            {
                sb.Append($", {prerequisiteStory.Value}");
            }
            else
            {
                sb.Append($", null");
            }

            sb.AppendLine();
            sb.AppendLine("            )");
            sb.AppendLine("        },");
        }

        sb.AppendLine("    };");
        sb.AppendLine();

        // メソッド群の追加
        AppendStorySceneMethods(sb);

        sb.AppendLine("}");
        sb.AppendLine();

        // ファイル出力
        SaveToFile(sb.ToString());
    }

    /// <summary>
    /// 位置補正量文字列をVector3に変換
    /// </summary>
    private Vector3 ParsePositionCorrection(string positionString)
    {
        if (string.IsNullOrEmpty(positionString))
            return Vector3.zero;

        var parts = positionString.Split('-');

        float x = parts.Length > 0 && float.TryParse(parts[0], out var xVal) ? xVal : 0f;
        float y = parts.Length > 1 && float.TryParse(parts[1], out var yVal) ? yVal : 0f;
        float z = parts.Length > 2 && float.TryParse(parts[2], out var zVal) ? zVal : 0f;

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// StorySceneMasterのメソッド群を追加
    /// </summary>
    private void AppendStorySceneMethods(StringBuilder sb)
    {
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// IDからストーリーシーンデータを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static StorySceneData GetSceneById(int id)");
        sb.AppendLine("    {");
        sb.AppendLine("        return _sceneData.GetValueOrDefault(id, null);");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// シーン名からストーリーシーンデータを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static StorySceneData GetSceneByName(string sceneName)");
        sb.AppendLine("    {");
        sb.AppendLine("        return _sceneData.Values.FirstOrDefault(scene => scene.SceneName == sceneName);");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// チャプターIDとシーンIDからストーリーシーンデータを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static StorySceneData GetSceneById(int chapterId, int sceneId)");
        sb.AppendLine("    {");
        sb.AppendLine("        return _sceneData.Values.FirstOrDefault(scene => ");
        sb.AppendLine("            scene.ChapterId == chapterId && scene.SceneId == sceneId);");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 指定チャプターの全シーンを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static IEnumerable<StorySceneData> GetScenesByChapter(int chapterId)");
        sb.AppendLine("    {");
        sb.AppendLine("        return _sceneData.Values.Where(scene => scene.ChapterId == chapterId)");
        sb.AppendLine("                                .OrderBy(scene => scene.SceneId);");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 指定パートの全シーンを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static IEnumerable<StorySceneData> GetScenesByPart(int partId)");
        sb.AppendLine("    {");
        sb.AppendLine("        return _sceneData.Values.Where(scene => scene.PartId == partId)");
        sb.AppendLine("                                .OrderBy(scene => scene.ChapterId)");
        sb.AppendLine("                                .ThenBy(scene => scene.SceneId);");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 前提ストーリーが指定されているシーンを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine(
            "    public static IEnumerable<StorySceneData> GetScenesWithPrerequisite(int prerequisiteStoryId)");
        sb.AppendLine("    {");
        sb.AppendLine(
            "        return _sceneData.Values.Where(scene => scene.PrerequisiteStoryId == prerequisiteStoryId);");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 全ストーリーシーンのIDリストを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static IEnumerable<int> GetAllSceneIds()");
        sb.AppendLine("    {");
        sb.AppendLine("        return _sceneData.Keys;");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 全ストーリーシーンデータを取得");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static IEnumerable<StorySceneData> GetAllScenes()");
        sb.AppendLine("    {");
        sb.AppendLine("        return _sceneData.Values.OrderBy(scene => scene.Id);");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// シーンが実行可能かチェック（前提ストーリーの条件確認）");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static bool CanExecuteScene(int sceneId, HashSet<int> completedStories)");
        sb.AppendLine("    {");
        sb.AppendLine("        var scene = GetSceneById(sceneId);");
        sb.AppendLine("        if (scene == null) return false;");
        sb.AppendLine();
        sb.AppendLine("        // 前提ストーリーが指定されていない場合は実行可能");
        sb.AppendLine("        if (!scene.PrerequisiteStoryId.HasValue) return true;");
        sb.AppendLine();
        sb.AppendLine("        // 前提ストーリーが完了している場合は実行可能");
        sb.AppendLine("        return completedStories.Contains(scene.PrerequisiteStoryId.Value);");
        sb.AppendLine("    }");
    }
}