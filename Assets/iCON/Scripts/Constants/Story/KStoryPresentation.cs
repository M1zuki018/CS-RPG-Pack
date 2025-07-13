using DG.Tweening;

namespace iCON.Constants
{
    /// <summary>
    /// ストーリーに関連する定数
    /// </summary>
    public static class KStoryPresentation
    {
        /// <summary>
        /// フェード時間
        /// </summary>
        public const float IMAGE_FADE_DURATION = 0.2f;
        
        /// <summary>
        /// BGMのフェード時間
        /// </summary>
        public const float BGM_FADE_DURATION = 0.5f;

        /// <summary>
        /// オート再生モードでのテキスト表示間隔
        /// </summary>
        public const int AUTO_PLAY_INTERVAL = 3;

        /// <summary>
        /// フェード時のイージング
        /// </summary>
        public const Ease FADE_EASE = Ease.Linear;
        
        /// <summary>
        /// BGMフェード時のイージング
        /// </summary>
        public const Ease BGM_FADE_EASE = Ease.OutSine;
    }
}
