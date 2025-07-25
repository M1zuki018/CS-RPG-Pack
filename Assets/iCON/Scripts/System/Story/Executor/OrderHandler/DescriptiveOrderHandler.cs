using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.Extensions;
using iCON.UI;

namespace iCON.System
{
    /// <summary>
    /// Descriptive - 地の文・説明文表示
    /// </summary>
    [OrderHandler(OrderType.Descriptive)]
    public class DescriptiveOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Descriptive;
        
        public override UniTask<Tween> HandleAsync(OrderData data, StoryView view)
        {
            // 名前なしのダイアログを表示する
            return view.SetDescription(data.DialogText, data.Duration).ToUniTaskWithResult();
        }
    }
}