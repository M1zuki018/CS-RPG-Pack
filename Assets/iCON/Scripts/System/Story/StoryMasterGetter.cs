using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        
        public async UniTask Setup()
        {
            var data = await SheetsDataService.Instance.ReadFromSpreadsheetAsync("TestStory", "TestStory!A3:N15");
            var list = LoadFromSpreadsheet(data);

            foreach (var a in list)
            {
                Debug.Log(a.OrderType);
            }
            
            SheetsDataService.Instance.LogTableContentInternal("TestStory", "TestStory!A3:N15", 12);
        }
        
        /// <summary>
        /// スプレッドシートから読み込み
        /// </summary>
        public List<OrderData> LoadFromSpreadsheet(IList<IList<object>> spreadsheetData)
        {
            // ヘッダー行から列インデックスマップを作成
            BuildColumnIndexMap(spreadsheetData);
            
            var orderDataList = new List<OrderData>();
            
            // データ行を処理（ヘッダー行をスキップ）
            for (int row = 1; row < spreadsheetData.Count; row++)
            {
                var orderData = CreateOrderData(spreadsheetData, row);
                if (orderData != null)
                {
                    orderDataList.Add(orderData);
                }
            }
            
            return orderDataList;
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
                    Sequence = GetIntValue(row, StoryDataColumn.Sequence),
                    SpeakerId = GetStringValue(row, StoryDataColumn.SpeakerId),
                    DialogText = GetStringValue(row, StoryDataColumn.DialogText),
                    OverrideDisplayName = GetStringValue(row, StoryDataColumn.OverrideDisplayName),
                    FilePath = GetStringValue(row, StoryDataColumn.FilePath),
                    Position = GetType<CharacterPositionType>(row, StoryDataColumn.Position),
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
        /// マスタデータからデータを取得する
        /// </summary>
        public async UniTask<OrderData> Get(int partId, int chapterId, int sceneId, int orderId = 0)
        {
            
            return null;
        }
        
        /// <summary>
        /// ヘッダー行から列インデックスマップを構築（IList<object>対応）
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
        /// 指定された列から文字列値を取得（IList<object>対応）
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
