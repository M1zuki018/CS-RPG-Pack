using CryStar.Utility;
using UnityEngine;

namespace CryStar.Story.Settings
{
    /// <summary>
    /// 設定へのアクセスを簡単にするヘルパー
    /// </summary>
    public static class StorySettings
    {
        private static StorySystemSettingsSO _instance;

        public static StorySystemSettingsSO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<StorySystemSettingsSO>("StorySystemSettingsSO");
                    if (_instance == null)
                    {
                        LogUtility.Warning("Resource フォルダ内に StorySystemSettingsSO が見つかりませんでした。デフォルト設定で生成します");
                        _instance = StorySystemSettingsSO.CreateDefault();
                    }
                }

                return _instance;
            }
        }
    }
}