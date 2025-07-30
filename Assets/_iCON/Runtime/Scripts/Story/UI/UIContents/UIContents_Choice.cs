using System;
using System.Collections.Generic;
using CryStar.UI;
using UnityEngine;

namespace CryStar.Story.UI
{
    /// <summary>
    /// UIContents 選択肢表示
    /// </summary>
    public class UIContents_Choice : UIContentsCanvasGroupBase, IChoice
    {
        /// <summary>
        /// 選択肢のボタンのプレハブ
        /// </summary>
        [SerializeField] 
        private CustomButton _choiceButtonPrefab;
        
        /// <summary>
        /// 再生一時停止のコールバック
        /// </summary>
        private Action _onStopAction;
        
        public override void Initialize()
        {
            base.Initialize();
            SetVisibility(false);
        }

        private void OnDestroy()
        {
            _onStopAction = null;
        }

        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(Action onStopAction)
        {
            _onStopAction = onStopAction;
        }

        /// <summary>
        /// 選択肢の表示を行う
        /// </summary>
        public void ShowChoices(IReadOnlyList<ViewData> choiceViewDataList)
        {
            // 一時停止
            _onStopAction?.Invoke();
            
            foreach (var viewData in choiceViewDataList)
            {
                // 選択肢のボタンを子オブジェクトに生成
                var button = Instantiate(_choiceButtonPrefab, transform);
                
                button.SetText(viewData.Message);
                button.SetClickAction(() =>
                {
                    // ボタンが押されたとき、ViewDataとして渡されたアクションの実行と、キャンバスグループ非表示処理を行う
                    viewData.ClickAction?.Invoke();
                    SetVisibility(false);
                });
            }

            SetVisibility(true);
        }
        
        /// <summary>
        /// 選択肢表示のためのViewData
        /// </summary>
        public class ViewData
        {
            /// <summary>
            /// 表示する文字列
            /// </summary>
            public string Message;

            /// <summary>
            /// クリックした時のAction
            /// </summary>
            public Action ClickAction;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public ViewData(string message, Action onClickAction)
            {
                Message = message;
                ClickAction = onClickAction;
            }
        }
    }
}