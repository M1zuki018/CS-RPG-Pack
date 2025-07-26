using System;
using iCON.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace iCON.UI
{
    [RequireComponent(typeof(Button))]
    public class ChoiceButton : MonoBehaviour
    {
        /// <summary>
        /// ボタン
        /// </summary>
        private Button _button;

        public void Setup(string message, Action clickAction)
        {
            _button = GetComponent<Button>();
            var text = _button.GetComponentInChildren<Text>();
            
            // テキストを設定
            text.text = message;
            _button.onClick.SafeAddListener(() => clickAction?.Invoke());
        }

        /// <summary>
        /// Destroy
        /// </summary>
        private void OnDestroy()
        {
            _button.onClick.SafeRemoveAllListeners();
        }
    }
}