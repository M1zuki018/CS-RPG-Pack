#if UNITY_EDITOR
using CryStar.Network;
using UnityEditor;
using UnityEngine;

namespace CryStar.Editor
{
    /// <summary>
    /// SheetsDataService専用のエディタウィンドウ
    /// </summary>
    public class SheetsDataServiceWindow : EditorWindow
    {
        private SheetsDataService _service;
        private Vector2 _scrollPosition;
        private string _newSpreadsheetName = "";
        private string _newSpreadsheetId = "";

        [MenuItem("CryStar/Network/Sheets Data Service")]
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
}
#endif