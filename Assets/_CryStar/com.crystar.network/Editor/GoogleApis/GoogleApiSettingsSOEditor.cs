using System;
using System.IO;
using CryStar.Network;
using UnityEditor;
using UnityEngine;

namespace CryStar.Editor
{
    /// <summary>
    /// GoogleApiSettingsSOのエディター拡張
    /// </summary>
    public static class GoogleApiSettingsSOEditor
    {
        private const string DEFAULT_PATH_BASE = "Assets/Resources/GoogleApiSettings";
        private const string DEFAULT_EXTENSION = ".asset";
        private const int MAX_UNIQUE_ATTEMPTS = 1000; // 無限ループをふせぐためのユニークパスの数字の最大値

        /// <summary>
        /// デフォルト設定でスクリプタブルオブジェクトを作成する
        /// </summary>
        [MenuItem("CryStar/Network/Create Google API Settings")]
        public static GoogleApiSettingsSO CreateAsset()
        {
            return CreateAsset(DEFAULT_PATH_BASE);
        }

        /// <summary>
        /// 指定パスでスクリプタブルオブジェクトを作成
        /// </summary>
        public static GoogleApiSettingsSO CreateAsset(string basePathWithoutExtension)
        {
            var settings = ScriptableObject.CreateInstance<GoogleApiSettingsSO>();

            // 名前の重複を避けるためにユニークパスを生成
            var uniquePath = GenerateUniqueAssetPath(basePathWithoutExtension, DEFAULT_EXTENSION);

            // ディレクトリが存在しない場合は作成
            var directory = Path.GetDirectoryName(uniquePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            AssetDatabase.CreateAsset(settings, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 作成したアセットを選択・ハイライト
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);

            Debug.Log($"GoogleApiSettings を作成しました: {uniquePath}");

            return settings;
        }

        /// <summary>
        /// 重複を避けるユニークなアセットパスを生成
        /// </summary>
        private static string GenerateUniqueAssetPath(string basePathWithoutExtension, string extension)
        {
            var basePath = basePathWithoutExtension + extension;
            
            if (!File.Exists(basePath))
            {
                return basePath;
            }

            // 番号付きでユニークなパスを生成
            var counter = 1;
            string uniquePath;
            
            do
            {
                uniquePath = $"{basePathWithoutExtension} {counter}{extension}";
                counter++;
            } 
            while (File.Exists(uniquePath) && counter < MAX_UNIQUE_ATTEMPTS); // 無限ループ防止

            return uniquePath;
        }

        /// <summary>
        /// プロジェクト内のGoogleApiSettingsを検索する
        /// </summary>
        public static GoogleApiSettingsSO FindInProject()
        {
            var guids = AssetDatabase.FindAssets("t:GoogleApiSettingsSO");

            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GoogleApiSettingsSO>(path);
            }

            return null;
        }

        /// <summary>
        /// プロジェクト内のすべてのGoogleApiSettingsを検索する
        /// </summary>
        public static GoogleApiSettingsSO[] FindAllInProject()
        {
            var guids = AssetDatabase.FindAssets("t:GoogleApiSettingsSO");
            var settings = new GoogleApiSettingsSO[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                settings[i] = AssetDatabase.LoadAssetAtPath<GoogleApiSettingsSO>(path);
            }

            return settings;
        }

        /// <summary>
        /// プロジェクト内のGoogleApiSettingsを検索し、なければ作成する
        /// </summary>
        public static GoogleApiSettingsSO FindOrCreate()
        {
            var existing = FindInProject();
            if (existing != null)
            {
                return existing;
            }

            return CreateAsset();
        }

        /// <summary>
        /// 指定した名前でGoogleApiSettingsを検索する
        /// </summary>
        public static GoogleApiSettingsSO FindByName(string name)
        {
            var allSettings = FindAllInProject();
            
            foreach (var setting in allSettings)
            {
                if (setting.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return setting;
                }
            }

            return null;
        }
    }
}