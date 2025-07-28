using System;
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
        /// 読み込んだシーンデータのキャッシュ
        /// </summary>
        private SceneData _sceneData;

        // TODO: 仮おき
        private const string SPREAD_SHEET_NAME = "TestStory";
        private const string HEADER_RANGE = "TestStory!A2:O2";
        private const string RANGE = "TestStory!A220:O298";
        
        /// <summary>
        /// 指定範囲のデータを読み込んでSceneDataを作成する
        /// </summary>
        public async UniTask LoadSceneDataAsync(string spreadsheetName, string range)
        {
            _sceneData = await _dataLoader.LoadSceneDataAsync(spreadsheetName, range);
        }

        public async UniTask PlayStory(SceneDataSO sceneDataSo, Action endAction)
        {
            if (_sceneData == null)
            {
                await InitializeAsync(SPREAD_SHEET_NAME, HEADER_RANGE);
                await LoadSceneDataAsync(SPREAD_SHEET_NAME, RANGE);
            }
            
            _player.Play(_sceneData);
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