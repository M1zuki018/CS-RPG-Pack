using System;
using System.Collections.Generic;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// UIContents 選択肢表示
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIContents_Choice : MonoBehaviour
    {
        /// <summary>
        /// 選択肢のボタンのプレハブ
        /// </summary>
        [SerializeField] 
        private CustomButton _choiceButtonPrefab;

        /// <summary>
        /// CanvasGroup
        /// </summary>
        private CanvasGroup _canvasGroup;
        
        #region Lifecycle

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            SetVisibility(false);
        }

        #endregion

        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(IReadOnlyList<ViewData> choiceViewDataList)
        {
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

        #region Private Methods

        /// <summary>
        /// 選択肢の表示/非表示を切り替える
        /// </summary>
        private void SetVisibility(bool isActive)
        {
            _canvasGroup.alpha = isActive ? 1 : 0;
            _canvasGroup.interactable = isActive;
            _canvasGroup.blocksRaycasts = isActive;
        }

        #endregion
        
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