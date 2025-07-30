using System;
using System.Collections.Generic;
using CryStar.Story.Data;

namespace CryStar.Story.UI
{
    /// <summary>
    /// 選択肢表示のUIContentsが継承すべきインターフェース
    /// </summary>
    public interface IChoice
    {
        /// <summary>
        /// Setup
        /// </summary>
        void Setup(Action onStopAction);

        /// <summary>
        /// 選択肢の表示を行う
        /// </summary>
        public void ShowChoices(IReadOnlyList<ChoiceViewData> choiceViewDataList);
    }
}