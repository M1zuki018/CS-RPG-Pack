using System;

namespace CryStar.Story.Core
{
    /// <summary>
    /// StorySceneDataServiceに関連するクラスを生成するFactory
    /// </summary>
    public static class StorySceneDataServiceFactory
    {
        private static Func<IStorySceneDataService> _customFactory;
    
        /// <summary>
        /// DIコンテナ使用時向け カスタムファクトリを設定
        /// </summary>
        public static void SetCustomFactory(Func<IStorySceneDataService> factory)
        {
            _customFactory = factory;
        }
    
        /// <summary>
        /// サービスインスタンスを作成
        /// </summary>
        public static IStorySceneDataService Create()
        {
            // カスタムファクトリが設定されていればそれを使用
            if (_customFactory != null)
            {
                return _customFactory();
            }
            
            var repository = new StorySceneDataRepository();
            var converter = new StorySceneDataConverter();
            var cache = new StorySceneDataCache();

            return new StorySceneDataService(repository, converter, cache);
        }
    }
}