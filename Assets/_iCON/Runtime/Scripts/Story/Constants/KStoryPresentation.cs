using DG.Tweening;

namespace CryStar.Story.Constants
{
    /// <summary>
    /// ストーリーに関連する定数
    /// </summary>
    public static class KStoryPresentation
    {
        /// <summary>
        /// ストーリーのスプレッドシート名 TODO: 仮
        /// </summary>
        public const string SPREAD_SHEET_NAME = "TestStory";
        
        /// <summary>
        /// ヘッダーの範囲 TODO: 仮
        /// </summary>
        public const string HEADER_RANGE = "A2:O2";
        
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
        public const Ease BGM_FADE_EASE = Ease.InSine;
        
        /// <summary>
        /// ダイアログのフェード秒数
        /// </summary>
        public const float DIALOG_FADE_DURATION = 0.1f;

        /// <summary>
        /// ダイアログの文字送りの速さ
        /// </summary>
        public const float DIALOG_TEXT_SPEED = 0.04f;
    }
}
