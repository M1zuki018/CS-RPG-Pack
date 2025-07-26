using System.Threading;
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
    public class ChangeBackgroundOrderHandler : AsyncOrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.ChangeBackground;
        
        public override async UniTask<Tween> HandleOrderAsync(OrderData data, StoryView view, CancellationToken cancellationToken)
        {
            return await view.SetBackground(data.FilePath, data.Duration);
        }
    }
}