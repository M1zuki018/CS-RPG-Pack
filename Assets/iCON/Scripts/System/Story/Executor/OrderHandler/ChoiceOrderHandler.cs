using System;
using System.Collections.Generic;
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
        private Action<int> _choiceAction;
        public override OrderType SupportedOrderType => OrderType.Choice;
        
        public void SetChoiceAction(Action<int> choiceAction)
        {
            _choiceAction = choiceAction;
        }
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            // ViewDataのリストを作成しておく
            var viewDataList = new List<UIContents_Choice.ViewData>();
            
            var text = data.DialogText.Split(',');
            viewDataList.Add(new UIContents_Choice.ViewData(text[0], () => _choiceAction?.Invoke(int.Parse(text[1]))));
            viewDataList.Add(new UIContents_Choice.ViewData(text[2], () => _choiceAction?.Invoke(int.Parse(text[3]))));
            
            view.Choice(viewDataList);
            return null;
        }
    }
}