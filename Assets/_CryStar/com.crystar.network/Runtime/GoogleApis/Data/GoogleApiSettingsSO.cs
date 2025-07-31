using System.Collections.Generic;
using UnityEngine;

namespace CryStar.Network
{
    /// <summary>
    /// Google API関連の設定を管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "GoogleApiSettings", menuName = "CryStar/Google API Settings", order = 100)]
    public class GoogleApiSettingsSO : ScriptableObject
    {
        [Header("認証設定")] [SerializeField, Tooltip("サービスアカウントキーのファイル名")]
        private string _serviceAccountKeyFileName = "service-account-key.json";

        [SerializeField, Tooltip("APIリクエスト時に送信されるアプリケーション名")]
        private string _applicationName = "My Unity Game";

        [Header("スプレッドシート設定")] [SerializeField, Tooltip("登録されたスプレッドシート一覧")]
        private List<SpreadsheetConfig> _spreadsheetConfigs = new List<SpreadsheetConfig>();

        /// <summary>
        /// サービスアカウントキーファイル名
        /// </summary>
        public string ServiceAccountKeyFileName
        {
            get => _serviceAccountKeyFileName;
            set => _serviceAccountKeyFileName = value;
        }

        /// <summary>
        /// アプリケーション名
        /// </summary>
        public string ApplicationName
        {
            get => _applicationName;
            set => _applicationName = value;
        }

        /// <summary>
        /// スプレッドシート設定一覧
        /// </summary>
        public List<SpreadsheetConfig> SpreadsheetConfigs => _spreadsheetConfigs;

        /// <summary>
        /// スプレッドシート設定を追加
        /// </summary>
        public void AddSpreadsheetConfig(string name, string spreadsheetId, string description = "")
        {
            // 重複チェック
            foreach (var config in _spreadsheetConfigs)
            {
                if (config.Name == name)
                {
                    Debug.LogWarning($"スプレッドシート名 '{name}' は既に存在します");
                    return;
                }
            }

            var newConfig = new SpreadsheetConfig();
            // リフレクションで設定（SpreadsheetConfigのフィールドがprivateのため）
            var nameField = typeof(SpreadsheetConfig).GetField("_name",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var idField = typeof(SpreadsheetConfig).GetField("_spreadsheetId",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var descField = typeof(SpreadsheetConfig).GetField("_description",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            nameField?.SetValue(newConfig, name);
            idField?.SetValue(newConfig, spreadsheetId);
            descField?.SetValue(newConfig, description);

            _spreadsheetConfigs.Add(newConfig);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif

            Debug.Log($"スプレッドシート設定を追加: {name}");
        }

        /// <summary>
        /// スプレッドシート設定を削除
        /// </summary>
        public bool RemoveSpreadsheetConfig(string name)
        {
            for (int i = 0; i < _spreadsheetConfigs.Count; i++)
            {
                if (_spreadsheetConfigs[i].Name == name)
                {
                    _spreadsheetConfigs.RemoveAt(i);

#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
#endif

                    Debug.Log($"スプレッドシート設定を削除: {name}");
                    return true;
                }
            }

            Debug.LogWarning($"スプレッドシート '{name}' が見つかりません");
            return false;
        }

        /// <summary>
        /// 指定名のスプレッドシート設定を取得
        /// </summary>
        public SpreadsheetConfig GetSpreadsheetConfig(string name)
        {
            foreach (var config in _spreadsheetConfigs)
            {
                if (config.Name == name)
                {
                    return config;
                }
            }

            return null;
        }

        /// <summary>
        /// 設定の妥当性をチェック
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(_serviceAccountKeyFileName))
            {
                Debug.LogError("サービスアカウントキーファイル名が設定されていません");
                return false;
            }

            if (string.IsNullOrEmpty(_applicationName))
            {
                Debug.LogError("アプリケーション名が設定されていません");
                return false;
            }

            // キーファイルの存在確認
            var keyFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, _serviceAccountKeyFileName);
            if (!System.IO.File.Exists(keyFilePath))
            {
                Debug.LogError($"サービスアカウントキーファイルが見つかりません: {keyFilePath}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// SheetsDataServiceに設定を適用
        /// </summary>
        public void ApplyToService(SheetsDataService service)
        {
            if (service == null)
            {
                Debug.LogError("SheetsDataServiceが無効です");
                return;
            }

            // リフレクションで設定を適用
            var serviceAccountKeyField = typeof(SheetsDataService).GetField("_serviceAccountKeyFileName",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var applicationNameField = typeof(SheetsDataService).GetField("_applicationName",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var configsField = typeof(SheetsDataService).GetField("_spreadsheetConfigs",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            serviceAccountKeyField?.SetValue(service, _serviceAccountKeyFileName);
            applicationNameField?.SetValue(service, _applicationName);
            configsField?.SetValue(service, _spreadsheetConfigs.ToArray());

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(service);
#endif

            Debug.Log("GoogleApiSettingsをSheetsDataServiceに適用しました");
        }

#if UNITY_EDITOR
        /// <summary>
        /// デフォルト設定でアセットを作成
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/CryStar/Google API Settings")]
        public static GoogleApiSettingsSO CreateDefaultAsset()
        {
            var settings = CreateInstance<GoogleApiSettingsSO>();

            var path = "Assets/GoogleApiSettings.asset";
            var uniquePath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);

            UnityEditor.AssetDatabase.CreateAsset(settings, uniquePath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            UnityEditor.Selection.activeObject = settings;
            UnityEditor.EditorGUIUtility.PingObject(settings);

            Debug.Log($"GoogleApiSettings を作成: {uniquePath}");

            return settings;
        }

        /// <summary>
        /// プロジェクト内のGoogleApiSettingsを検索
        /// </summary>
        public static GoogleApiSettingsSO FindInProject()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:GoogleApiSettingsSO");

            if (guids.Length > 0)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<GoogleApiSettingsSO>(path);
            }

            return null;
        }

        /// <summary>
        /// プロジェクト内のGoogleApiSettingsを検索、なければ作成
        /// </summary>
        public static GoogleApiSettingsSO FindOrCreate()
        {
            var existing = FindInProject();
            if (existing != null)
            {
                return existing;
            }

            return CreateDefaultAsset();
        }
#endif
    }
}