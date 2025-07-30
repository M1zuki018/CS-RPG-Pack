using UnityEngine;

namespace CryStar.Story.Settings
{
    /// <summary>
    /// ストーリーシステムの設定
    /// </summary>
    [CreateAssetMenu(fileName = "StorySystemSettingsSO", menuName = "CryStar/Story/System Settings")]
    public class StorySystemSettingsSO : ScriptableObject
    {
        [Header("スプレッドシート設定")] 
        [SerializeField] private string _spreadsheetName = "TestStory";
        [SerializeField] private string _headerRange = "A2:O2";

        [Header("デフォルト値")] 
        [SerializeField] private float _defaultTextSpeed = 0.04f;
        [SerializeField] private float _defaultFadeDuration = 0.5f;
        [SerializeField] private int _autoPlayInterval = 3;

        [Header("キャッシュ設定")] 
        [SerializeField] private int _maxCacheSize = 10;
        [SerializeField] private bool _enablePreloading = true;

        /// <summary>
        /// スプレッドシート名
        /// </summary>
        public string SpreadsheetName => _spreadsheetName;
        
        /// <summary>
        /// ヘッダーの範囲
        /// </summary>
        public string HeaderRange => _headerRange;
        
        /// <summary>
        /// デフォルトのテキスト再生速度
        /// </summary>
        public float DefaultTextSpeed => _defaultTextSpeed;
        
        /// <summary>
        /// デフォルトのフェードにかける秒数
        /// </summary>
        public float DefaultFadeDuration => _defaultFadeDuration;
        
        /// <summary>
        /// オーダー実行後オート再生が実行されるまでの間隔
        /// </summary>
        public int AutoPlayInterval => _autoPlayInterval;
        
        /// <summary>
        /// 最大のキャッシュサイズ
        /// </summary>
        public int MaxCacheSize => _maxCacheSize;

        /// <summary>
        /// 事前ロードを行うか
        /// </summary>
        public bool EnablePreloading => _enablePreloading;

        /// <summary>
        /// デフォルト設定を作成
        /// </summary>
        public static StorySystemSettingsSO CreateDefault()
        {
            var settings = CreateInstance<StorySystemSettingsSO>();
            settings._spreadsheetName = "TestStory";
            settings._headerRange = "A2:O2";
            settings._defaultTextSpeed = 0.04f;
            settings._defaultFadeDuration = 0.5f;
            settings._autoPlayInterval = 3;
            settings._maxCacheSize = 10;
            settings._enablePreloading = true;
            return settings;
        }
    }
}