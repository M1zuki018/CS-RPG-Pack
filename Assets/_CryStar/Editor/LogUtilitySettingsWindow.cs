using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using CryStar.Utility;
using CryStar.Utility.Enum;

namespace iCON.Utility.Editor
{
    /// <summary>
    /// LogUtilityの設定を管理するエディター拡張ウィンドウ
    /// </summary>
    public class LogUtilitySettingsWindow : EditorWindow
    {
        #region Private Fields
        private SerializedObject _logSettingsObject;
        private LogSettings _logSettings;

        // Serialized Properties
        private SerializedProperty _minLogLevelProp;
        private SerializedProperty _isTimestampEnabledProp;
        private SerializedProperty _isStackTraceEnabledProp;
        private SerializedProperty _isFileLoggingEnabledProp;
        private SerializedProperty _isPerformanceLoggingEnabledProp;
        private SerializedProperty _useCustomColorsProp;
        private SerializedProperty _categorySettingsProp;
        private SerializedProperty _levelColorSettingsProp;
        private SerializedProperty _categoryColorSettingsProp;

        private Vector2 _scrollPosition;
        private bool _showBasicSettings = true;
        private bool _showCategorySettings = true;
        private bool _showColorSettings = false;
        private bool _showFileSettings = false;
        private bool _showTestSection = false;

        // テスト用の一時的な値
        private string _testMessage = "Test log message";
        private LogLevel _testLogLevel = LogLevel.Info;
        private LogCategory _testCategory = LogCategory.Debug;

        // UI設定
        private const float LABEL_WIDTH = 180f;
        private const float BUTTON_HEIGHT = 25f;
        private const float SECTION_SPACING = 10f;
        #endregion

        #region Unity Menu Integration
        [MenuItem("Tools/iCON/Log Utility Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<LogUtilitySettingsWindow>("Log Settings");
            window.minSize = new Vector2(450f, 500f);
            window.Show();
        }
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            titleContent = new GUIContent("Log Settings", EditorGUIUtility.IconContent("console.infoicon").image);
            
            // LogSettingsアセットをロードしてSerializedObjectを作成
            _logSettings = LogSettings.Instance;
            if (_logSettings != null)
            {
                _logSettingsObject = new SerializedObject(_logSettings);
                FindSerializedProperties();
            }
        }

        private void OnGUI()
        {
            if (_logSettingsObject == null || _logSettings == null)
            {
                EditorGUILayout.HelpBox("LogSettings asset not found in a 'Resources' folder. Please create one via 'Create > CryStar > Log Settings'.", MessageType.Error);
                if (GUILayout.Button("Try to Reload"))
                {
                    OnEnable(); // 再読み込みを試みる
                }
                return;
            }

            // SerializedObjectの更新を開始
            _logSettingsObject.Update();

            EditorGUILayout.Space(5f);
            DrawHeader();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawBasicSettings();
            DrawCategorySettings();
            DrawColorSettings();
            DrawFileSettings();
            DrawTestSection();
            DrawPresetSettings();

            EditorGUILayout.EndScrollView();
            DrawFooter();

            // 変更を適用
            if (_logSettingsObject.ApplyModifiedProperties())
            {
                // Optional: Force save of the asset to disk
                EditorUtility.SetDirty(_logSettings);
            }
        }
        #endregion

        private void FindSerializedProperties()
        {
            _minLogLevelProp = _logSettingsObject.FindProperty("MinLogLevel");
            _isTimestampEnabledProp = _logSettingsObject.FindProperty("IsTimestampEnabled");
            _isStackTraceEnabledProp = _logSettingsObject.FindProperty("IsStackTraceEnabled");
            _isFileLoggingEnabledProp = _logSettingsObject.FindProperty("IsFileLoggingEnabled");
            _isPerformanceLoggingEnabledProp = _logSettingsObject.FindProperty("IsPerformanceLoggingEnabled");
            _useCustomColorsProp = _logSettingsObject.FindProperty("UseCustomColors");
            _categorySettingsProp = _logSettingsObject.FindProperty("CategorySettings");
            _levelColorSettingsProp = _logSettingsObject.FindProperty("LevelColorSettings");
            _categoryColorSettingsProp = _logSettingsObject.FindProperty("CategoryColorSettings");
        }

