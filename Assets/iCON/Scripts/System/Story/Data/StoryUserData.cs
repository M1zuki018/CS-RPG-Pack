using System;
using System.Collections.Generic;

public class StoryUserData
{
    private static Dictionary<StorySaveData, bool> _storySaveData = new Dictionary<StorySaveData, bool>();

    /// <summary>
    /// クリアしたか
    /// </summary>
    public static void AddStoryClearData(StorySaveData storySaveData)
    {
        _storySaveData[storySaveData] = true;
    }

    /// <summary>
    /// 前提ストーリーをClearしているか
    /// </summary>
    /// <returns></returns>
    public static bool IsPremiseStoryClear(StorySaveData storySaveData)
    {
        return _storySaveData.ContainsKey(storySaveData);
    }
}

[Serializable]
public class StorySaveData
{
    public int PartId;
    public int ChapterId;
    public int SceneId;

    public StorySaveData(int partId, int chapterId, int sceneId)
    {
        PartId = partId;
        ChapterId = chapterId;
        SceneId = sceneId;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is StorySaveData other)
        {
            return PartId == other.PartId && 
                   ChapterId == other.ChapterId && 
                   SceneId == other.SceneId;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PartId, ChapterId, SceneId);
    }

    public override string ToString()
    {
        return $"Story({PartId}-{ChapterId}-{SceneId})";
    }
}
