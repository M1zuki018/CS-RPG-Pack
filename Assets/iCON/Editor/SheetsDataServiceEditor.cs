#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// SheetsDataService用のカスタムエディタ
/// </summary>
[CustomEditor(typeof(SheetsDataService))]
public class SheetsDataServiceEditor : Editor
{
    private SheetsDataService _target;
    private bool _showDebugInfo = false;
    private bool _showCacheInfo = false;
    private string _testSpreadsheetName = "";
    private string _testRange = "Sheet1!A1:Z10";

    private void OnEnable()
    {
        _target = (SheetsDataService)target;
    }

    public override void OnInspectorGUI()
    {
        // 基本情報の描画
        DrawDefaultInspector();
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);
        
        // 初期化状態の表示
        GUI.enabled = false;
        EditorGUILayout.Toggle("Is Initialized", _target.IsInitialized);
        GUI.enabled = true;
        
        // ボタン群
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("接続テスト"))
        {
            _target.TestConnection();
        }
        
        if (GUILayout.Button("設定情報表示"))
        {
            _target.LogConfiguration();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("キャッシュ情報"))
        {
            _target.LogCacheInfo();
        }
        
        if (GUILayout.Button("全キャッシュクリア"))
        {
            if (EditorUtility.DisplayDialog("確認", "全てのキャッシュをクリアしますか？", "はい", "いいえ"))
            {
                _target.ClearAllCache();
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        // テストデータ読み込み
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Test Data Loading", EditorStyles.boldLabel);
        
        _testSpreadsheetName = EditorGUILayout.TextField("Spreadsheet Name", _testSpreadsheetName);
        _testRange = EditorGUILayout.TextField("Range", _testRange);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("データ読み込み"))
        {
            if (!string.IsNullOrEmpty(_testSpreadsheetName) && !string.IsNullOrEmpty(_testRange))
            {
                TestLoadData();
            }
            else
            {
                Debug.LogWarning("スプレッドシート名と範囲を入力してください");
            }
        }
        
        if (GUILayout.Button("強制更新"))
        {
            if (!string.IsNullOrEmpty(_testSpreadsheetName) && !string.IsNullOrEmpty(_testRange))
            {
                TestLoadData(true);
            }
            else
            {
                Debug.LogWarning("スプレッドシート名と範囲を入力してください");
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        // 利用可能なスプレッドシート一覧
        EditorGUILayout.Space(5);
        _showDebugInfo = EditorGUILayout.Foldout(_showDebugInfo, "Available Spreadsheets");
        if (_showDebugInfo && _target.IsInitialized)
        {
            EditorGUI.indentLevel++;
            
            var availableNames = _target.AvailableSpreadsheetNames;
            if (availableNames.Length > 0)
            {
                foreach (var name in availableNames)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(name);
                    
                    if (GUILayout.Button("使用", GUILayout.Width(50)))
                    {
                        _testSpreadsheetName = name;
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("利用可能なスプレッドシートがありません");
            }
            
            EditorGUI.indentLevel--;
        }
        
        // キャッシュ統計情報
        EditorGUILayout.Space(5);
        _showCacheInfo = EditorGUILayout.Foldout(_showCacheInfo, "Cache Statistics");
        if (_showCacheInfo && _target.IsInitialized)
        {
            EditorGUI.indentLevel++;
            
            var stats = _target.GetCacheStats();
            EditorGUILayout.LabelField($"Cache Entries: {stats.entryCount}");
            EditorGUILayout.LabelField($"Total Rows: {stats.totalRows}");
            EditorGUILayout.LabelField($"Estimated Memory: {stats.estimatedMemoryKB} KB");
            
            EditorGUI.indentLevel--;
        }
        
        // ステータス情報
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(_target.GetStatusInfo(), MessageType.Info);
        
        // 自動更新
        if (Application.isPlaying)
        {
            Repaint();
        }
    }

    private async void TestLoadData(bool forceRefresh = false)
    {
        try
        {
            Debug.Log($"テストデータ読み込み開始: {_testSpreadsheetName} - {_testRange}");
            
            var data = await _target.ReadFromSpreadsheetAsync(_testSpreadsheetName, _testRange, forceRefresh);
            
            if (data != null)
            {
                Debug.Log($"データ読み込み成功: {data.Count} 行");
                _target.LogTableContentInternal(_testSpreadsheetName, _testRange, 3);
            }
            else
            {
                Debug.LogWarning("データが取得できませんでした");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"データ読み込みエラー: {e.Message}");
        }
    }
}

/// <summary>
/// SheetsDataService専用のエディタウィンドウ
/// </summary>
public class SheetsDataServiceWindow : EditorWindow
{
    private SheetsDataService _service;
    private Vector2 _scrollPosition;
    private string _newSpreadsheetName = "";
    private string _newSpreadsheetId = "";

    [MenuItem("Tools/iCON/Sheets Data Service")]
    public static void ShowWindow()
    {
        var window = GetWindow<SheetsDataServiceWindow>("Sheets Service");
        window.minSize = new Vector2(400, 300);
    }

    private void OnEnable()
    {
        _service = SheetsDataService.Instance;
    }

    private void OnGUI()
    {
        if (_service == null)
        {
            EditorGUILayout.HelpBox("SheetsDataService が見つかりません", MessageType.Warning);
            
            if (GUILayout.Button("SheetsDataService を検索"))
            {
                _service = FindObjectOfType<SheetsDataService>();
                if (_service == null)
                {
                    EditorGUILayout.HelpBox("シーンにSheetsDataServiceが存在しません", MessageType.Error);
                }
            }
            return;
        }

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        // ヘッダー
        EditorGUILayout.LabelField("Google Sheets Data Service Manager", EditorStyles.largeLabel);
        EditorGUILayout.Space();

        // ステータス表示
        EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(_service.GetStatusInfo(), MessageType.Info);

        // スプレッドシート管理
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spreadsheet Management", EditorStyles.boldLabel);

        // 新しいスプレッドシート追加
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Add New Spreadsheet", EditorStyles.boldLabel);
        
        _newSpreadsheetName = EditorGUILayout.TextField("Name", _newSpreadsheetName);
        _newSpreadsheetId = EditorGUILayout.TextField("Spreadsheet ID", _newSpreadsheetId);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add"))
        {
            if (!string.IsNullOrEmpty(_newSpreadsheetName) && !string.IsNullOrEmpty(_newSpreadsheetId))
            {
                _service.AddSpreadsheetConfig(_newSpreadsheetName, _newSpreadsheetId);
                _newSpreadsheetName = "";
                _newSpreadsheetId = "";
            }
        }
        
        if (GUILayout.Button("Clear"))
        {
            _newSpreadsheetName = "";
            _newSpreadsheetId = "";
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        // 登録済みスプレッドシート一覧
        if (_service.IsInitialized)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Registered Spreadsheets", EditorStyles.boldLabel);
            
            var availableNames = _service.AvailableSpreadsheetNames;
            foreach (var name in availableNames)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(name);
                
                if (GUILayout.Button("Clear Cache", GUILayout.Width(100)))
                {
                    _service.ClearCache(name);
                }
                
                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    if (EditorUtility.DisplayDialog("確認", $"'{name}' を削除しますか？", "はい", "いいえ"))
                    {
                        _service.RemoveSpreadsheetConfig(name);
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }

        // 一括操作
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Bulk Operations", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Force Refresh All"))
        {
            if (EditorUtility.DisplayDialog("確認", "全データを強制更新しますか？", "はい", "いいえ"))
            {
                _service.ForceRefreshAllData();
            }
        }
        
        if (GUILayout.Button("Clear All Cache"))
        {
            if (EditorUtility.DisplayDialog("確認", "全キャッシュをクリアしますか？", "はい", "いいえ"))
            {
                _service.ClearAllCache();
            }
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }
}
#endif