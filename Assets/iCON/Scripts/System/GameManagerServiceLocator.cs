using UnityEngine;

/// <summary>
/// GameManagerへの参照を提供するサービスロケーター
/// </summary>
public static class GameManagerServiceLocator
{
    private static IGameManager _instance;
    private static bool _isInitialized = false;
    
    public static IGameManager Instance
    {
        get
        {
            if (_instance == null && _isInitialized)
            {
                Debug.LogError("GameManagerが初期化されていません");
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// GameManagerインスタンスを設定する
    /// </summary>
    public static void SetInstance(IGameManager instance)
    {
        _instance = instance;
        _isInitialized = true;
    }
    
    /// <summary>
    /// テスト用にGameManagerをリセットする
    /// </summary>
    public static void Reset()
    {
        _instance = null;
        _isInitialized = false;
    }
    
    /// <summary>
    /// GameManagerが初期化されているかを確認
    /// </summary>
    public static bool IsInitialized() => _isInitialized;
}