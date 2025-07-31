using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using UnityEngine.Serialization;

namespace CryStar.Network
{
    /// <summary>
    /// Googleスプレッドシートとの通信を行うクラス
    /// </summary>
    public class SheetsDataService : MonoBehaviour
    {
        // インスタンス
        private static SheetsDataService _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;

        [Header("認証設定")] 
        [SerializeField] private GoogleApiSettingsSO _apiSettings;

        [Header("スプレッドシート設定")] [SerializeField]
        private SpreadsheetConfig[] _spreadsheetConfigs;

        // Service関連
        private SheetsService _sheetsService;
        private Dictionary<string, string> _spreadsheetIdMap = new Dictionary<string, string>();

        // キャッシュ用辞書（スプレッドシート名 + 範囲をキーにしてデータを保持）
        private Dictionary<string, IList<IList<object>>> _dataCache = new Dictionary<string, IList<IList<object>>>();

        // 初期化状態管理
        private bool _isInitialized = false; // 初期化済み
        private bool _isInitializing = false; // 初期化中
        private UniTaskCompletionSource _initializationTcs; // 初期化用のCompletionSource

        /// <summary>
        /// SheetsDataServiceのインスタンス
        /// </summary>
        public static SheetsDataService Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning("SheetsDataService: アプリケーション終了中のためnullを返します");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<SheetsDataService>();

                        if (_instance == null)
                        {
                            GameObject go = new GameObject("SheetsDataService");
                            _instance = go.AddComponent<SheetsDataService>();
                            DontDestroyOnLoad(go);
                        }
                    }

