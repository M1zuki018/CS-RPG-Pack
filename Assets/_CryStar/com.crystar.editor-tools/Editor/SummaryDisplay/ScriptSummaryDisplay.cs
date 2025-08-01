#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CryStar.Editor
{
    /// <summary>
    /// プロジェクトウィンドウにSummaryの内容を表示する
    /// </summary>
    [InitializeOnLoad]
    public class ScriptSummaryDisplay
    {
        // キャッシュ用の辞書（GUID -> Summary）
        private static readonly Dictionary<string, CachedSummary> _summaryCache = new Dictionary<string, CachedSummary>();
        
        /// <summary>
        /// キャッシュエントリの構造体
        /// </summary>
        private struct CachedSummary
        {
            /// <summary>
            /// Summaryの文字列
            /// </summary>
            public string Summary;
            
            /// <summary>
            /// ファイルの最終更新時間
            /// NOTE: ファイルが編集されたことを検知してキャッシュを更新するための変数
            /// </summary>
            public long LastWriteTime;
        }

        static ScriptSummaryDisplay()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
            
            // エディタ再生時などにキャッシュをクリア
            AssemblyReloadEvents.beforeAssemblyReload += ClearCache;
        }

        private static void ClearCache()
        {
            _summaryCache.Clear();
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            
            if (!assetPath.EndsWith(".cs"))
            {
                // スクリプトファイルのみ適用
                return;
            }
            
            if (selectionRect.width < 100)
            {
                // プロジェクトウィンドウがリストビューで表示されているかチェック
                // 横幅が狭ければアイコン表示とみなし、スキップ
                return;
            }

            // Summary コメントを取得（キャッシュ付き）
            string summary = GetCachedSummary(guid, assetPath);
            if (string.IsNullOrEmpty(summary))
                return;

            // ファイル名の描画幅を計算
            GUIStyle style = EditorStyles.label;
            string fileName = Path.GetFileName(assetPath);
            Vector2 fileNameSize = style.CalcSize(new GUIContent(fileName));

            // 表示可能な最大幅を計算
            float maxWidth = selectionRect.width - fileNameSize.x - 20; // 余白を考慮
            if (maxWidth <= 0)
                return;

            // 文字列を幅に合わせて切り詰め
            string displayText = TruncateText(summary, style, maxWidth);

            // ファイル名の右側にラベルを表示
            Rect labelRect = new Rect(
                selectionRect.x + fileNameSize.x + 10,
                selectionRect.y,
                maxWidth,
                selectionRect.height
            );

            // 薄いグレーで表示
            Color originalColor = GUI.color;
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            GUI.Label(labelRect, displayText, style);
            GUI.color = originalColor;
        }

        /// <summary>
        /// キャッシュを使用してSummaryを取得
        /// </summary>
        private static string GetCachedSummary(string guid, string assetPath)
        {
            // ファイルの最終更新時間を取得
            long currentWriteTime = 0;
            if (File.Exists(assetPath))
            {
                currentWriteTime = File.GetLastWriteTime(assetPath).Ticks;
            }

            // キャッシュに存在し、更新時間が同じ場合はキャッシュを返す
            if (_summaryCache.TryGetValue(guid, out CachedSummary cached))
            {
                if (cached.LastWriteTime == currentWriteTime)
                {
                    return cached.Summary;
                }
            }

            // キャッシュにないか、ファイルが更新されている場合は新しく取得
            string summary = ScriptSummaryExtractor.GetFormattedSummary(assetPath);
            
            // キャッシュに保存
            _summaryCache[guid] = new CachedSummary
            {
                Summary = summary,
                LastWriteTime = currentWriteTime
            };

            return summary;
        }

        /// <summary>
        /// 指定された幅に収まるように文字列を切り詰める
        /// </summary>
        private static string TruncateText(string text, GUIStyle style, float maxWidth)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // 全体が収まる場合はそのまま返す
            Vector2 fullSize = style.CalcSize(new GUIContent(text));
            if (fullSize.x <= maxWidth)
                return text;

            // バイナリサーチで最適な長さを見つける
            int left = 0;
            int right = text.Length;
            string ellipsis = "...";
            Vector2 ellipsisSize = style.CalcSize(new GUIContent(ellipsis));

            while (left < right)
            {
                int mid = (left + right + 1) / 2;
                string truncated = text.Substring(0, mid) + ellipsis;
                Vector2 size = style.CalcSize(new GUIContent(truncated));

                if (size.x <= maxWidth)
                {
                    left = mid;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return left > 0 ? text.Substring(0, left) + ellipsis : ellipsis;
        }
    }
}
#endif