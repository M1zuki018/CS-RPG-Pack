  using System.Collections.Generic;
  using CryStar.Story.Data;
  using CryStar.Story.Enums;

  namespace CryStar.Story.Player
  {
      /// <summary>
      /// ストーリーオーダーの取得を専門とするクラス
      /// </summary>
      public class StoryOrderProvider
      {
          /// <summary>
          /// 再生するオーダーのリスト
          /// </summary>
          private IReadOnlyList<OrderData> _orders;

          /// <summary>
          /// 再生を行うシーンデータのキャッシュのセットアップ
          /// </summary>
          public void Setup(IReadOnlyList<OrderData> orders)
          {
              _orders = orders;
          }

          /// <summary>
          /// 指定位置からAppendが出現するまでの連続オーダーを取得
          /// </summary>
          public List<OrderData> GetContinuousOrdersFrom(int startPosition)
          {
              var orders = new List<OrderData>();
              
              var firstOrder = GetOrderAt(startPosition);
              if (firstOrder == null) return orders;

              // 最初のオーダーを追加
              orders.Add(firstOrder);
              startPosition++;

              // Append以外のオーダーが続く限り取得を継続
              while (IsValidOrderIndex(startPosition))
              {
                  var order = GetOrderAt(startPosition);
                  if (order.Sequence == SequenceType.Append)
                      break;

                  orders.Add(order);
                  startPosition++;
              }

              return orders;
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
          public OrderData GetOrderAt(int orderIndex)
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