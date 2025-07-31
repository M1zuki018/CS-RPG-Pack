using System.IO;
using UnityEditor;
using UnityEngine;

namespace CryStar.Network
{
    /// <summary>
    /// Google API関連の設定を管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "GoogleApiSettings", menuName = "CryStar/Network/Google API Settings")]
    public class GoogleApiSettingsSO : ScriptableObject
    {
        [Header("認証設定")] 
        [SerializeField, Tooltip("サービスアカウントキーのファイル名")]
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

            // キーファイルの存在を確認する
            var keyFilePath = Path.Combine(Application.streamingAssetsPath, _serviceAccountKeyFileName);
            if (!File.Exists(keyFilePath))
            {
                Debug.LogError($"サービスアカウントキーファイルが見つかりません: {keyFilePath}");
                return false;
            }

            return true;
        }
    }
}