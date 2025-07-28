using System;
using CryStar.Story.Constants;
using CryStar.Story.Player;
using Cysharp.Threading.Tasks;
using iCON.Utility;
using UnityEngine;

namespace CryStar.Story.Core
{
    /// <summary>
    /// Storyシステムを管理するクラス
    /// </summary>
    public class StoryManager : ViewBase
    {
        /// <summary>
        /// ストーリー再生用クラス
        /// </summary>
        [SerializeField]
        private StoryPlayer _player;
        
        /// <summary>
        /// マスタデータを読み取りストーリー再生可能なデータに整えるためのクラス
        /// </summary>
        private StorySceneDataService _sceneDataService = new StorySceneDataService();
        
        #region Life cycle

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