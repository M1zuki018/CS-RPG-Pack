using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using iCON.System;
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

        // TODO: 仮おき
        private const string SPREAD_SHEET_NAME = "TestStory";
        private const string HEADER_RANGE = "TestStory!A2:O2";
        private const string RANGE = "TestStory!A220:O298";
        
        /// <summary>
        /// 指定範囲のデータを読み込んでSceneDataを作成する
        /// </summary>
        public async UniTask LoadSceneDataAsync(string spreadsheetName, string range)
        {
            _orders = await _dataLoader.LoadSceneDataAsync(spreadsheetName, range);
        }

        public async UniTask PlayStory(SceneDataSO sceneDataSo, Action endAction)
        {
            if (_orders == null)
            {
                await InitializeAsync(SPREAD_SHEET_NAME, HEADER_RANGE);
                await LoadSceneDataAsync(SPREAD_SHEET_NAME, RANGE);
            }
            
            _player.Play(_orders);
            await _player.PlayStory(sceneDataSo, endAction);
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