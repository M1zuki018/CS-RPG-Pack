#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CryStar.Editor
{
    /// <summary>
    /// ハイライトのルールを設定するためのウィンドウ
    /// </summary>
    public class HierarchyHighlighterSettings : EditorWindow
    {
        private List<HighlightRule> _rules;
        private Vector2 _scrollPosition;

        public static void ShowWindow()
        {
            var window = GetWindow<HierarchyHighlighterSettings>("Hierarchy Highlighter Settings");
            window.Show();
        }

        private void OnEnable()
        {
            _rules = new List<HighlightRule>(HierarchyHighlighter.GetHighlightRules());
        }

        private void OnGUI()
        {
            GUILayout.Label("Hierarchy Folder Highlighter Settings", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "ヒエラルキービューでのフォルダハイライト設定です。\n" +
                "指定したプレフィックスで始まるGameObjectの背景色とテキストスタイルを変更できます。",
                MessageType.Info
            );

            GUILayout.Space(10);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            for (int i = 0; i < _rules.Count; i++)
            {
                DrawRuleGUI(i);
                GUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            // ボタン類
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("新しいルールを追加"))
            {
                _rules.Add(new HighlightRule());
            }

            if (GUILayout.Button("デフォルトに戻す"))
            {
                if (EditorUtility.DisplayDialog("確認", "設定をデフォルトに戻しますか？", "はい", "いいえ"))
                {
                    // デフォルトルールを読み込み直す
                    HierarchyHighlighter.ResetToDefault();
                    _rules = new List<HighlightRule>(HierarchyHighlighter.GetHighlightRules());
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 保存ボタン
            if (GUILayout.Button("設定を保存", GUILayout.Height(30)))
            {
                HierarchyHighlighter.UpdateHighlightRules(_rules);
                EditorUtility.DisplayDialog("保存完了", "設定が保存されました。", "OK");
            }
        }

        private void DrawRuleGUI(int index)
        {
            var rule = _rules[index];

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            rule.Enabled = EditorGUILayout.Toggle(rule.Enabled, GUILayout.Width(20));
            EditorGUILayout.LabelField($"ルール {index + 1}", EditorStyles.boldLabel);

            if (GUILayout.Button("削除", GUILayout.Width(50)))
            {
                _rules.RemoveAt(index);
                return;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!rule.Enabled);

            rule.Prefix = EditorGUILayout.TextField("プレフィックス", rule.Prefix);
            rule.BackgroundColor = EditorGUILayout.ColorField("背景色", rule.BackgroundColor);
            rule.TextColor = EditorGUILayout.ColorField("テキスト色", rule.TextColor);
            rule.FontStyle = (FontStyle)EditorGUILayout.EnumPopup("フォントスタイル", rule.FontStyle);

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();

            _rules[index] = rule;
        }
    }
}
#endif