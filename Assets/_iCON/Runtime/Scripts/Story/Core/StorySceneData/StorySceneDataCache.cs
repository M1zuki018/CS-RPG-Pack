using System.Collections.Generic;
using CryStar.Story.Data;

namespace CryStar.Story.Core
{
    /// <summary>
    /// StorySceneDataのキャッシュ管理を行う
    /// </summary>
    public class StorySceneDataCache : IStorySceneDataCache
    {
        private readonly Dictionary<int, List<OrderData>> _cache = new();

        /// <summary>
        /// キャッシュから取得を試行
        /// </summary>
        public bool TryGet(int sceneId, out List<OrderData> data)
        {
            return _cache.TryGetValue(sceneId, out data);
        }

        /// <summary>
        /// キャッシュに保存
        /// </summary>
        public void Set(int sceneId, List<OrderData> data)
        {
            _cache[sceneId] = data;
        }

        /// <summary>
        /// 指定シーンのキャッシュをクリア
        /// </summary>
        public void Remove(int sceneId)
        {
            _cache.Remove(sceneId);
        }

        /// <summary>
        /// 全キャッシュをクリア
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// キャッシュ済みか確認する
        /// </summary>
        public bool Contains(int sceneId)
        {
            return _cache.ContainsKey(sceneId);
        }
    }
}