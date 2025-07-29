// ============================================================================
// AUTO GENERATED - DO NOT MODIFY
// Generated at: 2025-07-30 01:35:31
// ============================================================================

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CryStar.Story.Data;

/// <summary>
/// ストーリーシーン情報の定数クラス
/// </summary>
public static class MasterStoryScene
{
    private static readonly Dictionary<int, StorySceneData> _sceneData = new Dictionary<int, StorySceneData>
    {
        {
            1, new StorySceneData(1, "琴葉の休息", 1, 1, 1, 
                "A3:O218", 1.0f, new Vector3(0.0f, 0.0f, 0.0f), null
            )
        },
        {
            2, new StorySceneData(2, "マキ・幼少期 - 1", 2, 1, 1, 
                "A220:O298", 1.2f, new Vector3(180.0f, 60.0f, 0.0f), null
            )
        },
        {
            3, new StorySceneData(3, "マキ・幼少期 - 2", 2, 1, 2, 
                "A300:O359", 1.2f, new Vector3(180.0f, 60.0f, 0.0f), 2
            )
        },
    };

    /// <summary>
    /// IDからストーリーシーンデータを取得
    /// </summary>
    public static StorySceneData GetSceneById(int id)
    {
        return _sceneData.GetValueOrDefault(id, null);
    }

    /// <summary>
    /// シーン名からストーリーシーンデータを取得
    /// </summary>
    public static StorySceneData GetSceneByName(string sceneName)
    {
        return _sceneData.Values.FirstOrDefault(scene => scene.SceneName == sceneName);
    }

    /// <summary>
    /// チャプターIDとシーンIDからストーリーシーンデータを取得
    /// </summary>
    public static StorySceneData GetSceneById(int chapterId, int sceneId)
    {
        return _sceneData.Values.FirstOrDefault(scene => 
            scene.ChapterId == chapterId && scene.SceneId == sceneId);
    }

    /// <summary>
    /// 指定チャプターの全シーンを取得
    /// </summary>
    public static IEnumerable<StorySceneData> GetScenesByChapter(int chapterId)
    {
        return _sceneData.Values.Where(scene => scene.ChapterId == chapterId)
                                .OrderBy(scene => scene.SceneId);
    }

    /// <summary>
    /// 指定パートの全シーンを取得
    /// </summary>
    public static IEnumerable<StorySceneData> GetScenesByPart(int partId)
    {
        return _sceneData.Values.Where(scene => scene.PartId == partId)
                                .OrderBy(scene => scene.ChapterId)
                                .ThenBy(scene => scene.SceneId);
    }

    /// <summary>
    /// 前提ストーリーが指定されているシーンを取得
    /// </summary>
    public static IEnumerable<StorySceneData> GetScenesWithPrerequisite(int prerequisiteStoryId)
    {
        return _sceneData.Values.Where(scene => scene.PrerequisiteStoryId == prerequisiteStoryId);
    }

    /// <summary>
    /// 全ストーリーシーンのIDリストを取得
    /// </summary>
    public static IEnumerable<int> GetAllSceneIds()
    {
        return _sceneData.Keys;
    }

    /// <summary>
    /// 全ストーリーシーンデータを取得
    /// </summary>
    public static IEnumerable<StorySceneData> GetAllScenes()
    {
        return _sceneData.Values.OrderBy(scene => scene.Id);
    }

    /// <summary>
    /// シーンが実行可能かチェック（前提ストーリーの条件確認）
    /// </summary>
    public static bool CanExecuteScene(int sceneId, HashSet<int> completedStories)
    {
        var scene = GetSceneById(sceneId);
        if (scene == null) return false;

        // 前提ストーリーが指定されていない場合は実行可能
        if (!scene.PrerequisiteStoryId.HasValue) return true;

        // 前提ストーリーが完了している場合は実行可能
        return completedStories.Contains(scene.PrerequisiteStoryId.Value);
    }
}

