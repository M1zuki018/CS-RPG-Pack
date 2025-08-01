using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// テンプレートを選択しスクリプトを作成するウィンドウを提供するエディター拡張
/// </summary>
public class ScriptCreationWindow : EditorWindow
{
    private int _selectedTab = 0; // タブのインデックス
    private string _scriptName = "NewScript"; // スクリプトの名前
    private string _savePath = "Assets"; // 保存パス
    private int _templateIndex = 0; // テンプレートのインデックス
    private string[] _templates = new string[0]; // テンプレートの配列
    private string _enumPath = "Assets";
    private string _templateFolderPath = "Assets/Editor/ScriptTemplates"; // スクリプトテンプレートのパス
    
    [MenuItem("CryStar/Tools/Script Creation Window")]
    public static void ShowWindow()
    {
        // ウィンドウを表示
        ScriptCreationWindow window = GetWindow<ScriptCreationWindow>("Script Creation");
        window.Show();
    }

    private void OnEnable()
    {
        LoadSettings(); // 設定をロード
        LoadTemplates(); // スクリプトのテンプレートをロード
    }
    
    private void OnDisable()
    {
        SaveSettings(); // 設定を保存
    }

    private void OnGUI()
    {
        // 設定セクション
        ShowSettingsGUI();
        
        EditorGUILayout.Space();
        
        // スクリプト名を入力するフィールド
        _scriptName = EditorGUILayout.TextField("Script Name", _scriptName);
        
        // 保存フォルダ選択フィールド
        GUILayout.Label("Select Save Folder", EditorStyles.boldLabel);
        _savePath = EditorGUILayout.TextField("Folder Path", _savePath);
        
        // パス選択ボタン
        if (GUILayout.Button("Select Folder"))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");

            if (!string.IsNullOrEmpty(folderPath))
            {
                if (folderPath.StartsWith(Application.dataPath))
                {
                    folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
                }
                _savePath = folderPath;
            }
        }
        
        // タブのレイアウト
        EditorGUILayout.BeginHorizontal();

        // スクリプト生成タブ
        if (GUILayout.Toggle(_selectedTab == 0, "Script Creation", "Button"))
        {
            _selectedTab = 0;
        }

        // Enum生成タブ
        if (GUILayout.Toggle(_selectedTab == 1, "Enum Generation", "Button"))
        {
            _selectedTab = 1;
        }

        EditorGUILayout.EndHorizontal();

        // タブに応じたGUIの表示
        switch (_selectedTab)
        {
            case 0:
                ShowScriptCreationGUI(); // スクリプト生成
                break;
            case 1:
                ShowEnumGenerationGUI(); // Enum生成
                break;
        }
    }
    
    /// <summary>
    /// 設定GUI
    /// </summary>
    private void ShowSettingsGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        
        // テンプレートフォルダパス設定
        EditorGUILayout.BeginHorizontal();
        _templateFolderPath = EditorGUILayout.TextField("Template Folder", _templateFolderPath);
        if (GUILayout.Button("Select", GUILayout.Width(60)))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Template Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(folderPath))
            {
                if (folderPath.StartsWith(Application.dataPath))
                {
                    folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
                }
                _templateFolderPath = folderPath;
                LoadTemplates(); // テンプレートを再読み込み
            }
        }
        EditorGUILayout.EndHorizontal();
        
        // テンプレート再読み込みボタン
        if (GUILayout.Button("Reload Templates"))
        {
            LoadTemplates();
        }
    }

    /// <summary>
    /// スクリプトのテンプレートから新しいスクリプトを作成するGUI
    /// </summary>
    private void ShowScriptCreationGUI()
    {
        GUILayout.Label("Create a New Script", EditorStyles.boldLabel);

        // テンプレートを選ぶドロップダウンメニュー
        if (_templates != null && _templates.Length > 0)
        {
            // テンプレートが存在したらポップアップに含めて表示する
            _templateIndex = EditorGUILayout.Popup("Template", _templateIndex, _templates);
        }
        else
        {
            // テンプレートが存在しなかったらメッセージを出す
            EditorGUILayout.HelpBox("'ScriptTemplates' フォルダーにテンプレートが見つかりません！", MessageType.Warning);
        }
        
        // スクリプト作成ボタン
        if (GUILayout.Button("Create Script"))
        {
            ScriptCreator.CreateScript(_savePath, _scriptName, _templateIndex, _templates, _templateFolderPath);
        }
    }
    
    /// <summary>
    /// Enum生成用のGUI
    /// </summary>
    private void ShowEnumGenerationGUI()
    {
        GUILayout.Label("Generate Enum from Folder", EditorStyles.boldLabel);

        // Enumを生成したいフォルダを選択するフィールド
        _enumPath = EditorGUILayout.TextField("Folder Path", _enumPath);
        
        // Enumを生成したいフォルダーのパスを選択するボタン
        if (GUILayout.Button("Select Create Enum Folder"))
        {
            string dataPath = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");

            if (!string.IsNullOrEmpty(dataPath))
            {
                if (dataPath.StartsWith(Application.dataPath))
                {
                    dataPath = "Assets" + dataPath.Substring(Application.dataPath.Length);
                }
                _enumPath = dataPath;
            }
        }

        // Enum生成ボタン
        if (GUILayout.Button("Generate Enum"))
        {
            EnumGenerator.GenerateEnum(_enumPath, _scriptName, _savePath);
        }
    }

    /// <summary>
    /// テンプレートの格納フォルダからテンプレートをロードする
    /// </summary>
    private void LoadTemplates()
    {
        if (Directory.Exists(_templateFolderPath))
        {
            // フォルダ内のtxtファイルをすべて取得
            string[] templateFiles = Directory.GetFiles(_templateFolderPath, "*.txt");
            
            // テンプレート名をファイル名（拡張子なし）でリスト化
            _templates = templateFiles
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }
        else
        {
            _templates = new string[] { }; // フォルダがない場合は空のリスト
        }
    }
    
    /// <summary>
    /// 設定を保存
    /// </summary>
    private void SaveSettings()
    {
        EditorPrefs.SetString("ScriptCreationWindow_TemplateFolderPath", _templateFolderPath);
        EditorPrefs.SetString("ScriptCreationWindow_SavePath", _savePath);
        EditorPrefs.SetString("ScriptCreationWindow_EnumPath", _enumPath);
        EditorPrefs.SetInt("ScriptCreationWindow_TemplateIndex", _templateIndex);
    }

    /// <summary>
    /// 設定を読み込み
    /// </summary>
    private void LoadSettings()
    {
        _templateFolderPath = EditorPrefs.GetString("ScriptCreationWindow_TemplateFolderPath", "Assets/_CryStar/Editor/ScriptTemplates");
        _savePath = EditorPrefs.GetString("ScriptCreationWindow_SavePath", "Assets");
        _enumPath = EditorPrefs.GetString("ScriptCreationWindow_EnumPath", "Assets");
        _templateIndex = EditorPrefs.GetInt("ScriptCreationWindow_TemplateIndex", 0);
    }
}