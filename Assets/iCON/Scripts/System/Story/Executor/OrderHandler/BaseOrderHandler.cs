using DG.Tweening;
using iCON.Enums;
using iCON.UI;
using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// オーダーを実行するHandlerのベースクラス
    /// </summary>
    public abstract class BaseOrderHandler : MonoBehaviour
    {
        public abstract OrderType SupportedOrderType { get; }
        public abstract Tween Handler(OrderData data, StoryView view);
    }
}
