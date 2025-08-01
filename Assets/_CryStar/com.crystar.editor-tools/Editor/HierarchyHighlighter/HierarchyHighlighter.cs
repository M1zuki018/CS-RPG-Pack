#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CryStar.Editor
{
    /// <summary>
    /// ヒエラルキービューで#Folderで始まるオブジェクトの背景色を変更する
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyHighlighter
    {
        private static List<HighlightRule> _highlightRules = new List<HighlightRule>();

        static HierarchyHighlighter()
        {
            LoadHighlightRules();
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            // 設定されたルールをチェック
            foreach (var rule in _highlightRules)
            {
                if (rule.Enabled && obj.name.StartsWith(rule.Prefix))
                {
                    // 背景色を描画
                    EditorGUI.DrawRect(selectionRect, rule.BackgroundColor);

                    // プレフィックスを除いた名前を表示
                    string displayName = obj.name.Substring(rule.Prefix.Length).Trim();
                    if (string.IsNullOrEmpty(displayName))
                    {
                        displayName = rule.Prefix; // プレフィックスのみの場合はそのまま表示
                    }

                    // テキストスタイルを適用
                    GUIStyle textStyle = new GUIStyle(EditorStyles.label)
                    {
                        normal = { textColor = rule.TextColor },
                        fontStyle = rule.FontStyle
                    };

                    EditorGUI.LabelField(selectionRect, displayName, textStyle);
                    break; // 最初にマッチしたルールのみ適用
                }
            }
        }

        /// <summary>
        /// 設定ウィンドウを開く
        /// </summary>
        [MenuItem("CryStar/Tools/Hierarchy Folder Highlighter")]
        private static void OpenSettings()
        {
            HierarchyHighlighterSettings.ShowWindow();
        }

        /// <summary>
        /// デフォルトルールをリセット
        /// </summary>
        public static void ResetToDefault()
        {
            ResetHighlightRules();
            SaveHighlightRules();
        }

        /// <summary>
        /// ハイライトルールを読み込み
        /// </summary>
        private static void LoadHighlightRules()
        {
            string rulesJson = EditorPrefs.GetString("HierarchyHighlighter_Rules", "");

            if (string.IsNullOrEmpty(rulesJson))
            {
                ResetHighlightRules();
            }
            else
            {
                try
                {
                    var wrapper = JsonUtility.FromJson<HighlightRuleListWrapper>(rulesJson);
                    _highlightRules = wrapper.Rules ?? new List<HighlightRule>();

                    // 空の場合はデフォルト設定
                    if (_highlightRules.Count == 0)
                    {
                        ResetHighlightRules();
                    }
                }
                catch (Exception)
                {
                    Debug.LogWarning("HierarchyHighlighter: 設定の読み込みに失敗しました。デフォルト設定を使用します。");
                    ResetHighlightRules();
                }
            }
        }

        /// <summary>
        /// ハイライトルールを保存
        /// </summary>
        public static void SaveHighlightRules()
        {
            var wrapper = new HighlightRuleListWrapper { Rules = _highlightRules };
            string rulesJson = JsonUtility.ToJson(wrapper, true);
            EditorPrefs.SetString("HierarchyHighlighter_Rules", rulesJson);
        }

        /// <summary>
        /// デフォルトルールに戻す
        /// </summary>
        private static void ResetHighlightRules()
        {
            _highlightRules = new List<HighlightRule>
            {
                new HighlightRule
                {
                    Prefix = "#Folder",
                    BackgroundColor = new Color(0.7f, 0f, 0f, 1f),
                    TextColor = Color.white,
                    FontStyle = FontStyle.Normal,
                    Enabled = true
                },
                new HighlightRule
                {
                    Prefix = "#Section",
                    BackgroundColor = new Color(0f, 0.5f, 0.8f, 1f),
                    TextColor = Color.white,
                    FontStyle = FontStyle.Bold,
                    Enabled = true
                },
                new HighlightRule
                {
                    Prefix = "#Group",
                    BackgroundColor = new Color(0.2f, 0.6f, 0.2f, 1f),
                    TextColor = Color.white,
                    FontStyle = FontStyle.Normal,
                    Enabled = true
                },
                new HighlightRule
                {
                    Prefix = "#UI",
                    BackgroundColor = new Color(0.8f, 0.4f, 0f, 1f),
                    TextColor = Color.white,
                    FontStyle = FontStyle.Italic,
                    Enabled = true
                }
            };
        }

        /// <summary>
        /// 外部からルールリストにアクセス（設定ウィンドウ用）
        /// </summary>
        public static List<HighlightRule> GetHighlightRules()
        {
            return _highlightRules;
        }

        /// <summary>
        /// ルールリストを更新（設定ウィンドウ用）
        /// </summary>
        public static void UpdateHighlightRules(List<HighlightRule> newRules)
        {
            _highlightRules = newRules;
            SaveHighlightRules();
        }
    }
}
#endif