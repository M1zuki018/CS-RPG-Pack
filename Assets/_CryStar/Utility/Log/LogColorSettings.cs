using System.Collections.Generic;
using CryStar.Utility.Enum;
using UnityEngine;

namespace CryStar.Utility
{
    /// <summary>
    /// ログ色設定管理クラス
    /// </summary>
    public static class LogColorSettings
    {
        // デフォルトカラー設定
        private static readonly Dictionary<LogLevel, Color> DefaultLevelColors = new()
        {
            { LogLevel.Verbose, new Color(0.7f, 0.7f, 0.7f, 1f) }, // Gray
            { LogLevel.Debug, new Color(1f, 1f, 1f, 1f) }, // White
            { LogLevel.Info, new Color(0f, 1f, 1f, 1f) }, // Cyan
            { LogLevel.Warning, new Color(1f, 1f, 0f, 1f) }, // Yellow
            { LogLevel.Error, new Color(1f, 0f, 0f, 1f) }, // Red
            { LogLevel.Fatal, new Color(1f, 0f, 1f, 1f) } // Magenta
        };

        private static readonly Dictionary<LogCategory, Color> DefaultCategoryColors = new()
        {
            { LogCategory.General, new Color(1f, 1f, 1f, 1f) },         // White
            { LogCategory.System, new Color(0f, 1f, 0f, 1f) },          // Green
            { LogCategory.Gameplay, new Color(0f, 0.5f, 1f, 1f) },      // Blue
            { LogCategory.UI, new Color(1f, 0.5f, 0f, 1f) },            // Orange
            { LogCategory.Audio, new Color(1f, 0f, 1f, 1f) },           // Magenta
            { LogCategory.Network, new Color(1f, 0.5f, 0f, 1f) },       // Orange
            { LogCategory.Performance, new Color(0f, 1f, 0f, 1f) },     // Green
            { LogCategory.Test, new Color(1f, 1f, 0f, 1f) },            // Yellow
            { LogCategory.Debug, new Color(0.5f, 0.5f, 1f, 1f) }        // Light Blue
        };

        // 現在の色設定（実行時に変更可能）
        private static Dictionary<LogLevel, Color> _currentLevelColors;
        private static Dictionary<LogCategory, Color> _currentCategoryColors;

        // プロパティ
        public static Dictionary<LogLevel, Color> LevelColors
        {
            get
            {
                _currentLevelColors ??= new Dictionary<LogLevel, Color>(DefaultLevelColors);
                return _currentLevelColors;
            }
        }

        public static Dictionary<LogCategory, Color> CategoryColors
        {
            get
            {
                _currentCategoryColors ??= new Dictionary<LogCategory, Color>(DefaultCategoryColors);
                return _currentCategoryColors;
            }
        }

        /// <summary>
        /// 色をHTML色文字列に変換
        /// </summary>
        public static string ColorToHtml(Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }

        /// <summary>
        /// ログレベルの色をHTML文字列で取得
        /// </summary>
        public static string GetLevelColorHtml(LogLevel level)
        {
            return ColorToHtml(LevelColors.GetValueOrDefault(level, Color.white));
        }

        /// <summary>
        /// ログカテゴリの色をHTML文字列で取得
        /// </summary>
        public static string GetCategoryColorHtml(LogCategory category)
        {
            return ColorToHtml(CategoryColors.GetValueOrDefault(category, Color.white));
        }
    }
}