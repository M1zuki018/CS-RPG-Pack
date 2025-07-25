using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// ChangeBackground - 背景変更
    /// </summary>
    [OrderHandler(OrderType.ChangeBackground)]
    public class ChangeBackgroundOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.ChangeBackground;
        
        public override async UniTask<Tween> HandleAsync(OrderData data, StoryView view)
        {
            return await view.SetBackground(data.FilePath, data.Duration);
        }
    }
}