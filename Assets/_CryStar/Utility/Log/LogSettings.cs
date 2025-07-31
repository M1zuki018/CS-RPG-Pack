using System;
using UnityEngine;
using System.Collections.Generic;
using CryStar.Utility.Enum;

namespace CryStar.Utility
{
    /// <summary>
    /// LogUtilityの設定を保持するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "LogSettings", menuName = "CryStar/Create ScriptableObject/Utility/Log Settings")]
    public class LogSettings : ScriptableObject
    {
        #region Singleton Access
        private static LogSettings _instance;
        public static LogSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<LogSettings>("LogSettings");
                    if (_instance == null)
                    {
                        Debug.LogError("LogSettings asset not found in Resources folder. Please create one.");
                        // Create a temporary instance to avoid null reference errors
                        _instance = CreateInstance<LogSettings>();
                        _instance.ResetToDefaults(); // Use default values
                    }
                }
                return _instance;
            }
        }
        #endregion

        [Header("🔧 Basic Settings")]
        [Tooltip("出力する最小ログレベル。これより低いレベルのログは出力されない")]
        public LogLevel MinLogLevel = LogLevel.Debug;

        [Tooltip("ログにタイムスタンプを付与するか")]
        public bool IsTimestampEnabled = true;

        [Tooltip("Warning以上のログでコールスタックを表示するか")]
        public bool IsStackTraceEnabled = false;

        [Header("📂 Category Settings")]
        public List<CategorySetting> CategorySettings = new List<CategorySetting>();

        [Header("🎨 Color Settings")]
        [Tooltip("カスタムカラー設定を使用するか")]
        public bool UseCustomColors = true;
        public List<LevelColorSetting> LevelColorSettings = new List<LevelColorSetting>();
        public List<CategoryColorSetting> CategoryColorSettings = new List<CategoryColorSetting>();

        [Header("💾 File Logging Settings")]
        [Tooltip("ファイルへのログ出力を有効にするか")]
        public bool IsFileLoggingEnabled = false;

        [Header("⚡ Performance Settings")]
        [Tooltip("パフォーマンス測定ログの出力を有効にするか")]
        public bool IsPerformanceLoggingEnabled = true;

        /// <summary>
        /// 設定をデフォルト値にリセットする
        /// </summary>
        [ContextMenu("Reset to Defaults")]
        public void ResetToDefaults()
        {
            MinLogLevel = LogLevel.Debug;
            IsTimestampEnabled = true;
            IsStackTraceEnabled = false;
            IsFileLoggingEnabled = false;
            UseCustomColors = true;
            IsPerformanceLoggingEnabled = true;

            // Categories
            CategorySettings.Clear();
            foreach (LogCategory category in System.Enum.GetValues(typeof(LogCategory)))
            {
                CategorySettings.Add(new CategorySetting { Category = category, IsEnabled = true });
            }

            // Level Colors
            LevelColorSettings.Clear();
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Fatal, Color = new Color(1f, 0.2f, 0.2f) });
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Error, Color = Color.red });
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Warning, Color = Color.yellow });
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Info, Color = new Color(0f, 0.8f, 1f) });
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Debug, Color = Color.white });
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Verbose, Color = Color.gray });

            // Category Colors
            CategoryColorSettings.Clear();
            CategoryColorSettings.Add(new CategoryColorSetting { Category = LogCategory.General, Color = Color.cyan });
            CategoryColorSettings.Add(new CategoryColorSetting { Category = LogCategory.System, Color = Color.green });
            CategoryColorSettings.Add(new CategoryColorSetting { Category = LogCategory.Gameplay, Color = new Color(1f, 0.5f, 0f) });
            CategoryColorSettings.Add(new CategoryColorSetting { Category = LogCategory.UI, Color = Color.magenta });
            CategoryColorSettings.Add(new CategoryColorSetting { Category = LogCategory.Audio, Color = new Color(0.5f, 0.5f, 1f) });
            CategoryColorSettings.Add(new CategoryColorSetting { Category = LogCategory.Network, Color = new Color(0f, 1f, 0.5f) });
            CategoryColorSettings.Add(new CategoryColorSetting { Category = LogCategory.Performance, Color = Color.green });
            CategoryColorSettings.Add(new CategoryColorSetting { Category = LogCategory.Test, Color = Color.cyan });
            CategoryColorSettings.Add(new CategoryColorSetting { Category = LogCategory.Debug, Color = Color.white });
            
            Debug.Log("LogSettings have been reset to default values.");
        }

        #region Helper Methods for Enum Lists
        private void EnsureAllEnumValuesExist<T, TEnum>(List<T> list, System.Func<T> creator) where T : IEnumHolder<TEnum> where TEnum : System.Enum
        {
            foreach (TEnum enumValue in System.Enum.GetValues(typeof(TEnum)))
            {
                if (!list.Exists(item => item.GetEnum().Equals(enumValue)))
                {
                    T newItem = creator();
                    newItem.SetEnum(enumValue);
                    list.Add(newItem);
                }
            }
        }
        #endregion
    }

    #region Helper Classes for Serialization
    public interface IEnumHolder<TEnum> where TEnum : System.Enum
    {
        TEnum GetEnum();
        void SetEnum(TEnum value);
    }

    [Serializable]
    public class CategorySetting : IEnumHolder<LogCategory>
    {
        public LogCategory Category;
        public bool IsEnabled;
        public LogCategory GetEnum() => Category;
        public void SetEnum(LogCategory value) => Category = value;
    }

    [Serializable]
    public class LevelColorSetting : IEnumHolder<LogLevel>
    {
        public LogLevel Level;
        public Color Color = Color.white;
        public LogLevel GetEnum() => Level;
        public void SetEnum(LogLevel value) => Level = value;
    }

    [Serializable]
    public class CategoryColorSetting : IEnumHolder<LogCategory>
    {
        public LogCategory Category;
        public Color Color = Color.white;
        public LogCategory GetEnum() => Category;
        public void SetEnum(LogCategory value) => Category = value;
    }
    #endregion
}
