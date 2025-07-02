using System;
using UnityEngine;
using UnityEngine.UI;

namespace iCON.UI
{
    /// <summary>
    /// UIContents ストーリー画面のオーバーレイ上のUIを管理
    /// </summary>
    public class UIContents_OverlayContents : MonoBehaviour
    {
        /// <summary>
        /// UI非表示ボタン
        /// </summary>
        [SerializeField]
        private Button _immersedButton;

        #region Lifecycle

        private void OnDestroy()
        {
            _immersedButton.onClick.RemoveAllListeners();
        }

        #endregion

        /// <summary>
        /// UI非表示ボタンのセットアップ
        /// </summary>
        public void SetupImmerseButton(Action action)
        {
            // NOTE: 非表示になったときはダイアログを非表示に・ストーリーが進行しないようにする
            _immersedButton.onClick.RemoveAllListeners();
            _immersedButton.onClick.AddListener(() => action?.Invoke());
        }
        
        #region Private Methods

        

        #endregion
    }
}