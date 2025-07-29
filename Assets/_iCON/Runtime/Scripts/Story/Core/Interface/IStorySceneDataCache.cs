using System.Collections.Generic;
using CryStar.Story.Data;

namespace CryStar.Story.Core
{
    /// <summary>
    /// StorySceneDataのキャッシュ管理を行うクラスが継承すべきインターフェース
    /// </summary>
    public interface IStorySceneDataCache
    {
        /// <summary>
        /// キャッシュから取得を試行
        /// </summary>
        bool TryGet(int sceneId, out List<OrderData> data);
        
        /// <summary>
        /// キャッシュに保存
        /// </summary>
        void Set(int sceneId, List<OrderData> data);
        
        /// <summary>
        /// 指定シーンのキャッシュをクリア
        /// </summary>
        void Remove(int sceneId);
        
        /// <summary>
        /// 全キャッシュをクリア
        /// </summary>
        void Clear();
        
        /// <summary>
        /// キャッシュ済みか確認する
        /// </summary>
        bool Contains(int sceneId);
    }
}