        #region UI Drawing Methods
        private void DrawHeader()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.LabelField("🛠️ LogUtility Settings", titleStyle);
            EditorGUILayout.Space(3f);
            GUIStyle statusStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Application.isPlaying ? Color.green : Color.gray } };
            string status = Application.isPlaying ? "● Runtime Active" : "○ Editor Only";
            EditorGUILayout.LabelField(status, statusStyle);
            GUILayout.EndVertical();
            EditorGUILayout.Space(SECTION_SPACING);
        }

        private void DrawBasicSettings()
        {
            DrawSectionHeader("🔧 Basic Settings", ref _showBasicSettings);
            if (_showBasicSettings)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(_minLogLevelProp, new GUIContent("Minimum Log Level"));
                EditorGUILayout.PropertyField(_isTimestampEnabledProp, new GUIContent("Enable Timestamp"));
                EditorGUILayout.PropertyField(_isStackTraceEnabledProp, new GUIContent("Enable Stack Trace"));
                EditorGUILayout.PropertyField(_isPerformanceLoggingEnabledProp, new GUIContent("Performance Logging"));
                GUILayout.EndVertical();
            }
            EditorGUILayout.Space(SECTION_SPACING);
        }

        private void DrawCategorySettings()
        {
            DrawSectionHeader("📂 Category Settings", ref _showCategorySettings);
            if (_showCategorySettings)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Enable All", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    SetAllCategoriesEnabled(true);
                }
                if (GUILayout.Button("Disable All", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    SetAllCategoriesEnabled(false);
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(5f);

                // 各カテゴリの設定
                for (int i = 0; i < _categorySettingsProp.arraySize; i++)
                {
                    SerializedProperty categorySettingProp = _categorySettingsProp.GetArrayElementAtIndex(i);
                    SerializedProperty categoryProp = categorySettingProp.FindPropertyRelative("Category");
                    SerializedProperty isEnabledProp = categorySettingProp.FindPropertyRelative("IsEnabled");

                    var category = (LogCategory)Enum.GetValues(typeof(LogCategory)).GetValue(categoryProp.enumValueIndex);
                    
                    EditorGUILayout.BeginHorizontal();
                    string icon = GetCategoryIcon(category);
                    EditorGUILayout.LabelField($"{icon} {category}", GUILayout.Width(LABEL_WIDTH));
                    EditorGUILayout.PropertyField(isEnabledProp, GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }
                
                GUILayout.EndVertical();
            }
            EditorGUILayout.Space(SECTION_SPACING);
        }
        
        private void DrawColorSettings()
        {
            DrawSectionHeader("🎨 Color Settings", ref _showColorSettings);
            if (_showColorSettings)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.PropertyField(_useCustomColorsProp, new GUIContent("Use Custom Colors"));
                
                if (_useCustomColorsProp.boolValue)
                {
                    EditorGUILayout.Space(8f);
                    EditorGUILayout.LabelField("📊 Log Level Colors:", EditorStyles.boldLabel);
                    DrawColorList(_levelColorSettingsProp, true);
                    
                    EditorGUILayout.Space(8f);
                    EditorGUILayout.LabelField("📂 Log Category Colors:", EditorStyles.boldLabel);
                    DrawColorList(_categoryColorSettingsProp, false);
                }
                
                GUILayout.EndVertical();
            }
            EditorGUILayout.Space(SECTION_SPACING);
        }

        private void DrawColorList(SerializedProperty listProp, bool isLevel)
        {
            for (int i = 0; i < listProp.arraySize; i++)
            {
                SerializedProperty itemProp = listProp.GetArrayElementAtIndex(i);
                SerializedProperty colorProp = itemProp.FindPropertyRelative("Color");
                
                string name;
                string icon;
                if (isLevel)
                {
                    var level = (LogLevel)Enum.GetValues(typeof(LogLevel)).GetValue(itemProp.FindPropertyRelative("Level").enumValueIndex);
                    name = level.ToString();
                    icon = GetLevelIcon(level);
                }
                else
                {
                    var category = (LogCategory)Enum.GetValues(typeof(LogCategory)).GetValue(itemProp.FindPropertyRelative("Category").enumValueIndex);
                    name = category.ToString();
                    icon = GetCategoryIcon(category);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{icon} {name}", GUILayout.Width(LABEL_WIDTH));
                EditorGUILayout.PropertyField(colorProp, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawFileSettings()
        {
            DrawSectionHeader("💾 File Logging Settings", ref _showFileSettings);
            if (_showFileSettings)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(_isFileLoggingEnabledProp, new GUIContent("Enable File Logging"));
                
                EditorGUILayout.Space(5f);
                string logPath = Path.Combine(Application.persistentDataPath, "Logs");
                EditorGUILayout.LabelField("Log Directory:", EditorStyles.boldLabel);
                EditorGUILayout.SelectableLabel(logPath, EditorStyles.textField, GUILayout.Height(18f));
                
                EditorGUILayout.Space(3f);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Open Log Folder", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    OpenLogFolder();
                }
                if (GUILayout.Button("Clear Log File", GUILayout.Height(BUTTON_HEIGHT)))
                {
                    if (EditorUtility.DisplayDialog("Clear Log File", "Are you sure you want to clear the current log file?", "Clear", "Cancel"))
                    {
                        LogUtility.ClearLogFile();
                        ShowNotification(new GUIContent("Log file cleared!"));
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            EditorGUILayout.Space(SECTION_SPACING);
        }

        private void DrawTestSection()
        {
            DrawSectionHeader("🧪 Test Log Output", ref _showTestSection);
            if (_showTestSection)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                _testMessage = EditorGUILayout.TextField("Test Message:", _testMessage);
                _testLogLevel = (LogLevel)EditorGUILayout.EnumPopup("Log Level:", _testLogLevel);
                _testCategory = (LogCategory)EditorGUILayout.EnumPopup("Category:", _testCategory);
                
                EditorGUILayout.Space(5f);
                if (GUILayout.Button("🚀 Send Test Log", GUILayout.Height(BUTTON_HEIGHT * 1.2f)))
                {
                    SendTestLog();
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.Space(SECTION_SPACING);
        }

        private void DrawPresetSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("⚙️ Configuration Presets", EditorStyles.boldLabel);
            EditorGUILayout.Space(3f);
            
            if (GUILayout.Button("🔄 Reset to Defaults", GUILayout.Height(BUTTON_HEIGHT)))
            {
                if (EditorUtility.DisplayDialog("Reset Settings", "Are you sure you want to reset all settings to their default values?", "Reset", "Cancel"))
                {
                    _logSettings.ResetToDefaults();
                    ShowNotification(new GUIContent("Settings reset to defaults!"));
                }
            }
            
            GUILayout.EndVertical();
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space(SECTION_SPACING);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUIStyle footerStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.gray } };
            EditorGUILayout.LabelField("LogUtility Settings v2.0 | Changes are saved to LogSettings.asset", footerStyle);
            GUILayout.EndVertical();
        }

        private void DrawSectionHeader(string title, ref bool isExpanded)
        {
            GUIStyle headerStyle = new GUIStyle(EditorStyles.foldout) { fontSize = 12, fontStyle = FontStyle.Bold };
            isExpanded = EditorGUILayout.Foldout(isExpanded, title, headerStyle);
        }
        #endregion

        #region Helper Methods
        private void SetAllCategoriesEnabled(bool enabled)
        {
            for (int i = 0; i < _categorySettingsProp.arraySize; i++)
            {
                _categorySettingsProp.GetArrayElementAtIndex(i).FindPropertyRelative("IsEnabled").boolValue = enabled;
            }
        }

        private string GetCategoryIcon(LogCategory category)
        {
            return category switch
            {
                LogCategory.General => "📝",
                LogCategory.System => "⚙️",
                LogCategory.Gameplay => "🎮",
                LogCategory.UI => "🖼️",
                LogCategory.Audio => "🔊",
                LogCategory.Network => "🌐",
                LogCategory.Performance => "⚡",
                LogCategory.Test => "🧪",
                LogCategory.Debug => "🐛",
                _ => "📄"
            };
        }
        
        private string GetLevelIcon(LogLevel level)
        {
            return level switch
            {
                LogLevel.Fatal => "🔥",
                LogLevel.Error => "❌",
                LogLevel.Warning => "⚠️",
                LogLevel.Info => "ℹ️",
                LogLevel.Debug => "🐞",
                LogLevel.Verbose => "💬",
                _ => "📄"
            };
        }

        private void SendTestLog()
        {
            string message = $"{_testMessage} (Test from Editor)";
            switch (_testLogLevel)
            {
                case LogLevel.Verbose: LogUtility.Verbose(message, _testCategory); break;
                case LogLevel.Debug: LogUtility.Debug(message, _testCategory); break;
                case LogLevel.Info: LogUtility.Info(message, _testCategory); break;
                case LogLevel.Warning: LogUtility.Warning(message, _testCategory); break;
                case LogLevel.Error: LogUtility.Error(message, _testCategory); break;
                case LogLevel.Fatal: LogUtility.Fatal(message, _testCategory); break;
            }
            ShowNotification(new GUIContent($"Sent {_testLogLevel} log!"));
        }

        private void OpenLogFolder()
        {
            string logPath = Path.Combine(Application.persistentDataPath, "Logs");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            EditorUtility.RevealInFinder(logPath);
        }
        #endregion
    }
}