                    return _instance;
                }
            }
        }
        
        /// <summary>
        /// API設定アセット
        /// </summary>
        public GoogleApiSettingsSO ApiSettings 
        { 
            get => _apiSettings; 
            set => _apiSettings = value; 
        }

        /// <summary>
        /// 初期化が完了しているかどうか
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// 利用可能なスプレッドシート名一覧
        /// </summary>
        public string[] AvailableSpreadsheetNames
        {
            get
            {
                var names = new string[_spreadsheetIdMap.Count];
                _spreadsheetIdMap.Keys.CopyTo(names, 0);
                return names;
            }
        }

        #region Life cycle

        /// <summary>
        /// OnAwake
        /// </summary>
        private async void Awake()
        {
            // シングルトンパターン
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                await InitializeAsync();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        /// <summary>
        /// 指定されたスプレッドシートIDを取得
        /// </summary>
        public string GetSpreadsheetId(string name)
        {
            if (_spreadsheetIdMap.TryGetValue(name, out var spreadsheetId))
            {
                return spreadsheetId;
            }

            Debug.LogError($"スプレッドシート '{name}' が見つかりません。利用可能な名前: {string.Join(", ", _spreadsheetIdMap.Keys)}");
            return null;
        }

        /// <summary>
        /// スプレッドシートからデータを読み取り（キャッシュ機能付き）
        /// </summary>
        /// <param name="spreadsheetName">スプレッドシート名</param>
        /// <param name="range">読み取り範囲</param>
        /// <param name="forceRefresh">キャッシュを無視して再取得するか</param>
        /// <returns>データ</returns>
        public async UniTask<IList<IList<object>>> ReadFromSpreadsheetAsync(string spreadsheetName, string range,
            bool forceRefresh = false)
        {
            await EnsureInitializedAsync();

            string cacheKey = $"{spreadsheetName}:{range}";

            // キャッシュから取得を試行
            if (!forceRefresh && _dataCache.TryGetValue(cacheKey, out var cachedData))
            {
                Debug.Log($"[{spreadsheetName}] キャッシュからデータを返却: {cachedData?.Count ?? 0} 行");
                return cachedData;
            }

            // スプレッドシートから取得
            var spreadsheetId = GetSpreadsheetId(spreadsheetName);
            if (string.IsNullOrEmpty(spreadsheetId))
            {
                throw new ArgumentException($"スプレッドシート '{spreadsheetName}' が見つかりません");
            }

            try
            {
                var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
                var response = await request.ExecuteAsync();

                // キャッシュに保存
                _dataCache[cacheKey] = response.Values;

                Debug.Log($"[{spreadsheetName}] データ取得＆キャッシュ保存: {response.Values?.Count ?? 0} 行");
                return response.Values;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{spreadsheetName}] データ読み取りエラー: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 特定のスプレッドシートのキャッシュをクリア
        /// </summary>
        public void ClearCache(string spreadsheetName)
        {
            var keysToRemove = new List<string>();
            foreach (var key in _dataCache.Keys)
            {
                if (key.StartsWith($"{spreadsheetName}:"))
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _dataCache.Remove(key);
            }

            Debug.Log($"[{spreadsheetName}] キャッシュクリア完了: {keysToRemove.Count} エントリ削除");
        }

        /// <summary>
        /// 全キャッシュをクリア
        /// </summary>
        public void ClearAllCache()
        {
            var count = _dataCache.Count;
            _dataCache.Clear();
            Debug.Log($"全キャッシュクリア完了: {count} エントリ削除");
        }


        /// <summary>
        /// スプレッドシート設定を動的に追加
        /// </summary>
        public void AddSpreadsheetConfig(string name, string spreadsheetId)
        {
            if (_spreadsheetIdMap.ContainsKey(name))
            {
                Debug.Log($"スプレッドシート '{name}' は既に存在します");
                return;
            }

            _spreadsheetIdMap[name] = spreadsheetId;
            Debug.Log($"スプレッドシート追加: {name}");
        }

        /// <summary>
        /// スプレッドシート設定を削除
        /// </summary>
        public void RemoveSpreadsheetConfig(string name)
        {
            if (_spreadsheetIdMap.Remove(name))
            {
                ClearCache(name); // 関連キャッシュも削除
                Debug.Log($"スプレッドシート削除: {name}");
            }
            else
            {
                Debug.LogWarning($"スプレッドシート '{name}' が見つかりません");
            }
        }

        /// <summary>
        /// 設定情報とキャッシュ状況を表示
        /// </summary>
        [ContextMenu("設定情報を表示")]
        public void LogConfiguration()
        {
            Debug.Log($"=== Google Sheets 設定情報 ===" +
                      $"\n初期化状態: {(_isInitialized ? "完了" : "未完了")}" +
                      $"\n登録済みスプレッドシート数: {_spreadsheetIdMap.Count} キャッシュエントリ数: {_dataCache.Count}");

            foreach (var kvp in _spreadsheetIdMap)
            {
                Debug.Log($"- {kvp.Key}: {kvp.Value}");
            }

            if (_dataCache.Count > 0)
            {
                Debug.Log("--- キャッシュ状況 ---");
                foreach (var cacheKey in _dataCache.Keys)
                {
                    var rowCount = _dataCache[cacheKey]?.Count ?? 0;
                    Debug.Log($"- {cacheKey}: {rowCount} 行");
                }
            }
        }

        /// <summary>
        /// ランタイム用の軽量ステータス情報取得
        /// </summary>
        public string GetStatusInfo()
        {
            return $"Initialized: {_isInitialized}, Spreadsheets: {_spreadsheetIdMap.Count}, Cache: {_dataCache.Count}";
        }

        /// <summary>
        /// ランタイム用のキャッシュ統計情報
        /// </summary>
        public (int entryCount, int totalRows, long estimatedMemoryKB) GetCacheStats()
        {
            var entryCount = _dataCache.Count;
            var totalRows = 0;
            var estimatedMemoryKB = 0L;

            foreach (var data in _dataCache.Values)
            {
                if (data != null)
                {
                    totalRows += data.Count;

                    // 大まかなメモリ使用量を推定（文字列は平均10バイト、オブジェクトは20バイトと仮定）
                    foreach (var row in data)
                    {
                        if (row != null)
                        {
                            estimatedMemoryKB += row.Count * 30; // バイト単位
                        }
                    }
                }
            }

            return (entryCount, totalRows, estimatedMemoryKB / 1024);
        }

        /// <summary>
        /// 指定されたテーブルの内容を表示
        /// </summary>
        public void LogTableContentInternal(string spreadsheetName, string range, int maxRows = 5)
        {
            string cacheKey = $"{spreadsheetName}:{range}";

            if (!_dataCache.TryGetValue(cacheKey, out var data))
            {
                Debug.LogWarning($"[{spreadsheetName}] キャッシュにデータが存在しません。先にReadFromSpreadsheetAsyncを実行してください");
                return;
            }

            if (data == null || data.Count == 0)
            {
                Debug.LogWarning($"[{spreadsheetName}] データが空です");
                return;
            }

            Debug.Log($"=== {spreadsheetName} テーブル内容 ===");
            Debug.Log($"総行数: {data.Count}");

            for (int i = 0; i < Math.Min(maxRows, data.Count); i++)
            {
                var row = data[i];
                var cellValues = new List<string>();

                for (int j = 0; j < row.Count; j++)
                {
                    cellValues.Add($"[{j}]={row[j] ?? "null"}");
                }

                Debug.Log($"Row[{i}]: {string.Join(", ", cellValues)}");
            }

            if (data.Count > maxRows)
            {
                Debug.Log($"... 他 {data.Count - maxRows} 行");
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// キャッシュ情報を表示
        /// </summary>
        [ContextMenu("キャッシュ情報を表示")]
        public void LogCacheInfo()
        {
            Debug.Log($"=== キャッシュ情報 === 総エントリ数: {_dataCache.Count}");

            foreach (var kvp in _dataCache)
            {
                var rowCount = kvp.Value?.Count ?? 0;
                var cellCount = 0;
                if (kvp.Value != null)
                {
                    foreach (var row in kvp.Value)
                    {
                        cellCount += row?.Count ?? 0;
                    }
                }

                Debug.Log($"- {kvp.Key}: {rowCount} 行, {cellCount} セル");
            }
        }

        /// <summary>
        /// 特定テーブルの内容を表示（エディタ専用・デバッグ用）
        /// </summary>
        [ContextMenu("テーブル内容確認")]
        public void LogTableContent()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("初期化が完了していません");
                return;
            }

            // 最初のスプレッドシートの最初のシートを表示
            if (_spreadsheetIdMap.Count > 0)
            {
                var firstSpreadsheet = _spreadsheetIdMap.Keys.First();
                LogTableContentInternal(firstSpreadsheet, "Sheet1!A1:Z10");
            }
            else
            {
                Debug.LogWarning("登録されたスプレッドシートがありません");
            }
        }

        /// <summary>
        /// 強制的に全データを再取得（エディタ専用・デバッグ用）
        /// </summary>
        [ContextMenu("全データ強制更新")]
        public async void ForceRefreshAllData()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("初期化が完了していません");
                return;
            }

            Debug.Log("=== 全データ強制更新開始 ===");

            var cacheKeys = _dataCache.Keys.ToArray();
            var refreshCount = 0;

            foreach (var cacheKey in cacheKeys)
            {
                try
                {
                    var parts = cacheKey.Split(':');
                    if (parts.Length == 2)
                    {
                        var spreadsheetName = parts[0];
                        var range = parts[1];

                        Debug.Log($"更新中: {cacheKey}");
                        await ReadFromSpreadsheetAsync(spreadsheetName, range, true);
                        refreshCount++;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"更新エラー [{cacheKey}]: {e.Message}");
                }
            }

            Debug.Log($"=== 全データ強制更新完了: {refreshCount} エントリ更新 ===");
        }

        /// <summary>
        /// 接続テスト（エディタ専用）
        /// </summary>
        [ContextMenu("接続テスト実行")]
        public async void TestConnection()
        {
            try
            {
                Debug.Log("=== 接続テスト開始 ===");

                if (!_isInitialized)
                {
                    await InitializeAsync();
                }

                await TestSpreadsheetAccess();
                Debug.Log("=== 接続テスト完了 ===");
            }
            catch (Exception e)
            {
                Debug.LogError($"接続テストエラー: {e.Message}");
            }
        }
