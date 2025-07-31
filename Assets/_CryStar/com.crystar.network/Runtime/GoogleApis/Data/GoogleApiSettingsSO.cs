using UnityEngine;

namespace CryStar.Network
{
    /// <summary>
    /// GoogleAPI利用時の認証設定のためのスクリプタブルオブジェクト
    /// </summary>
    [CreateAssetMenu(fileName = "GoogleApiSettings", menuName = "CryStar/Network/Google Api Settings")]
    public class GoogleApiSettingsSO : ScriptableObject
    {
        [SerializeField, Tooltip("サービスアカウントキーのファイル名")]
        private string _serviceAccountKeyFileName = "service-account-key.json";
    
        [SerializeField, Tooltip("APIリクエスト時に送信されるアプリケーション名")]
        private string _applicationName = "";
        
        /// <summary>
        /// サービスアカウントキーのファイル名
        /// </summary>
        public string ServiceAccountKeyFileName => _serviceAccountKeyFileName;
        
        /// <summary>
        /// APIリクエスト時に送信されるアプリケーション名
        /// </summary>
        public string ApplicationName => _applicationName;
    }
}
