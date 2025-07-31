#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using CryStar.Network;

namespace CryStar.Editor
{
    /// <summary>
    /// SheetsDataService セットアップウィザード
    /// </summary>
    public class SheetsServiceSetupWizard : EditorWindow
    {
        private enum SetupStep
        {
            Welcome,
            ApiSetup,
            ServiceAccountKey,
            ServiceObjectSetup,
            SpreadsheetConfig,
            Complete
        }

        private SetupStep _currentStep = SetupStep.Welcome;
        private Vector2 _scrollPosition;

        // セットアップ用の変数
        private GoogleApiSettingsSO _apiSettings;
        private bool _keyFileExists = false;
        private SheetsDataService _serviceInstance;
        private string _newSpreadsheetName = "";
        private string _newSpreadsheetId = "";
        private string _newSpreadsheetDescription = "";

        // スタイル
        private GUIStyle _headerStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _warningStyle;
        private GUIStyle _successStyle;

        /// <summary>
        /// ウィンドウを表示
        /// </summary>
        [MenuItem("CryStar/Network/Setup Wizard", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<SheetsServiceSetupWizard>("Sheets Service Setup");
            window.minSize = new Vector2(500, 400);
            window.maxSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            // GoogleApiSettingsを検索または作成
            _apiSettings = GoogleApiSettingsSO.FindOrCreate();
            CheckServiceAccountKeyFile();
            FindServiceInstance();
        }

        private void OnGUI()
        {
            InitializeStyles();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawHeader();
            DrawProgressBar();

            EditorGUILayout.Space(10);

            switch (_currentStep)
            {
                case SetupStep.Welcome:
                    DrawWelcomeStep();
                    break;
                case SetupStep.ApiSetup:
                    DrawApiSetupStep();
                    break;
                case SetupStep.ServiceAccountKey:
                    DrawServiceAccountKeyStep();
                    break;
                case SetupStep.ServiceObjectSetup:
                    DrawServiceObjectSetupStep();
                    break;
                case SetupStep.SpreadsheetConfig:
                    DrawSpreadsheetConfigStep();
                    break;
                case SetupStep.Complete:
                    DrawCompleteStep();
                    break;
            }

            EditorGUILayout.Space(10);
            DrawNavigationButtons();

            EditorGUILayout.EndScrollView();
        }

        #region 初期化

        /// <summary>
        /// 初期化
        /// </summary>
        private void InitializeStyles()
        {
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(EditorStyles.largeLabel)
                {
                    fontSize = 18,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    fixedHeight = 30
                };
            }

            if (_warningStyle == null)
            {
                _warningStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    normal = { textColor = Color.yellow }
                };
            }

