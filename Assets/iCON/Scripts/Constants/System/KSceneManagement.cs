namespace iCON.Constants
{
    /// <summary>
    /// シーン管理に関連する定数
    /// </summary>
    public static class KSceneManagement
    {
        /// <summary>
        /// BootSceneのパス
        /// </summary>
        public const string BOOT_SCENE_PATH = "Assets/Scenes/Bootstrap.unity";
        
        /// <summary>
        /// システムシーンの総数
        /// </summary>
        public const int SYSTEM_SCENE_COUNT = 2;

        /// <summary>
        /// アセットのプリロードのために待つ時間（ミリ秒）
        /// </summary>
        public const int ASSET_PRELOAD_WAIT_TIME_MS = 500;
    }
}
