#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CryStar.Editor
{
    /// <summary>
    /// ボタンの子にあるTextオブジェクト（Text/TextMeshProUGUI）の名前を自動リネームするエディター拡張
    /// </summary>
    public class ButtonTextRenamer : EditorWindow
    {
        private bool _includeInactiveObjects = true;
        private bool _showDebugLog = true;
        private bool _supportLegacyText = true;
        private bool _supportTMPText = true;
        private string _customSuffix = "_Text";

        [MenuItem("CryStar/Tools/Button Text Renamer")]
        public static void ShowWindow()
        {
            GetWindow<ButtonTextRenamer>("Button Text Renamer");
        }

        private void OnEnable()
        {
            LoadPreferences();
        }

        private void OnDisable()
        {
            SavePreferences();
        }

        private void OnGUI()
        {
            GUILayout.Label("Button Text Renamer", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "このツールはシーン内のすべてのButtonコンポーネントを検索し、" +
                "その子オブジェクトにあるText/TextMeshProUGUIコンポーネントの名前を親ボタンのGameObject名に合わせてリネームします。",
                MessageType.Info
            );

            GUILayout.Space(10);

            // 対応コンポーネント設定
            EditorGUILayout.LabelField("対応コンポーネント", EditorStyles.boldLabel);
            _supportLegacyText = EditorGUILayout.Toggle("Legacy Text (UI)", _supportLegacyText);
            _supportTMPText = EditorGUILayout.Toggle("TextMeshPro UGUI", _supportTMPText);

            if (!_supportLegacyText && !_supportTMPText)
            {
                EditorGUILayout.HelpBox("少なくとも1つのテキストコンポーネントを選択してください。", MessageType.Warning);
            }

            GUILayout.Space(10);

            // オプション設定
            EditorGUILayout.LabelField("オプション", EditorStyles.boldLabel);
            _includeInactiveObjects = EditorGUILayout.Toggle("非アクティブオブジェクトも含む", _includeInactiveObjects);
            _showDebugLog = EditorGUILayout.Toggle("ログを表示", _showDebugLog);
            _customSuffix = EditorGUILayout.TextField("カスタムサフィックス", _customSuffix);

            GUILayout.Space(15);

            // 実行ボタン
            EditorGUI.BeginDisabledGroup(!_supportLegacyText && !_supportTMPText);
            if (GUILayout.Button("シーン内の全ボタンテキストをリネーム", GUILayout.Height(30)))
            {
                RenameAllButtonTexts();
            }

            GUILayout.Space(10);

            // 選択されたオブジェクトのみ処理するボタン
            if (GUILayout.Button("選択されたボタンのテキストをリネーム", GUILayout.Height(25)))
            {
                RenameSelectedButtonTexts();
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            // 統計情報表示
            ShowStatistics();

            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "使用方法:\n" +
                "1. 'シーン内の全ボタンテキストをリネーム'でシーン内すべてのボタンを処理\n" +
                "2. '選択されたボタンのテキストをリネーム'で選択したボタンのみを処理\n" +
                "3. 対応コンポーネントで処理対象を選択可能",
                MessageType.None
            );
        }

        /// <summary>
        /// 統計情報を表示
        /// </summary>
        private void ShowStatistics()
        {
            Button[] buttons = FindObjectsOfType<Button>(_includeInactiveObjects);
            int textCount = 0;
            int tmpCount = 0;

            foreach (Button button in buttons)
            {
                if (_supportLegacyText)
                {
                    Text[] texts = button.GetComponentsInChildren<Text>(_includeInactiveObjects);
                    textCount += texts.Length;
                }

                if (_supportTMPText)
                {
                    TextMeshProUGUI[] tmpTexts =
                        button.GetComponentsInChildren<TextMeshProUGUI>(_includeInactiveObjects);
                    tmpCount += tmpTexts.Length;
                }
            }

            EditorGUILayout.LabelField("統計情報", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"検出されたボタン: {buttons.Length}個");
            if (_supportLegacyText)
                EditorGUILayout.LabelField($"Legacy Textコンポーネント: {textCount}個");
            if (_supportTMPText)
                EditorGUILayout.LabelField($"TextMeshPro UGUIコンポーネント: {tmpCount}個");
        }

        /// <summary>
        /// シーン内のすべてのボタンのテキストをリネーム
        /// </summary>
        private void RenameAllButtonTexts()
        {
            Button[] buttons = FindObjectsOfType<Button>(_includeInactiveObjects);
            var renamedObjects = new List<GameObject>();
            int renamedCount = 0;

            // Undoの準備
            foreach (Button button in buttons)
            {
                renamedObjects.AddRange(GetAllTextGameObjects(button));
            }

            if (renamedObjects.Count > 0)
            {
                Undo.RecordObjects(renamedObjects.ToArray(), "Rename Button Texts");
            }

            foreach (Button button in buttons)
            {
                if (RenameButtonText(button))
                {
                    renamedCount++;
                }
            }

            if (_showDebugLog)
            {
                Debug.Log($"ButtonTextRenamer: {renamedCount}個のボタンのテキストをリネームしました。（検索対象: {buttons.Length}個）");
            }

            EditorUtility.DisplayDialog("完了", $"{renamedCount}個のボタンのテキストをリネームしました。", "OK");
        }

        /// <summary>
        /// 選択されたボタンのテキストをリネーム
        /// </summary>
        private void RenameSelectedButtonTexts()
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            int renamedCount = 0;

            if (selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("エラー", "ボタンオブジェクトを選択してください。", "OK");
                return;
            }

            Button[] selectedButtons = Array
                .FindAll(selectedObjects, obj => obj.GetComponent<Button>() != null)
                .Select(obj => obj.GetComponent<Button>())
                .ToArray();

            if (selectedButtons.Length == 0)
            {
                EditorUtility.DisplayDialog("エラー", "選択されたオブジェクトにButtonコンポーネントが見つかりません。", "OK");
                return;
            }

            var renamedObjects = new List<GameObject>();
            foreach (Button button in selectedButtons)
            {
                renamedObjects.AddRange(GetAllTextGameObjects(button));
            }

            if (renamedObjects.Count > 0)
            {
                Undo.RecordObjects(renamedObjects.ToArray(), "Rename Selected Button Texts");
            }

            foreach (Button button in selectedButtons)
            {
                if (RenameButtonText(button))
                {
                    renamedCount++;
                }
            }

            if (_showDebugLog)
            {
                Debug.Log($"ButtonTextRenamer: {renamedCount}個の選択されたボタンのテキストをリネームしました。");
            }

            EditorUtility.DisplayDialog("完了", $"{renamedCount}個のボタンのテキストをリネームしました。", "OK");
        }

        /// <summary>
        /// 指定されたボタンの子テキストオブジェクトをリネーム
        /// </summary>
        private bool RenameButtonText(Button button)
        {
            if (button == null) return false;

            bool renamed = false;

            // Legacy Text処理
            if (_supportLegacyText)
            {
                Text[] childTexts = button.GetComponentsInChildren<Text>(_includeInactiveObjects);
                foreach (Text text in childTexts)
                {
                    if (IsValidTextChild(text.transform, button.transform))
                    {
                        if (RenameTextObject(text.gameObject, button.gameObject.name, "Text"))
                        {
                            renamed = true;
                        }
                    }
                }
            }

            // TextMeshPro処理
            if (_supportTMPText)
            {
                TextMeshProUGUI[] childTMPTexts =
                    button.GetComponentsInChildren<TextMeshProUGUI>(_includeInactiveObjects);
                foreach (TextMeshProUGUI tmpText in childTMPTexts)
                {
                    if (IsValidTextChild(tmpText.transform, button.transform))
                    {
                        if (RenameTextObject(tmpText.gameObject, button.gameObject.name, "TMP"))
                        {
                            renamed = true;
                        }
                    }
                }
            }

            return renamed;
        }

        /// <summary>
        /// テキストオブジェクトが有効な子オブジェクトかチェック
        /// </summary>
        private bool IsValidTextChild(Transform textTransform, Transform buttonTransform)
        {
            // 直接の子オブジェクトまたは孫オブジェクト（1階層下まで）を対象
            return textTransform.parent == buttonTransform ||
                   textTransform.parent.parent == buttonTransform;
        }

        /// <summary>
        /// テキストオブジェクトをリネーム
        /// </summary>
        private bool RenameTextObject(GameObject textObject, string buttonName, string componentType)
        {
            string newName = $"{buttonName}{_customSuffix}";

            // TMP の場合は区別のためサフィックスを変更（オプション）
            if (componentType == "TMP" && _supportLegacyText && _supportTMPText)
            {
                newName = $"{buttonName}_TMP";
            }

            if (textObject.name != newName)
            {
                string oldName = textObject.name;
                textObject.name = newName;

                if (_showDebugLog)
                {
                    Debug.Log($"リネーム ({componentType}): {oldName} → {newName}");
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 指定されたボタンからすべてのテキストGameObjectを取得
        /// </summary>
        private List<GameObject> GetAllTextGameObjects(Button button)
        {
            var textObjects = new List<GameObject>();

            if (_supportLegacyText)
            {
                Text[] texts = button.GetComponentsInChildren<Text>(_includeInactiveObjects);
                foreach (Text text in texts)
                {
                    if (IsValidTextChild(text.transform, button.transform))
                    {
                        textObjects.Add(text.gameObject);
                    }
                }
            }

            if (_supportTMPText)
            {
                TextMeshProUGUI[] tmpTexts = button.GetComponentsInChildren<TextMeshProUGUI>(_includeInactiveObjects);
                foreach (TextMeshProUGUI tmpText in tmpTexts)
                {
                    if (IsValidTextChild(tmpText.transform, button.transform))
                    {
                        textObjects.Add(tmpText.gameObject);
                    }
                }
            }

            return textObjects;
        }

        /// <summary>
        /// 設定を保存
        /// </summary>
        private void SavePreferences()
        {
            EditorPrefs.SetBool("ButtonTextRenamer_IncludeInactive", _includeInactiveObjects);
            EditorPrefs.SetBool("ButtonTextRenamer_ShowDebugLog", _showDebugLog);
            EditorPrefs.SetBool("ButtonTextRenamer_SupportLegacyText", _supportLegacyText);
            EditorPrefs.SetBool("ButtonTextRenamer_SupportTMPText", _supportTMPText);
            EditorPrefs.SetString("ButtonTextRenamer_CustomSuffix", _customSuffix);
        }

        /// <summary>
        /// 設定を読み込み
        /// </summary>
        private void LoadPreferences()
        {
            _includeInactiveObjects = EditorPrefs.GetBool("ButtonTextRenamer_IncludeInactive", true);
            _showDebugLog = EditorPrefs.GetBool("ButtonTextRenamer_ShowDebugLog", true);
            _supportLegacyText = EditorPrefs.GetBool("ButtonTextRenamer_SupportLegacyText", true);
            _supportTMPText = EditorPrefs.GetBool("ButtonTextRenamer_SupportTMPText", true);
            _customSuffix = EditorPrefs.GetString("ButtonTextRenamer_CustomSuffix", "_Text");
        }
    }
}
#endif