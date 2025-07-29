using System;
using Cysharp.Threading.Tasks;

namespace CryStar.Story.Orchestrators
{
    /// <summary>
    /// Storyシステム全体を統括して管理するクラスが継承すべきインターフェース
    /// </summary>
    public interface IStoryOrchestrator
    {
        /// <summary>
        /// ストーリーを再生する
        /// </summary>
        UniTask PlayStoryAsync(int sceneId, Action endAction);
        
        /// <summary>
        /// 指定範囲のデータを読み込んでSceneDataを作成する
        /// NOTE: 事前にロードしておきたい場合などはこのメソッドだけ呼び出せばOK
        /// </summary>
        UniTask LoadSceneDataAsync(int sceneId);
        
        /// <summary>
        /// 指定したシーンのキャッシュをクリアする
        /// </summary>
        void ClearCache(int sceneId);
        
        /// <summary>
        /// キャッシュをクリアする
        /// </summary>
        void ClearAllCache();
    }
}
