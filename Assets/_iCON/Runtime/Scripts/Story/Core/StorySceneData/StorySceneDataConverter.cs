using System;
using System.Collections.Generic;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using UnityEngine;

namespace CryStar.Story.Core
{
    /// <summary>
    /// ストーリーのデータをゲーム内で使用できる形に変換するロジックをまとめたクラス
    /// </summary>
    public class StorySceneDataConverter : IStorySceneDataConverter
    {
        private Dictionary<string, int> _columnIndexMap = new();
        private bool _isInitialized = false;

        /// <summary>
        /// ヘッダーデータから列マップを初期化
        /// </summary>
        public void Initialize(IList<IList<object>> headerData)
        {
            BuildColumnIndexMap(headerData);
            _isInitialized = true;
        }

        /// <summary>
        /// 生データをOrderDataリストに変換
        /// </summary>
        public List<OrderData> ConvertToOrderDataList(IList<IList<object>> rawData)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Initialize を先に呼び出してください");
            }

            var orderDataList = new List<OrderData>();

            for (int row = 0; row < rawData.Count; row++)
            {
                var orderData = CreateOrderData(rawData, row);
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

                return new OrderData(
                    partId: GetIntValue(row, StoryDataColumnType.PartId),
                    chapterId: GetIntValue(row, StoryDataColumnType.ChapterId),
                    sceneId: GetIntValue(row, StoryDataColumnType.SceneId),
                    orderId: GetIntValue(row, StoryDataColumnType.OrderId),
                    orderType: GetType<OrderType>(row, StoryDataColumnType.OrderType),
                    sequence: GetType<SequenceType>(row, StoryDataColumnType.Sequence),
                    speakerId: GetIntValue(row, StoryDataColumnType.SpeakerId),
                    dialogText: GetStringValue(row, StoryDataColumnType.DialogText),
                    overrideDisplayName: GetStringValue(row, StoryDataColumnType.OverrideDisplayName),
                    filePath: GetStringValue(row, StoryDataColumnType.FilePath),
                    position: GetType<CharacterPositionType>(row, StoryDataColumnType.CharacterPosition),
                    facialExpressionType: GetType<FacialExpressionType>(row, StoryDataColumnType.FacialExpression),
                    overrideTextSpeed: GetFloatValue(row, StoryDataColumnType.OverrideTextSpeed),
                    duration: GetFloatValue(row, StoryDataColumnType.Duration)
                );
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
        private string GetStringValue(IList<object> row, StoryDataColumnType column)
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
        private T GetType<T>(IList<object> row, StoryDataColumnType column) where T : struct, Enum
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
        private int GetIntValue(IList<object> row, StoryDataColumnType column)
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
        private float GetFloatValue(IList<object> row, StoryDataColumnType column)
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
