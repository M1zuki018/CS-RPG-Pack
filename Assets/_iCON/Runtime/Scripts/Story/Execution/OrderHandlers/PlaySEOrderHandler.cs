using System.Threading;
using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.System;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// PlaySE - SEを再生する
    /// </summary>
    [OrderHandler(OrderType.PlaySE)]
    public class PlaySEOrderHandler : AsyncOrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.PlaySE;
        
        public override async UniTask<Tween> HandleOrderAsync(OrderData data, StoryView view, CancellationToken cancellationToken)
        {
            await AudioManager.Instance.PlaySE(data.FilePath, data.OverrideTextSpeed);
            return null;
        }
    }
}