            if (_successStyle == null)
            {
                _successStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    normal = { textColor = Color.green }
                };
            }
        }

        #endregion

        #region ヘッダー・進捗

        /// <summary>
        /// ヘッダーの描画
        /// </summary>
        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Google Sheets Data Service", _headerStyle);
            GUILayout.Label("セットアップウィザード", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 進捗表示
        /// </summary>
        private void DrawProgressBar()
        {
            EditorGUILayout.BeginHorizontal();

            var stepNames = new[] { "Welcome", "API設定", "認証キー", "サービス", "シート設定", "完了" };
            var currentIndex = (int)_currentStep;

            for (int i = 0; i < stepNames.Length; i++)
            {
                var style = i <= currentIndex ? EditorStyles.miniButtonMid : EditorStyles.miniButton;
                if (i <= currentIndex)
                {
                    GUI.backgroundColor = Color.green;
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                }

                GUILayout.Button(stepNames[i], style);
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region ステップごとの描画

        /// <summary>
        /// ホーム
        /// </summary>
        private void DrawWelcomeStep()
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("Google Sheets Data Serviceへようこそ！", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("このウィザードでは以下のセットアップを行います：", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("• Google Cloud Console プロジェクトの作成", EditorStyles.label);
            EditorGUILayout.LabelField("• Google Sheets API の有効化", EditorStyles.label);
            EditorGUILayout.LabelField("• サービスアカウントキーの設定", EditorStyles.label);
            EditorGUILayout.LabelField("• SheetsDataService オブジェクトの作成", EditorStyles.label);
            EditorGUILayout.LabelField("• スプレッドシートの登録", EditorStyles.label);

            EditorGUILayout.Space();

            // 設定アセットの表示
            EditorGUILayout.LabelField("設定アセット:", EditorStyles.boldLabel);

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Google API Settings", _apiSettings, typeof(GoogleApiSettingsSO), false);
            GUI.enabled = true;

            if (_apiSettings == null)
            {
                EditorGUILayout.HelpBox("設定アセットが見つかりません。", MessageType.Error);

                if (GUILayout.Button("設定アセットを作成"))
                {
                    // アセット作成
                    _apiSettings = GoogleApiSettingsSO.CreateDefaultAsset();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("✓ 設定アセットが準備されました！", MessageType.Info);

                // サービスオブジェクトの事前作成
                if (_serviceInstance == null)
                {
                    EditorGUILayout.Space();
                    if (GUILayout.Button("SheetsDataServiceオブジェクトを事前作成"))
                    {
                        CreateServiceInstance();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("✓ SheetsDataServiceオブジェクトも準備済みです！", MessageType.Info);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("事前にGoogleアカウントが必要です。準備ができたら「次へ」をクリックしてください。", MessageType.Info);

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// API設定
        /// </summary>
        private void DrawApiSetupStep()
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("Google Cloud Console 設定", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("以下の手順でAPIを設定してください：", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            DrawStepBox("1. Google Cloud Consoleにアクセス",
                "https://console.cloud.google.com/ にアクセスしてプロジェクトを作成または選択してください。");

            DrawStepBox("2. Google Sheets API を有効化",
                "「APIとサービス」→「ライブラリ」から「Google Sheets API」を検索し、有効化してください。");

            DrawStepBox("3. サービスアカウントを作成",
                "「APIとサービス」→「認証情報」→「認証情報を作成」→「サービスアカウント」を選択してください。");

            EditorGUILayout.Space();

            if (GUILayout.Button("Google Cloud Console を開く", _buttonStyle))
            {
                Application.OpenURL("https://console.cloud.google.com/");
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// サービスアカウントキーの設定
        /// </summary>
        private void DrawServiceAccountKeyStep()
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("サービスアカウントキーの設定", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawStepBox("1. サービスアカウントキーをダウンロード",
                "作成したサービスアカウントの「キー」タブから、JSONキーをダウンロードしてください。");

            DrawStepBox("2. StreamingAssetsフォルダに配置",
                "ダウンロードしたJSONファイルを「Assets/StreamingAssets/」フォルダに配置してください。");

            EditorGUILayout.Space();

            // StreamingAssetsフォルダの作成ボタン
            if (!Directory.Exists(Path.Combine(Application.dataPath, "StreamingAssets")))
            {
                EditorGUILayout.HelpBox("StreamingAssetsフォルダが存在しません。", MessageType.Warning);
                if (GUILayout.Button("StreamingAssetsフォルダを作成", _buttonStyle))
                {
                    Directory.CreateDirectory(Path.Combine(Application.dataPath, "StreamingAssets"));
                    AssetDatabase.Refresh();
                }

                EditorGUILayout.Space();
            }

            // 設定アセットから値を取得/設定
            if (_apiSettings != null)
            {
                EditorGUILayout.LabelField("認証設定:", EditorStyles.boldLabel);

                // アプリケーション名
                EditorGUILayout.LabelField("アプリケーション名:");
                var newAppName = EditorGUILayout.TextField(_apiSettings.ApplicationName);
                if (newAppName != _apiSettings.ApplicationName)
                {
                    _apiSettings.ApplicationName = newAppName;
                    EditorUtility.SetDirty(_apiSettings);
                }

                // サービスアカウントキーファイル名
                EditorGUILayout.LabelField("サービスアカウントキーファイル名:");
                var newKeyFileName = EditorGUILayout.TextField(_apiSettings.ServiceAccountKeyFileName);
                if (newKeyFileName != _apiSettings.ServiceAccountKeyFileName)
                {
                    _apiSettings.ServiceAccountKeyFileName = newKeyFileName;
                    EditorUtility.SetDirty(_apiSettings);
                    CheckServiceAccountKeyFile(); // ファイル名変更時に再チェック
                }

                EditorGUILayout.Space();

                // ファイル存在チェック
                CheckServiceAccountKeyFile();
                if (_keyFileExists)
                {
                    EditorGUILayout.HelpBox("✓ サービスアカウントキーファイルが見つかりました！", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        $"⚠ ファイルが見つかりません: Assets/StreamingAssets/{_apiSettings.ServiceAccountKeyFileName}",
                        MessageType.Warning);
                }

                if (GUILayout.Button("ファイル存在チェック", _buttonStyle))
                {
                    CheckServiceAccountKeyFile();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("設定アセットが見つかりません。前の手順に戻って作成してください。", MessageType.Error);
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// サービスインスタンスの設定
        /// </summary>
        private void DrawServiceObjectSetupStep()
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("SheetsDataService オブジェクト設定", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (_apiSettings == null)
            {
                EditorGUILayout.HelpBox("設定アセットが見つかりません。前の手順に戻って作成してください。", MessageType.Error);
                EditorGUILayout.EndVertical();
                return;
            }

            // 設定アセットの表示
            EditorGUILayout.LabelField("設定アセット:", EditorStyles.boldLabel);
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Google API Settings", _apiSettings, typeof(GoogleApiSettingsSO), false);
            GUI.enabled = true;

            EditorGUILayout.Space();

            // サービスインスタンスの確認
            FindServiceInstance();

            EditorGUILayout.LabelField("サービスオブジェクト:", EditorStyles.boldLabel);

            if (_serviceInstance == null)
            {
                EditorGUILayout.HelpBox("SheetsDataServiceオブジェクトが見つかりません。", MessageType.Warning);

                EditorGUILayout.Space();
                if (GUILayout.Button("SheetsDataServiceオブジェクトを作成", _buttonStyle))
                {
                    CreateServiceInstance();
                }

                // レイアウト統一のための空白領域
                EditorGUILayout.Space(60);
            }
            else
            {
                EditorGUILayout.HelpBox("✓ SheetsDataServiceオブジェクトが見つかりました！", MessageType.Info);

                // ApiSettingsが正しく設定されているかチェック
                if (_serviceInstance.ApiSettings != _apiSettings)
                {
                    EditorGUILayout.HelpBox("ApiSettingsが設定されていないか、異なる設定が適用されています。", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("✓ ApiSettingsも正しく設定されています！", MessageType.Info);
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("設定を適用", _buttonStyle))
                {
                    ApplySettingsToService();
                }
            }

            EditorGUILayout.Space();

            // 詳細情報セクション（サービスが存在する場合のみ表示、折りたたみ可能）
            if (_serviceInstance != null)
            {
                if (EditorGUILayout.Foldout(true, "詳細情報", true))
                {
                    EditorGUI.indentLevel++;

                    // 現在の設定表示
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("Service Instance", _serviceInstance, typeof(SheetsDataService), true);
                    EditorGUILayout.ObjectField("Assigned ApiSettings", _serviceInstance.ApiSettings,
                        typeof(GoogleApiSettingsSO), false);
                    GUI.enabled = true;

                    // 設定済みスプレッドシート数
                    var configCount = _serviceInstance.SpreadsheetConfigs?.Count ?? 0;
                    EditorGUILayout.LabelField($"登録済みスプレッドシート: {configCount} 件");

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// スプレッドシートの登録
        /// </summary>
        private void DrawSpreadsheetConfigStep()
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("スプレッドシート設定", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (_apiSettings == null)
            {
                EditorGUILayout.HelpBox("設定アセットが見つかりません。前の手順に戻って作成してください。", MessageType.Error);
                EditorGUILayout.EndVertical();
                return;
            }

            if (_serviceInstance == null)
            {
                EditorGUILayout.HelpBox("SheetsDataServiceオブジェクトが見つかりません。前の手順に戻って作成してください。", MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            // スプレッドシート追加セクション
            EditorGUILayout.BeginVertical("box");

            // 入力フィールド
            EditorGUILayout.LabelField("名前:");
            _newSpreadsheetName = EditorGUILayout.TextField(_newSpreadsheetName);

            EditorGUILayout.LabelField("スプレッドシートID:");
            _newSpreadsheetId = EditorGUILayout.TextField(_newSpreadsheetId);

            EditorGUILayout.LabelField("説明（任意）:");
            _newSpreadsheetDescription = EditorGUILayout.TextField(_newSpreadsheetDescription);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("スプレッドシートIDは、URLの「/spreadsheets/d/」と「/edit」の間の文字列です。", MessageType.Info);
            EditorGUILayout.Space();

            // ボタン群（Horizontalレイアウトを改善）
            EditorGUILayout.BeginHorizontal();

            bool canAdd = !string.IsNullOrEmpty(_newSpreadsheetName) && !string.IsNullOrEmpty(_newSpreadsheetId);
            GUI.enabled = canAdd;

            if (GUILayout.Button("追加", GUILayout.Height(25)))
            {
                try
                {
                    _serviceInstance.AddSpreadsheetConfig(_newSpreadsheetName, _newSpreadsheetId,
                        _newSpreadsheetDescription);

                    // 入力フィールドをクリア
                    _newSpreadsheetName = "";
                    _newSpreadsheetId = "";
                    _newSpreadsheetDescription = "";

                    // サービスに設定を反映
                    ApplySettingsToService();

                    // ウィンドウを再描画
                    Repaint();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"スプレッドシート追加エラー: {e.Message}");
                }
            }

            GUI.enabled = true;

            if (GUILayout.Button("クリア", GUILayout.Height(25)))
            {
                _newSpreadsheetName = "";
                _newSpreadsheetId = "";
                _newSpreadsheetDescription = "";
                GUI.FocusControl(null); // フォーカスをクリア
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // === 登録済みスプレッドシート一覧セクション ===
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("登録済みスプレッドシート:", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            var configs = _serviceInstance.SpreadsheetConfigs;
            if (configs != null && configs.Count > 0)
            {
                // スクロール可能なリスト表示
                var listHeight = Mathf.Min(configs.Count * 60, 200); // 最大高さを制限
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(listHeight));

                for (int i = 0; i < configs.Count; i++)
                {
                    var config = configs[i];

                    EditorGUILayout.BeginHorizontal("box");

                    // スプレッドシート情報
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(config.Name ?? "名前なし", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"ID: {config.SpreadsheetId ?? "IDなし"}", EditorStyles.miniLabel);
                    if (!string.IsNullOrEmpty(config.Description))
                    {
                        EditorGUILayout.LabelField(config.Description, EditorStyles.miniLabel);
                    }

                    EditorGUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    // 削除ボタン
                    if (GUILayout.Button("削除", GUILayout.Width(60), GUILayout.Height(40)))
                    {
                        if (EditorUtility.DisplayDialog("確認", $"'{config.Name}' を削除しますか？", "はい", "いいえ"))
                        {
                            try
                            {
                                _serviceInstance.RemoveSpreadsheetConfig(config.Name);
                                ApplySettingsToService();
                                Repaint(); // リスト更新後に再描画
                                break; // ループを抜けてリストの変更に対応
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError($"スプレッドシート削除エラー: {e.Message}");
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    if (i < configs.Count - 1)
                    {
                        EditorGUILayout.Space(5);
                    }
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("登録されたスプレッドシートがありません。", MessageType.Info);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // === 接続テストセクション ===
            EditorGUILayout.BeginVertical("box");

            if (configs != null && configs.Count > 0)
            {
                EditorGUILayout.LabelField("接続テスト:", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                if (GUILayout.Button("接続テストを実行", _buttonStyle))
                {
                    TestServiceInitialization();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("スプレッドシートを最低1つ追加してからテストを実行してください。", MessageType.Info);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// コンプリート
        /// </summary>
        private void DrawCompleteStep()
        {
            EditorGUILayout.BeginVertical("box");
            
            GUILayout.Label("セットアップ完了！", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox("✓ Google Sheets Data Service のセットアップが完了しました！", MessageType.Info);
            
            EditorGUILayout.Space();
            
            // シーンへの配置状況確認
            var sceneInstance = FindObjectOfType<SheetsDataService>();
            if (sceneInstance == null)
            {
                EditorGUILayout.HelpBox("現在のシーンにSheetsDataServiceオブジェクトがありません。", MessageType.Warning);
                
                if (GUILayout.Button("現在のシーンに配置", _buttonStyle))
                {
                    PlaceServiceInCurrentScene();
                }
                
                EditorGUILayout.Space();
            }
            else
            {
                EditorGUILayout.HelpBox("✓ 現在のシーンにSheetsDataServiceオブジェクトが配置されています！", MessageType.Info);
                
                if (GUILayout.Button("シーン内のオブジェクトを選択", _buttonStyle))
                {
                    Selection.activeGameObject = sceneInstance.gameObject;
                    EditorGUIUtility.PingObject(sceneInstance.gameObject);
                }
                
                EditorGUILayout.Space();
            }
            
            EditorGUILayout.LabelField("次のステップ:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• スプレッドシートにサービスアカウントの閲覧権限を付与", EditorStyles.label);
            EditorGUILayout.LabelField("• コードからReadFromSpreadsheetAsyncを呼び出してテスト", EditorStyles.label);
            EditorGUILayout.LabelField("• SheetsDataService Windowから詳細な管理", EditorStyles.label);
            
            EditorGUILayout.Space();
            
            // 権限設定の詳細ガイド
            if (EditorGUILayout.Foldout(true, "権限設定の詳細手順", true))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox(
                    "1. Google Sheetsでスプレッドシートを開く\n" +
                    "2. 右上の「共有」ボタンをクリック\n" +
                    "3. サービスアカウントのメールアドレスを追加\n" +
                    "4. 権限を「閲覧者」に設定して完了", 
                    MessageType.Info);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("接続テストを実行", _buttonStyle))
            {
                var testInstance = sceneInstance ?? _serviceInstance;
                if (testInstance != null)
                {
                    testInstance.TestConnection();
                }
                else
                {
                    Debug.LogWarning("テスト用のSheetsDataServiceインスタンスが見つかりません");
                }
            }
            
            if (GUILayout.Button("セットアップを再実行", _buttonStyle))
            {
                _currentStep = SetupStep.Welcome;
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        
        #endregion

        #region パーツ
        
        /// <summary>
        /// 説明用のフィールドを表示
        /// </summary>
        private void DrawStepBox(string title, string description)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// 次へ/戻るのボタンを表示する
        /// </summary>
        private void DrawNavigationButtons()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = _currentStep != SetupStep.Welcome;
            if (GUILayout.Button("← 戻る", _buttonStyle))
            {
                _currentStep--;
            }
            
            GUILayout.FlexibleSpace();
            
            GUI.enabled = CanProceedToNextStep();
            if (_currentStep != SetupStep.Complete)
            {
                if (GUILayout.Button("次へ →", _buttonStyle))
                {
                    _currentStep++;
                }
            }
            
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
        
        #endregion

        #region Helper Methods

        private bool CanProceedToNextStep()
        {
            switch (_currentStep)
            {
                case SetupStep.Welcome:
                    return _apiSettings != null;
                    
                case SetupStep.ApiSetup:
                    return true;
                    
                case SetupStep.ServiceAccountKey:
                    return _keyFileExists && _apiSettings != null;
                    
                case SetupStep.ServiceObjectSetup:
                    return _serviceInstance != null && _apiSettings != null;
                    
                case SetupStep.SpreadsheetConfig:
                    return true;
                    
                default:
                    return false;
            }
        }

        /// <summary>
        /// サービスアカウントのキーファイルの存在を確認する
        /// </summary>
        private void CheckServiceAccountKeyFile()
        {
            if (_apiSettings != null)
            {
                string keyFilePath = Path.Combine(Application.streamingAssetsPath, _apiSettings.ServiceAccountKeyFileName);
                _keyFileExists = File.Exists(keyFilePath);
            }
            else
            {
                _keyFileExists = false;
            }
        }

        /// <summary>
        /// サービスのインスタンスが存在しているか検索
        /// </summary>
        private void FindServiceInstance()
        {
            _serviceInstance = FindObjectOfType<SheetsDataService>();
        }

        /// <summary>
        /// SheetsDataServiceオブジェクトを生成する
        /// </summary>
        private void CreateServiceInstance()
        {
            GameObject go = new GameObject("SheetsDataService");
            _serviceInstance = go.AddComponent<SheetsDataService>();
            
            // 設定を反映（ApiSettingsも含む）
            ApplySettingsToService();
            
            Debug.Log("SheetsDataService オブジェクトを作成しました");
        }

        /// <summary>
        /// 設定をサービスに適用
        /// </summary>
        private void ApplySettingsToService()
        {
            if (_serviceInstance == null || _apiSettings == null) return;
    
            // SheetsDataServiceにApiSettingsを設定
            _serviceInstance.ApiSettings = _apiSettings;
            
            // GoogleApiSettingsSOの参照をSheetsDataServiceに設定
            var apiSettingsField = typeof(SheetsDataService).GetField("_apiSettings", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            apiSettingsField?.SetValue(_serviceInstance, _apiSettings);
    
            // 設定をUnityエディタに反映
            EditorUtility.SetDirty(_serviceInstance);
    
            Debug.Log("設定をSheetsDataServiceに適用しました（ApiSettings含む）");
        }

        /// <summary>
        /// サービスのインスタンスをシーンに移動する
        /// </summary>
        private void PlaceServiceInCurrentScene()
        {
            // 既存のDontDestroyOnLoadインスタンスがあるか確認
            var existingInstance = SheetsDataService.Instance;
            
            if (existingInstance != null)
            {
                // DontDestroyOnLoadのインスタンスをシーンに移動
                var go = existingInstance.gameObject;
                
                // DontDestroyOnLoadを解除するために、親を一時的に設定してからnullに戻す
                var tempParent = new GameObject("Temp");
                go.transform.SetParent(tempParent.transform);
                go.transform.SetParent(null);
                DestroyImmediate(tempParent);
                
                // オブジェクト名を分かりやすく変更
                go.name = "SheetsDataService";
                
                Debug.Log("既存のSheetsDataServiceを現在のシーンに移動しました");
            }
            else
            {
                // 新しくシーンにインスタンスを作成
                GameObject go = new GameObject("SheetsDataService");
                var service = go.AddComponent<SheetsDataService>();
                
                // 設定を反映
                UpdateServiceInstanceSettings(service);
                
                Debug.Log("新しいSheetsDataServiceを現在のシーンに作成しました");
            }
            
            // 作成/移動後にインスタンスを更新
            _serviceInstance = FindObjectOfType<SheetsDataService>();
            
            // シーンを保存するか確認
            if (EditorUtility.DisplayDialog("シーン保存", 
                "SheetsDataServiceオブジェクトをシーンに追加しました。\nシーンを保存しますか？", 
                "保存", "後で"))
            {
                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
        }

        private void UpdateServiceInstanceSettings(SheetsDataService instance)
        {
            if (instance == null || _apiSettings == null) return;
    
            // ApiSettingsを設定
            instance.ApiSettings = _apiSettings;
    
            // エディタに変更を通知
            EditorUtility.SetDirty(instance);
        }

        private async void TestServiceInitialization()
        {
            if (_serviceInstance == null) return;
            
            try
            {
                Debug.Log("サービス初期化テスト開始...");
                
                // プライベートメソッドの呼び出し（テスト用）
                var initMethod = typeof(SheetsDataService).GetMethod("InitializeAsync", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                if (initMethod != null)
                {
                    await (Cysharp.Threading.Tasks.UniTask)initMethod.Invoke(_serviceInstance, null);
                    Debug.Log("✓ 初期化テスト成功");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"初期化テストエラー: {e.Message}");
            }
        }

        #endregion
    }
}
#endif