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
        /// 読み込んだオーダーデータのキャッシュ
        /// </summary>
        private List<OrderData> _orders;
        
        /// <summary>
        /// 指定範囲のデータを読み込んでSceneDataを作成する
        /// </summary>
        public async UniTask LoadSceneDataAsync(string spreadsheetName, string range)
        {
            _orders = await _dataLoader.LoadSceneDataAsync(spreadsheetName, range);
        }

        public async UniTask PlayStory(int sceneId, Action endAction)
        {
            var storySceneData = MasterStoryScene.GetSceneById(sceneId);
            if (storySceneData == null)
            {
                LogUtility.Error($"ストーリーが見つかりませんでした: {sceneId}", LogCategory.System);
            }
            
            if (_orders == null)
            {
                await InitializeAsync(KStoryPresentation.SPREAD_SHEET_NAME, RangeBuilder(KStoryPresentation.HEADER_RANGE));
                await LoadSceneDataAsync(KStoryPresentation.SPREAD_SHEET_NAME, RangeBuilder(storySceneData.Range));
            }
            
            _player.Play(_orders);
            await _player.PlayStory(storySceneData, endAction);
        }

        private string RangeBuilder(string range)
        {
            return KStoryPresentation.SPREAD_SHEET_NAME + "!" + range;
        }
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        private async UniTask InitializeAsync(string spreadsheetName, string headerRange)
        {
            // ヘッダー行を読み込んでインデックスマップを作成
            // ヘッダー行が全て共通している場合は呼びなおしは不要
            await _dataLoader.InitializeAsync(spreadsheetName, headerRange);
        }
    }
}