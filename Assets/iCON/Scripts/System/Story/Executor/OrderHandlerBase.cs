using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// オーダーを実行するHandlerのベースクラス
    /// </summary>
    public abstract class OrderHandlerBase
    {
        public abstract OrderType SupportedOrderType { get; }
        public abstract Tween HandleOrder(OrderData data, StoryView view);
    }
}
