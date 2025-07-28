using System;
using System.Collections.Generic;
using CryStar.Story.Constants;
using Cysharp.Threading.Tasks;
using iCON.System;
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
        private StoryDataLoader _dataLoader = new StoryDataLoader();
        
        /// <summary>
        /// ストーリーIDと読み込んだオーダーデータのキャッシュのkvp
        /// </summary>
        private Dictionary<int, List<OrderData>> _ordersCache = new Dictionary<int, List<OrderData>>();
        
        /// <summary>
        /// 初期化済みか
        /// </summary>
        private bool _isInitialized = false;

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
            
            // ストーリー再生
            _player.PlayStory(storySceneData, _ordersCache[sceneId], endAction);
        }
        
        /// <summary>
        /// 指定範囲のデータを読み込んでSceneDataを作成する
        /// NOTE: 事前にロードしておきたい場合などはこのメソッドだけ呼び出せばOK
        /// </summary>
        public async UniTask LoadSceneDataAsync(int sceneId)
        {
            if (_ordersCache.ContainsKey(sceneId))
            {
                // 既にキャッシュが生成されていればスキップ
                return;
            }
            
            if (!_isInitialized)
            {
                // ヘッダーの初期化を行っていない場合は先にヘッダーの初期化を行う
                await InitializeAsync();
            }
            
            if (TryGetMasterData(sceneId, out var sceneData))
            { 
                // オーダーリストを取得した後、辞書に登録
                var orders = await _dataLoader.LoadSceneDataAsync(KStoryPresentation.SPREAD_SHEET_NAME, BuildSheetRange(sceneData.Range));
                _ordersCache[sceneId] = orders;
            }
        }

        /// <summary>
        /// 指定したシーンのキャッシュをクリアする
        /// </summary>
        public void ClearCache(int sceneId)
        {
            _ordersCache[sceneId] = null;
        }
        
        /// <summary>
        /// キャッシュをクリアする
        /// </summary>
        public void ClearAllCache()
        {
            _ordersCache.Clear();
        }
        
        #region Private methods
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        private async UniTask InitializeAsync()
        {
            // ヘッダー行を読み込んでインデックスマップを作成
            // ヘッダー行が全て共通している場合は呼びなおしは不要
            await _dataLoader.InitializeAsync(KStoryPresentation.SPREAD_SHEET_NAME, BuildSheetRange(KStoryPresentation.HEADER_RANGE));
            _isInitialized = true;
        }
        
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