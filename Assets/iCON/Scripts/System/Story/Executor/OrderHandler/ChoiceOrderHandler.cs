using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// Choice - 選択肢表示
    /// </summary>
    [OrderHandler(OrderType.Choice)]
    public class ChoiceOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Choice;
        
        public override UniTask<Tween> HandleAsync(OrderData data, StoryView view)
        {
            // TODO
            return new UniTask<Tween>();
        }
    }
}