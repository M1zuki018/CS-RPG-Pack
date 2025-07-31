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

            serviceAccountKeyField?.SetValue(service, _serviceAccountKeyFileName);
            applicationNameField?.SetValue(service, _applicationName);

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