using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Choice - 選択肢表示
    /// </summary>
    [OrderHandler(OrderType.Choice)]
    public class ChoiceOrderHandler : OrderHandlerBase
    {
        private const char SPLIT = '-';
        
        /// <summary>
        /// DialogTextで分割に使用している文字
        /// </summary>
        private const char LINE_SPLIT = ',';
        
        /// <summary>
        /// 選択肢を選んだ時に実行するAction
        /// </summary>
        private Action<int> _choiceAction;
        public override OrderType SupportedOrderType => OrderType.Choice;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            var viewDataList = CreateChoiceViewDataList(data.DialogText, view);
            view.ShowChoices(viewDataList);

            return null;
        }
        
        /// <summary>
        /// 選択肢を選んだ時に実行するActionを登録する
        /// </summary>
        public void SetChoiceAction(Action<int> choiceAction)
        {
            _choiceAction = choiceAction;
        }
        
        /// <summary>
        /// DialogTextに入力されている文字列から選択肢のViewDataリストを作成する
        /// </summary>
        private List<ChoiceViewData> CreateChoiceViewDataList(string dialogText, StoryView view)
        {
            // 入力されている文字列を分割
            var lineTexts = dialogText.Split(LINE_SPLIT);
            
            var viewDataList = new List<ChoiceViewData>();
            
            for (int i = 0; i < lineTexts.Length; i++)
            {
                var splitText = lineTexts[i].Split(SPLIT);
                
                var buttonText = splitText[0].Trim();
                var orderIdText = splitText[1].Trim();
                var backgroundPath = splitText[2].Trim();
                var bgmPath = splitText[3].Trim();
                
                if (!TryParseOrderId(orderIdText, out int orderId))
                {
                    throw new FormatException($"オーダーID '{orderIdText}' を整数に変換できません");
                }
                
                viewDataList.Add(new ChoiceViewData(
                    buttonText, 
                    () =>
                    {
                        _choiceAction?.Invoke(orderId);

                        if (backgroundPath != null)
                        {
                            view.SetBackground(backgroundPath, 0).Forget();
                        }

                        if (bgmPath != null)
                        {
                            AudioManager.Instance.CrossFadeBGM(bgmPath).Forget();
                        }
                        
                    }));
            }
            
            return viewDataList;
        }
        
        /// <summary>
        /// オーダーIDの文字列を整数に変換を試行する
        /// </summary>
        private static bool TryParseOrderId(string orderIdText, out int orderId)
        {
            return int.TryParse(orderIdText, NumberStyles.Integer, CultureInfo.InvariantCulture, out orderId);
        }
    }
}