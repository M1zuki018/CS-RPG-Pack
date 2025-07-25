using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// Effect - エフェクト再生
    /// </summary>
    [OrderHandler(OrderType.Effect)]
    public class EffectOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Effect;
        
        public override UniTask<Tween> HandleAsync(OrderData data, StoryView view)
        {
            // TODO
            return new UniTask<Tween>();
        }
    }
}