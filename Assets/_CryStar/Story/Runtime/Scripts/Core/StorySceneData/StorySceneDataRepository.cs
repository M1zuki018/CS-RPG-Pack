using System;
using System.Collections.Generic;
using CryStar.Network;
using Cysharp.Threading.Tasks;

namespace CryStar.Story.Core
{
    /// <summary>
    /// スプレッドシートとの接続を行いデータを取得する
    /// </summary>
    public class StorySceneDataRepository : IStorySceneDataRepository
    {
        /// <summary>
        /// 初期化済みか
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// ヘッダー行の読み込みを行う
        /// </summary>
        public async UniTask<IList<IList<object>>> InitializeAsync(string spreadsheetName, string headerRange)
        {
            // ヘッダー行を読み込む
            var headerData = await SheetsDataService.Instance.ReadFromSpreadsheetAsync(spreadsheetName, headerRange);

            if (headerData == null || headerData.Count == 0)
            {
                throw new InvalidOperationException($"ヘッダーデータの読み込みに失敗しました: {spreadsheetName}, {headerRange}");
            }

            // 初期化完了
            _isInitialized = true;
            
            return headerData;
        }

        /// <summary>
        /// 指定範囲のデータを読み込んでSceneDataを作成する
        /// </summary>
        public async UniTask<IList<IList<object>>> LoadSceneDataAsync(string spreadsheetName, string dataRange)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("InitializeAsync を先に呼び出してください");
            }
            
            return await SheetsDataService.Instance.ReadFromSpreadsheetAsync(spreadsheetName, dataRange);
        }
    }
}