#endif

        #region Private Methods

        /// <summary>
        /// 初期化
        /// </summary>
        private async UniTask InitializeAsync()
        {
            if (_isInitialized)
            {
                // 既に初期化が済んでいたら以降の処理は行わない
                return;
            }

            if (_isInitializing)
            {
                // 初期化中であれば、初期化が終了するのを待つ
                await _initializationTcs.Task;
                return;
            }

            _isInitializing = true;
            _initializationTcs = new UniTaskCompletionSource();

            try
            {
                Debug.Log("Google Sheets API初期化開始");

                await InitializeGoogleSheetsService();
                RegisterSpreadsheetConfigs();
                await TestSpreadsheetAccess();

                _isInitialized = true;
                Debug.Log("Google Sheets API初期化完了");

                _initializationTcs.TrySetResult();
            }
            catch (Exception e)
            {
                Debug.LogError($"Google Sheets API初期化エラー: {e.Message}");
                _initializationTcs.TrySetException(e);
                throw;
            }
            finally
            {
                _isInitializing = false;
            }
        }

        /// <summary>
        /// Google Sheets APIサービスの初期化
        /// </summary>
        private async UniTask InitializeGoogleSheetsService()
        {
            // StreamingAssetsフォルダ内のサービスアカウントキーファイルのパスを構築
            // NOTE: サービスアカウントキーファイルは必ず「Assets/StreamingAssets」の下におく
            string keyFilePath = Path.Combine(Application.streamingAssetsPath,
                _apiSettings.ServiceAccountKeyFileName);

            // サービスアカウントキーファイルの存在確認
            if (!File.Exists(keyFilePath))
            {
                throw new FileNotFoundException($"サービスアカウントキーファイルが見つかりません: {keyFilePath}");
            }

            // サービスアカウントキーファイルを非同期で読み込み
            string jsonContent = await File.ReadAllTextAsync(keyFilePath);

            // JSONからGoogleCredentialオブジェクトを作成
            GoogleCredential credential = GoogleCredential.FromJson(jsonContent);

            // 認証スコープを読み取り専用に設定
            credential = credential.CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

            // Google Sheets APIサービスを初期化
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                // 認証情報を設定
                HttpClientInitializer = credential,
                ApplicationName = _apiSettings.ApplicationName
            });

            // 初期化完了フラグを設定
            _isInitialized = true;
        }

        /// <summary>
        /// スプレッドシート設定を辞書に登録
        /// </summary>
        private void RegisterSpreadsheetConfigs()
        {
            _spreadsheetIdMap.Clear();

            foreach (var config in _spreadsheetConfigs)
            {
                if (string.IsNullOrEmpty(config.Name) || string.IsNullOrEmpty(config.SpreadsheetId))
                {
                    Debug.LogWarning($"無効な設定をスキップ: Name={config.Name}, ID={config.SpreadsheetId}");
                    continue;
                }

                if (_spreadsheetIdMap.ContainsKey(config.Name))
                {
                    Debug.LogWarning($"重複するスプレッドシート名をスキップ: {config.Name}");
                    continue;
                }

                _spreadsheetIdMap[config.Name] = config.SpreadsheetId;
                Debug.Log($"スプレッドシート登録: {config.Name}");
            }

            Debug.Log($"登録完了: {_spreadsheetIdMap.Count} 件");
        }

        /// <summary>
        /// スプレッドシートアクセステスト
        /// </summary>
        private async UniTask TestSpreadsheetAccess()
        {
            var successCount = 0;
            var failureCount = 0;

            foreach (var kvp in _spreadsheetIdMap)
            {
                var name = kvp.Key;
                var spreadsheetId = kvp.Value;

                try
                {
                    Debug.Log($"\n[{name}] アクセステスト開始...");
                    await TestSingleSpreadsheetAccess(name, spreadsheetId);
                    successCount++;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[{name}] アクセステストエラー: {e.Message}");
                    failureCount++;
                }
            }

            Debug.Log($"=== アクセステスト完了 === 成功: {successCount}, 失敗: {failureCount}");

            if (failureCount > 0)
            {
                Debug.LogError("失敗したスプレッドシートがあります。権限設定を確認してください。");
            }
        }

        /// <summary>
        /// 単一スプレッドシートのアクセステスト
        /// </summary>
        private async UniTask TestSingleSpreadsheetAccess(string name, string spreadsheetId)
        {
            var request = _sheetsService.Spreadsheets.Get(spreadsheetId);
            var spreadsheet = await request.ExecuteAsync();

            Debug.Log($"[{name}] スプレッドシート名: {spreadsheet.Properties.Title}\n[{name}] シート数: {spreadsheet.Sheets.Count}");

            foreach (var sheet in spreadsheet.Sheets)
            {
                Debug.Log($"[{name}] - シート名: '{sheet.Properties.Title}'");
            }
        }

        /// <summary>
        /// 初期化を確認してからAPI実行
        /// </summary>
        private async UniTask EnsureInitializedAsync()
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }
        }

        #endregion
    }
}