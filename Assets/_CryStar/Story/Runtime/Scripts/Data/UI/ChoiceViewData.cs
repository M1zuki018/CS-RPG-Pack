using System;

namespace CryStar.Story.Data
{
    /// <summary>
    /// 選択肢表示のためのViewData
    /// </summary>
    public class ChoiceViewData
    {
        /// <summary>
        /// 表示する文字列
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// クリックした時のAction
        /// </summary>
        public Action ClickAction { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChoiceViewData(string message, Action onClickAction)
        {
            Message = message;
            ClickAction = onClickAction;
        }
    }
}
