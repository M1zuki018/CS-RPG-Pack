#if UNITY_EDITOR
using System;
using System.Text;
using CryStar.Story.Factory;
using CryStar.Story.Settings;
using CryStar.Story.UI;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CryStar.Story.Editor
{
    /// <summary>
    /// ストーリーシステム用のエディタ拡張
    /// </summary>
    public static class StorySystemEditorMenu
    {
        /// <summary>
        /// システム設定のスクリプタブルオブジェクトをResourcesフォルダ内に作成する
        /// </summary>
        [MenuItem("CryStar/Story/Create System Settings")]
        public static void CreateSystemSettings()
        {
            var settings = StorySystemSettingsSO.CreateDefault();
            
            // Resourcesフォルダを作成
            var resourcesPath = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(resourcesPath))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            
            // 設定ファイルを保存
            var assetPath = $"{resourcesPath}/StorySystemSettingsSO.asset";
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // 作成されたアセットを選択
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
            
            Debug.Log($"Created StorySystemSettingsSO at {assetPath}");
        }

        /// <summary>
        /// Factoryの初期化を行う
        /// </summary>
        [MenuItem("CryStar/Story//Initialize Factories")]
        public static void InitializeFactories()
        {
            OrderHandlerFactory.Initialize();
            EffectPerformerFactory.Initialize();
            
            Debug.Log($"Factories Initialized - OrderHandlers: {OrderHandlerFactory.GetRegisteredHandlerCount()}, EffectPerformers: {EffectPerformerFactory.GetRegisteredHandlerCount()}");
        }

        /// <summary>
        /// バリデーション
        /// </summary>
        [MenuItem("CryStar/Story//Validate System")]
        public static void ValidateSystem()
        {
            var report = new StringBuilder();
            report.AppendLine("=== CryStar Story System Validation ===");
            
            // 設定ファイルの確認
            var settings = Resources.Load<StorySystemSettingsSO>("StorySystemSettingsSO");
            if (settings != null)
            {
                report.AppendLine("✓ System Settings found");
            }
            else
            {
                report.AppendLine("✗ System Settings not found");
            }
            
            // ファクトリーの確認
            try
            {
                OrderHandlerFactory.Initialize();
                EffectPerformerFactory.Initialize();
                
                report.AppendLine($"✓ OrderHandlers: {OrderHandlerFactory.GetRegisteredHandlerCount()}");
                report.AppendLine($"✓ EffectPerformers: {EffectPerformerFactory.GetRegisteredHandlerCount()}");
            }
            catch (Exception ex)
            {
                report.AppendLine($"✗ Factory initialization failed: {ex.Message}");
            }
            
            // 必要なコンポーネントの確認
            var storyView = Object.FindObjectOfType<StoryView>();
            if (storyView != null)
            {
                report.AppendLine("✓ StoryView found in scene");
            }
            else
            {
                report.AppendLine("⚠ StoryView not found in current scene");
            }
            
            Debug.Log(report.ToString());
        }
    }
}
#endif