using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// <summary>
        /// DialogTextで分割に使用している文字
        /// </summary>
        private const char DELIMITER = ',';
        
        /// <summary>
        /// 選択肢のパラメーターの個数
        /// </summary>
        private const int CHOICE_DATA_PAIR_SIZE = 2;
        
        /// <summary>
        /// 選択肢を選んだ時に実行するAction
        /// </summary>
        private Action<int> _choiceAction;
        public override OrderType SupportedOrderType => OrderType.Choice;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            if (data?.DialogText == null)
            {
                throw new ArgumentNullException(nameof(data), "OrderData または DialogText が null です");
            }
            
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view), "StoryView が null です");
            }
            
            var viewDataList = CreateChoiceViewDataList(data.DialogText);
            view.SetupChoice(viewDataList);

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
        private List<UIContents_Choice.ViewData> CreateChoiceViewDataList(string dialogText)
        {
            // 入力されている文字列を分割
            var splitText = dialogText.Split(DELIMITER);
            
            // バリデーション
            ValidateChoiceData(splitText);
            
            var viewDataList = new List<UIContents_Choice.ViewData>();
            
            for (int i = 0; i < splitText.Length; i += CHOICE_DATA_PAIR_SIZE)
            {
                var buttonText = splitText[i].Trim();
                var orderIdText = splitText[i + 1].Trim();
                
                if (!TryParseOrderId(orderIdText, out int orderId))
                {
                    throw new FormatException($"オーダーID '{orderIdText}' を整数に変換できません");
                }
                
                viewDataList.Add(new UIContents_Choice.ViewData(
                    buttonText, 
                    () => _choiceAction?.Invoke(orderId)
                ));
            }
            
            return viewDataList;
        }
        
        /// <summary>
        /// 選択肢データの妥当性を検証する
        /// </summary>
        private static void ValidateChoiceData(string[] splitText)
        {
            if (splitText.Length == 0)
            {
                throw new ArgumentException("DialogText が空です");
            }
            
            if (splitText.Length % CHOICE_DATA_PAIR_SIZE != 0)
            {
                throw new ArgumentException(
                    $"DialogText の形式が正しくありません。「ボタンメッセージ,オーダーID」のペアで入力してください。現在の要素数: {splitText.Length}"
                );
            }
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