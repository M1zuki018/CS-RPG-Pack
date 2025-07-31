using System;
using CryStar.Core;
using CryStar.Story.Constants;
using CryStar.Story.Core;
using CryStar.Story.Data;
using CryStar.Story.Initialization;
using CryStar.Story.Player;
using CryStar.Utility;
using CryStar.Utility.Enum;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CryStar.Story.Orchestrators
{
    /// <summary>
    /// Story Orchestrator
    /// </summary>
    public class StoryOrchestrator : CustomBehaviour, IStoryOrchestrator
    {
        /// <summary>
        /// ストーリー再生用クラス
        /// </summary>
        [SerializeField]
        private StoryPlayer _player;
        
        /// <summary>
        /// マスタデータを読み取りストーリー再生可能なデータに整えるためのクラス
        /// </summary>
        private IStorySceneDataService _sceneDataService = StorySceneDataServiceFactory.Create();
        
        #region Life cycle

        public override UniTask OnAwake()
        {
            // 念のためストーリーシステムが初期化されていることを確認する
            StorySystemInitializer.Initialize();
            return base.OnAwake();
        }

        private void OnDestroy()
        {
            ClearAllCache();
        }

        #endregion
        
        /// <summary>
        /// ストーリーを再生する
        /// </summary>
        public async UniTask PlayStoryAsync(int sceneId, Action endAction)
        {
            // シーンIDを元にシーンマスタを取得
            if (!TryGetMasterData(sceneId, out var storySceneData))
            {
                return;
            }
            
            // 指定されたオーダーを取得
            await LoadSceneDataAsync(sceneId);
            var orders = await _sceneDataService.GetSceneDataAsync(
                sceneId, 
                KStoryPresentation.SPREAD_SHEET_NAME, 
                BuildSheetRange(storySceneData.Range)
            );
            
            // ストーリー再生
            _player.PlayStory(storySceneData, orders, endAction);
        }
        
        /// <summary>
        /// 指定範囲のデータを読み込んでSceneDataを作成する
        /// NOTE: 事前にロードしておきたい場合などはこのメソッドだけ呼び出せばOK
        /// </summary>
        public async UniTask LoadSceneDataAsync(int sceneId)
        {
            if (!_sceneDataService.IsInitialized)
            {
                // 初期化されていなかったらヘッダーの初期化を先に行う
                await _sceneDataService.InitializeAsync(
                    KStoryPresentation.SPREAD_SHEET_NAME, 
                    BuildSheetRange(KStoryPresentation.HEADER_RANGE)
                );
            }
        
            if (TryGetMasterData(sceneId, out var sceneData))
            {
                // シーンデータを取得する
                await _sceneDataService.GetSceneDataAsync(
                    sceneId,
                    KStoryPresentation.SPREAD_SHEET_NAME,
                    BuildSheetRange(sceneData.Range)
                );
            }
        }

        /// <summary>
        /// 指定したシーンのキャッシュをクリアする
        /// </summary>
        public void ClearCache(int sceneId)
        {
            _sceneDataService.ClearCache(sceneId);
        }
        
        /// <summary>
        /// キャッシュをクリアする
        /// </summary>
        public void ClearAllCache()
        {
            _sceneDataService.ClearAllCache();
        }
        
        #region Private methods
        
        /// <summary>
        /// マスタからStorySceneDataを取得してnullチェックを行う
        /// </summary>
        private bool TryGetMasterData(int sceneId, out StorySceneData sceneData)
        {
            sceneData = MasterStoryScene.GetSceneById(sceneId);
            if (sceneData == null)
            {
                LogUtility.Error($"ストーリーが見つかりませんでした: {sceneId}", LogCategory.System);
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// スプレッドシートの範囲指定文字列を構築する
        /// </summary>
        private string BuildSheetRange(string range)
        {
            return $"{KStoryPresentation.SPREAD_SHEET_NAME}!{range}";
        }
        
        #endregion
    }
}