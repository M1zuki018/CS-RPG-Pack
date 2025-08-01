using CryStar.Story.Factory;
using CryStar.Utility;
using UnityEngine;

namespace CryStar.Story.Initialization
{
    /// <summary>
    /// ストーリーシステムの初期化を行うクラス（ゲーム開始時に一度だけ実行される）
    /// </summary>
    public static class StorySystemInitializer
    {
        /// <summary>
        /// 初期化済みか
        /// </summary>
        private static bool _isInitialized = false;

        /// <summary>
        /// ストーリーシステムを初期化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (_isInitialized)
            {
                // 既に初期化済みであれば以降の処理は行わない
                return;
            }

            // ファクトリーの初期化
            OrderHandlerFactory.Initialize();
            EffectPerformerFactory.Initialize();

            LogUtility.Verbose($"ストーリーシステムを初期化しました - OrderHandlers: {OrderHandlerFactory.GetRegisteredHandlerCount()}, EffectPerformers: {EffectPerformerFactory.GetRegisteredHandlerCount()}");
            
            _isInitialized = true;
        }

        /// <summary>
        /// 手動初期化（必要に応じて使用する）
        /// </summary>
        public static void ManualInitialize()
        {
            _isInitialized = false;
            Initialize();
        }
    }   
}