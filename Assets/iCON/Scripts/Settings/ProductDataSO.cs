using UnityEditor;
using UnityEngine;

/// <summary>
/// プロダクトのデータをまとめて管理するスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/ProductDataSO")]
public class ProductDataSO : ScriptableObject
{
    [Header("アプリケーション情報")]
    [SerializeField] private string _version = "1.0.0";
    [SerializeField] private string _buildNumber = "001";
    [SerializeField] private string _appName = "Game Name";
    
    [Header("SNS・外部リンク")]
    [SerializeField] private string _snsUrl = "";
    
    [Header("ゲーム設定")]
    [SerializeField] private bool _isDebugMode = false;
    [SerializeField] private int _targetFrameRate = 60;
    
    public string Version => _version;
    public string BuildNumber => _buildNumber;
    public string AppName => _appName;
    public string SnsUrl => _snsUrl;
    public bool IsDebugMode => _isDebugMode;
    public int TargetFrameRate => _targetFrameRate;
    
    /// <summary>
    /// フルバージョン文字列を取得（バージョン + ビルド番号）
    /// </summary>
    public string GetFullVersionString() => $"{_version}.{_buildNumber}";
    
#if UNITY_EDITOR
    [Header("エディタ専用")]
    [SerializeField] private bool _autoUpdateBuildNumber = true;
    
    /// <summary>
    /// ビルド時にビルド番号を自動更新
    /// </summary>
    [ContextMenu("Increment Build Number")]
    public void IncrementBuildNumber()
    {
        if (int.TryParse(_buildNumber, out int buildNum))
        {
            _buildNumber = (buildNum + 1).ToString("000");
            EditorUtility.SetDirty(this);
            Debug.Log($"Build number updated to: {_buildNumber}");
        }
    }
#endif
}