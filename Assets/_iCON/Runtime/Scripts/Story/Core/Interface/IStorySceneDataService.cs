using System.Collections.Generic;
using CryStar.Story.Data;
using Cysharp.Threading.Tasks;

namespace CryStar.Story.Core
{
    public interface IStorySceneDataService
    {
        /// <summary>
        /// 初期化済みか
        /// </summary>
        bool IsInitialized { get; }
        
        /// <summary>
        /// Initialize - ヘッダー行から変換マップを作成
        /// </summary>
        UniTask InitializeAsync(string spreadsheetName, string headerRange);
        
        /// <summary>
        /// シーンデータを取得する
        /// </summary>
        UniTask<List<OrderData>> GetSceneDataAsync(int sceneId, string spreadsheetName, string dataRange);
        
        /// <summary>
        /// 指定したシーンIDのキャッシュをクリア
        /// </summary>
        void ClearCache(int sceneId);
        
        /// <summary>
        /// 登録されている全てのキャッシュをクリア
        /// </summary>
        void ClearAllCache();
    }
}