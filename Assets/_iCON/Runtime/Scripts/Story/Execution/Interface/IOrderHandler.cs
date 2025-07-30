using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// OrderHandlerが継承すべきインターフェース
    /// </summary>
    public interface IOrderHandler
    {
        /// <summary>
        /// このハンドラが担当するオーダーの種類
        /// </summary>
        OrderType SupportedOrderType { get; }
        
        /// <summary>
        /// オーダーを実行する
        /// </summary>
        Tween HandleOrder(OrderData data, StoryView view);
    }
}