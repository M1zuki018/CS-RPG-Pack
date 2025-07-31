#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using CryStar.Utility;
using CryStar.Utility.Enum;
using iCON.Constants;
using UnityEngine.SceneManagement;

namespace iCON.Boot
{
    /// <summary>
    /// エディター再生時に自動的にBootシーンに切り替えるエディター拡張機能
    /// </summary>
    [InitializeOnLoad]
    public static class BootSceneAutoSwitcher
    {
        /// <summary>
        /// 前回開いていたシーンのパスを保存するキー
        /// </summary>
        private const string PREF_KEY_PREVIOUS_SCENE = "BootSceneSwitcher_PreviousScene";
        
        /// <summary>
        /// 自動切り替え機能の有効/無効状態を保存するキー
        /// </summary>
        private const string PREF_KEY_ENABLED = "BootSceneSwitcher_Enabled";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static BootSceneAutoSwitcher()
        {
            // PlayModeの状態が変わった時に呼び出されるコールバックを登録
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// PlayModeの状態変化時に呼び出されるコールバック
        /// </summary>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // 機能が無効化されている場合は何もしない
            if (!IsAutoSwitchEnabled()) return;

            switch (state)
            {
                // Edit Mode → Play Mode への遷移時
                case PlayModeStateChange.ExitingEditMode:
                    // 現在のシーンを記憶してBootシーンに切り替える
                    SaveCurrentSceneAndSwitchToBoot();
                    break;

                // Play Mode → Edit Mode への遷移時（再生終了時）
                case PlayModeStateChange.EnteredEditMode:
                    // 記憶していた元のシーンに復帰する
                    RestorePreviousScene();
                    break;
            }
        }

        /// <summary>
        /// 現在開いているシーンのパスを保存してからBootシーンに遷移する
        /// </summary>
        private static void SaveCurrentSceneAndSwitchToBoot()
        {
            // 現在アクティブなシーンの情報を取得
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.path != KSceneManagement.BOOT_SCENE_PATH)
            {
                // 現在のシーンパスをEditorPrefsに保存（復帰用）
                EditorPrefs.SetString(PREF_KEY_PREVIOUS_SCENE, currentScene.path);

                // Bootシーンが実際に存在するかチェック
                if (File.Exists(KSceneManagement.BOOT_SCENE_PATH))
                {
                    // 未保存の変更がある場合はユーザーに保存を促してからBootSceneを開く
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    EditorSceneManager.OpenScene(KSceneManagement.BOOT_SCENE_PATH);
                }
                else
                {
                    LogUtility.Error($"Bootシーンが見つかりません: {KSceneManagement.BOOT_SCENE_PATH}", LogCategory.System);
                }
            }
        }

        /// <summary>
        /// PlayMode終了後、実行前に開いていたシーンを開き直す
        /// </summary>
        private static void RestorePreviousScene()
        {
            var previousScenePath = EditorPrefs.GetString(PREF_KEY_PREVIOUS_SCENE, "");

            if (!string.IsNullOrEmpty(previousScenePath) &&
                File.Exists(previousScenePath) &&
                previousScenePath != KSceneManagement.BOOT_SCENE_PATH)
            {
                // 元のシーンを開く
                EditorSceneManager.OpenScene(previousScenePath);
                
                // 使用済みの保存データを削除（次回のために）
                EditorPrefs.DeleteKey(PREF_KEY_PREVIOUS_SCENE);
            }
        }

        /// <summary>
        /// 自動切り替え機能が有効かどうかを判定する
        /// </summary>
        private static bool IsAutoSwitchEnabled()
        {
            return EditorPrefs.GetBool(PREF_KEY_ENABLED, true);
        }

        /// <summary>
        /// メニューからの自動切り替え機能のON/OFF切り替えを行う
        /// </summary>
        [MenuItem("Tools/Boot Scene Switcher/Toggle Auto Switch")]
        private static void ToggleAutoSwitch()
        {
            bool currentState = IsAutoSwitchEnabled();
            EditorPrefs.SetBool(PREF_KEY_ENABLED, !currentState); 
            LogUtility.Info($"Bootシーンの自動切り替え: {(!currentState ? "Enabled" : "Disabled")}", LogCategory.System);
        }
        
        /// <summary>
        /// メニュー項目の表示状態を制御（チェックマークの表示/非表示を管理する）
        /// </summary>
        [MenuItem("Tools/Boot Scene Switcher/Toggle Auto Switch", true)]
        private static bool ToggleAutoSwitchValidation()
        {
            Menu.SetChecked("Tools/Boot Scene Switcher/Toggle Auto Switch", IsAutoSwitchEnabled());
            return true;
        }

        /// <summary>
        /// 手動でBootシーンに切り替える機能（ショートカット: Ctrl+Alt+B）
        /// </summary>
        [MenuItem("Tools/Boot Scene Switcher/Go to Boot Scene %&b")]
        private static void GoToBootScene()
        {
            if (File.Exists(KSceneManagement.BOOT_SCENE_PATH))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(KSceneManagement.BOOT_SCENE_PATH);
            }
            else
            {
                LogUtility.Error($"Bootシーンが見つかりません: {KSceneManagement.BOOT_SCENE_PATH}", LogCategory.System);
            }
        }
    }
}
#endif