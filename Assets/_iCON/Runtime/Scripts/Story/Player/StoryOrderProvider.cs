  using System.Collections.Generic;
  using CryStar.Story.Core;
  using Cysharp.Threading.Tasks;
  using iCON.Enums;
  
  namespace iCON.System
  {
      /// <summary>
      /// ストーリーオーダーの取得を専門とするクラス
      /// NOTE: データの読み込みとオーダー取得のロジックを担当
      /// </summary>
      public class StoryOrderProvider
      {
          private IReadOnlyList<OrderData> _orders;

          /// <summary>
          /// 再生を行うシーンデータのキャッシュのセットアップ
          /// </summary>
          public void Setup(IReadOnlyList<OrderData> orders)
          {
              _orders = orders;
          }

          /// <summary>
          /// 指定位置のオーダーを取得
          /// </summary>
          public OrderData GetOrderAt(StoryPosition position)
          {
              return GetOrderAt(position.OrderIndex);
          }

          /// <summary>
          /// 指定位置からAppendが出現するまでの連続オーダーを取得
          /// </summary>
          public List<OrderData> GetContinuousOrdersFrom(StoryPosition startPosition)
          {
              var orders = new List<OrderData>();
              var currentIndex = startPosition.OrderIndex;

              // 最初のオーダーを追加
              var firstOrder = GetOrderAt(currentIndex);
              if (firstOrder == null) return orders;

              orders.Add(firstOrder);
              currentIndex++;

              // Append以外のオーダーが続く限り取得を継続
              while (IsValidOrderIndex(currentIndex))
              {
                  var order = GetOrderAt(currentIndex);
                  if (order.Sequence == SequenceType.Append)
                      break;

                  orders.Add(order);
                  currentIndex++;
              }

              return orders;
          }

          /// <summary>
          /// 次のオーダーが存在するかチェック
          /// </summary>
          public bool HasNextOrder(StoryPosition position)
          {
              return IsValidOrderIndex(position.OrderIndex + 1);
          }

          /// <summary>
          /// 次のオーダーのシーケンスタイプを確認
          /// </summary>
          public SequenceType? PeekNextOrderSequence(StoryPosition position)
          {
              var nextOrder = GetOrderAt(position.OrderIndex + 1);
              return nextOrder?.Sequence;
          }

          /// <summary>
          /// オーダーの総数を取得
          /// </summary>
          public int GetOrderCount()
          {
              return _orders?.Count ?? 0;
          }

          /// <summary>
          /// 指定インデックスのオーダーを取得
          /// </summary>
          private OrderData GetOrderAt(int orderIndex)
          {
              if (!IsValidOrderIndex(orderIndex))
                  return null;

              return _orders[orderIndex];
          }
          
          /// <summary>
          /// データの有効性を確認
          /// </summary>
          private bool IsValidOrderIndex(int orderIndex)
          {
              return _orders != null &&
                     orderIndex >= 0 &&
                     orderIndex < _orders.Count;
          }
      }
  }