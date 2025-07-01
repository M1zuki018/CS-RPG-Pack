using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// ストーリーのマスタデータを参照してゲーム内で利用できるように整形する
    /// </summary>
    public class StoryMasterGetter
    {
        private Dictionary<string, int> _columnIndexMap = new();
        private bool _isInitialized = false;
        
        /// <summary>
        /// Setup
        /// ヘッダー行を読み込んでインデックスマップを作成する
        /// </summary>
        public async UniTask InitializeAsync(string spreadsheetName, string headerRange)
        {
            var headerData = await SheetsDataService.Instance.ReadFromSpreadsheetAsync(spreadsheetName, headerRange);
            
            if (headerData == null || headerData.Count == 0)
            {
                throw new InvalidOperationException($"ヘッダーデータの読み込みに失敗しました: {spreadsheetName}, {headerRange}");
            }
            
            // ヘッダー行から列インデックスマップを作成
            BuildColumnIndexMap(headerData);
            
            // 初期化完了
            _isInitialized = true;
        }
        
        /// <summary>
        /// 指定範囲のデータを読み込んでSceneDataを作成する
        /// </summary>
        public async UniTask<SceneData> LoadSceneDataAsync(string spreadsheetName, string dataRange)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("InitializeAsync を先に呼び出してください");
            }
            
            var data = await SheetsDataService.Instance.ReadFromSpreadsheetAsync(spreadsheetName, dataRange);
            return LoadFromSpreadsheetData(data);
        }
        
        /// <summary>
        /// スプレッドシートから読み込み
        /// </summary>
        private SceneData LoadFromSpreadsheetData(IList<IList<object>> spreadsheetData)
        {
            var orderDataList = new List<OrderData>();
            
            // データ行を処理
            for (int row = 0; row < spreadsheetData.Count; row++)
            {
                var orderData = CreateOrderData(spreadsheetData, row);
                if (orderData != null)
                {
                    orderDataList.Add(orderData);
                }
            }
            
            SceneData sceneData = new SceneData(orderDataList[0].ChapterId, orderDataList[0].SceneId, orderDataList);
            
            return sceneData;
        }

        /// <summary>
        /// 行データからOrderDataを作成
        /// </summary>
        private OrderData CreateOrderData(IList<IList<object>> data, int rowIndex)
        {
            try
            {
                var row = data[rowIndex];

                return new OrderData
                {
                    PartId = GetIntValue(row, StoryDataColumn.PartId),
                    ChapterId = GetIntValue(row, StoryDataColumn.ChapterId),
                    SceneId = GetIntValue(row, StoryDataColumn.SceneId),
                    OrderId = GetIntValue(row, StoryDataColumn.OrderId),
                    OrderType = GetType<OrderType>(row, StoryDataColumn.OrderType),
                    Sequence = GetType<SequenceType>(row, StoryDataColumn.Sequence),
                    SpeakerId = GetStringValue(row, StoryDataColumn.SpeakerId),
                    DialogText = GetStringValue(row, StoryDataColumn.DialogText),
                    OverrideDisplayName = GetStringValue(row, StoryDataColumn.OverrideDisplayName),
                    FilePath = GetStringValue(row, StoryDataColumn.FilePath),
                    Position = GetType<CharacterPositionType>(row, StoryDataColumn.CharacterPositionType),
                    FacialExpressionType = GetType<FacialExpressionType>(row, StoryDataColumn.FacialExpressionType),
                    OverrideTextSpeed = GetFloatValue(row, StoryDataColumn.OverrideTextSpeed),
                    Duration = GetFloatValue(row, StoryDataColumn.Duration)
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"行 {rowIndex + 1} でエラーが発生しました: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// ヘッダー行から列インデックスマップを構築
        /// </summary>
        private void BuildColumnIndexMap(IList<IList<object>> data)
        {
            _columnIndexMap.Clear();
            
            if (data.Count == 0) return;
            
            var headerRow = data[0];
            for (int col = 0; col < headerRow.Count; col++)
            {
                string headerName = headerRow[col]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(headerName))
                {
                    _columnIndexMap[headerName] = col;
                }
            }
        }
        
        /// <summary>
        /// 指定された列から文字列値を取得
        /// </summary>
        private string GetStringValue(IList<object> row, StoryDataColumn column)
        {
            string columnName = column.ToString();
            if (_columnIndexMap.TryGetValue(columnName, out int columnIndex))
            {
                if (columnIndex < row.Count && row[columnIndex] != null)
                {
                    return row[columnIndex].ToString().Trim();
                }
            }
            
            if (columnIndex >= row.Count)
            {
                // 列が存在しない場合は警告を出さない（空の値として扱う）
                return string.Empty;
            }
            
            return string.Empty;
        }
        
        /// <summary>
        /// 指定された列からEnum値を取得
        /// </summary>
        private T GetType<T>(IList<object> row, StoryDataColumn column) where T : struct, Enum
        {
            string stringValue = GetStringValue(row, column);
            
            if (string.IsNullOrEmpty(stringValue))
            {
                return default(T);
            }
            
            if (Enum.TryParse(stringValue, true, out T result))
            {
                return result;
            }

            return default(T);
        }

        /// <summary>
        /// 指定された列から整数値を取得
        /// </summary>
        private int GetIntValue(IList<object> row, StoryDataColumn column)
        {
            string stringValue = GetStringValue(row, column);
            
            if (string.IsNullOrEmpty(stringValue))
            {
                return 0;
            }
            
            if (int.TryParse(stringValue, out int result))
            {
                return result;
            }
            
            return 0;
        }

        /// <summary>
        /// 指定された列からfloat値を取得
        /// </summary>
        private float GetFloatValue(IList<object> row, StoryDataColumn column)
        {
            string stringValue = GetStringValue(row, column);
            
            if (string.IsNullOrEmpty(stringValue))
            {
                return 0f;
            }
            
            if (float.TryParse(stringValue, out float result))
            {
                return result;
            }
            
            return 0f;
        }
    }
}
