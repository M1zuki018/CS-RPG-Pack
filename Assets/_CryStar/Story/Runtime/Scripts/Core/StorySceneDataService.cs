using System.Collections.Generic;
using CryStar.Story.Data;
using Cysharp.Threading.Tasks;

namespace CryStar.Story.Core
{
    /// <summary>
    /// ストーリーのシーンデータの管理を統括するクラス
    /// </summary>
    public class StorySceneDataService : IStorySceneDataService
    {
        /// <summary>
        /// 読み取り
        /// </summary>
        private readonly IStorySceneDataRepository _repository;
        
        /// <summary>
        /// 変換
        /// </summary>
        private readonly IStorySceneDataConverter _converter;
        
        /// <summary>
        /// キャッシュ管理
        /// </summary>
        private readonly IStorySceneDataCache _cache;
    
        /// <summary>
        /// 初期化済みか
        /// </summary>
        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StorySceneDataService(IStorySceneDataRepository repository, IStorySceneDataConverter converter,
            IStorySceneDataCache cache)
        {
            _repository = repository;
            _converter = converter;
            _cache = cache;
        }
    
        /// <summary>
        /// Initialize - ヘッダー行から変換マップを作成
        /// </summary>
        public async UniTask InitializeAsync(string spreadsheetName, string headerRange)
        {
            if (_isInitialized)
            {
                // 初期化済みであれば処理は行わない
                return;
            }
        
            // ヘッダーデータを取得して、コンバーターの変換マップの初期化を行う
            var headerData = await _repository.InitializeAsync(spreadsheetName, headerRange);
            _converter.Initialize(headerData);
        
            _isInitialized = true;
        }
    
        /// <summary>
        /// シーンデータを取得する
        /// </summary>
        public async UniTask<List<OrderData>> GetSceneDataAsync(int sceneId, string spreadsheetName, string dataRange)
        {
            // キャッシュを検索
            if (_cache.TryGet(sceneId, out var cachedData))
            {
                return cachedData;
            }
        
            // データの取得と変換を行う
            var rawData = await _repository.LoadSceneDataAsync(spreadsheetName, dataRange);
            var orderData = _converter.ConvertToOrderDataList(rawData);
        
            // キャッシュに登録
            _cache.Set(sceneId, orderData);
        
            return orderData;
        }

        /// <summary>
        /// 指定したシーンIDのキャッシュをクリア
        /// </summary>
        public void ClearCache(int sceneId)
        {
            if (_cache != null && _cache.Contains(sceneId))
            {
                // 存在していたらキャッシュをクリア
                _cache.Remove(sceneId);
            }
        }
        
        /// <summary>
        /// 登録されている全てのキャッシュをクリア
        /// </summary>
        public void ClearAllCache()
        {
            if (_cache != null)
            {
                _cache.Clear();
            }
        }
    }
}
