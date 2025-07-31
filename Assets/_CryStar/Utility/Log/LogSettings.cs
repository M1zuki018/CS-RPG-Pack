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
                        _instance = CreateInstance<LogSettings>();
                        _instance.ResetToDefaults();
                    }
                }
                return _instance;
            }
        }
        #endregion

        [Header("🔧 Basic Settings")]
        public LogLevel MinLogLevel = LogLevel.Debug;
        public bool IsTimestampEnabled = true;
        public bool IsStackTraceEnabled = false;

        [Header("📂 Category Settings")]
        public List<CategorySetting> CategorySettings = new List<CategorySetting>();

        [Header("🎨 Color Settings")]
        public bool UseCustomColors = true;
        public List<LevelColorSetting> LevelColorSettings = new List<LevelColorSetting>();
        public List<CategoryColorSetting> CategoryColorSettings = new List<CategoryColorSetting>();

        [Header("💾 File Logging Settings")]
        public bool IsFileLoggingEnabled = false;

        [Header("⚡ Performance Settings")]
        public bool IsPerformanceLoggingEnabled = true;

        /// <summary>
        /// カラーテーマを適用
        /// </summary>
        public void ApplyColorTheme(ColorTheme theme)
        {
            switch (theme)
            {
                case ColorTheme.Default:
                    ResetToDefaults(false);
                    break;
                case ColorTheme.Vivid:
                    ApplyVividTheme();
                    break;
                case ColorTheme.Light:
                    ApplyPastelTheme();
                    break;
                case ColorTheme.Monochrome:
                    ApplyMonochromeTheme();
                    break;
            }
            Debug.Log($"{theme} color theme applied.");
        }

        /// <summary>
        /// 設定をデフォルト値にリセットする
        /// </summary>
        [ContextMenu("Reset to Defaults")]
        public void ResetToDefaults(bool resetAll = true)
        {
            if (resetAll)
            {
                MinLogLevel = LogLevel.Debug;
                IsTimestampEnabled = true;
                IsStackTraceEnabled = false;
                IsFileLoggingEnabled = false;
                UseCustomColors = true;
                IsPerformanceLoggingEnabled = true;

                CategorySettings.Clear();
                foreach (LogCategory category in System.Enum.GetValues(typeof(LogCategory)))
                {
                    CategorySettings.Add(new CategorySetting { Category = category, IsEnabled = true });
                }
            }

            // Default Colors
            LevelColorSettings.Clear();
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Fatal, Color = new Color(1f, 0.2f, 0.2f) });
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Error, Color = Color.red });
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Warning, Color = Color.yellow });
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Info, Color = new Color(0f, 0.8f, 1f) });
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Debug, Color = Color.white });
            LevelColorSettings.Add(new LevelColorSetting { Level = LogLevel.Verbose, Color = Color.gray });

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
            
            if (resetAll) Debug.Log("LogSettings have been reset to default values.");
        }

        #region Theme Implementations
        private void ApplyVividTheme()
        {
            SetLevelColor(LogLevel.Fatal, new Color(1f, 0f, 0f));
            SetLevelColor(LogLevel.Error, new Color(1f, 0.2f, 0f));
            SetLevelColor(LogLevel.Warning, new Color(1f, 1f, 0f));
            SetLevelColor(LogLevel.Info, new Color(0f, 1f, 1f));
            SetLevelColor(LogLevel.Debug, new Color(1f, 1f, 1f));
            SetLevelColor(LogLevel.Verbose, new Color(0.7f, 0.7f, 0.7f));
        }

        private void ApplyPastelTheme()
        {
            SetLevelColor(LogLevel.Fatal, new Color(1f, 0.2f, 0.6f, 1f)); 
            SetLevelColor(LogLevel.Error, new Color(1f, 0.4f, 0.4f, 1f));
            SetLevelColor(LogLevel.Warning, new Color(1f, 0.7f, 0.3f, 1f));
            SetLevelColor(LogLevel.Info, new Color(0.5f, 0.8f, 1f, 1f));
            SetLevelColor(LogLevel.Debug, new Color(0.5f, 1f, 0.5f, 1f));
            SetLevelColor(LogLevel.Verbose, new Color(0.7f, 0.5f, 1f, 1f));
        }

        private void ApplyMonochromeTheme()
        {
            SetLevelColor(LogLevel.Fatal, new Color(1f, 1f, 1f));
            SetLevelColor(LogLevel.Error, new Color(0.9f, 0.9f, 0.9f));
            SetLevelColor(LogLevel.Warning, new Color(0.8f, 0.8f, 0.8f));
            SetLevelColor(LogLevel.Info, new Color(0.7f, 0.7f, 0.7f));
            SetLevelColor(LogLevel.Debug, new Color(0.6f, 0.6f, 0.6f));
            SetLevelColor(LogLevel.Verbose, new Color(0.5f, 0.5f, 0.5f));
        }

        private void SetLevelColor(LogLevel level, Color color)
        {
            var setting = LevelColorSettings.Find(s => s.Level == level);
            if (setting != null) setting.Color = color;
        }
        #endregion

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