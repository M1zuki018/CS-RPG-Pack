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
        private ChoiceButton _choiceButtonPrefab;

        /// <summary>
        /// CanvasGroup
        /// </summary>
        private CanvasGroup _canvasGroup;
        
        #region Lifecycle

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
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
                button.Setup(viewData.Message, () =>
                {
                    // ボタンが押されたとき、ViewDataとして渡されたアクションの実行と、キャンバスグループ非表示処理を行う
                    viewData.ClickAction?.Invoke();
                    SetActive(false);
                });
                SetActive(true);
            }
        }

        #region Private Methods

        private void SetActive(bool isActive)
        {
            _canvasGroup.alpha = isActive ? 1f : 0f;
            _canvasGroup.interactable = isActive;
            _canvasGroup.blocksRaycasts = isActive;
        }

        #endregion
        
        public class ViewData
        {
            public string Message;
            public Action ClickAction;

            public ViewData(string message, Action onClickAction)
            {
                Message = message;
                ClickAction = onClickAction;
            }
        }
    }
}