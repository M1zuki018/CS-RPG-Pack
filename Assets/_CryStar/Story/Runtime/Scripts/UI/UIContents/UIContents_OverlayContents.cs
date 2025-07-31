using System;
using CryStar.UI;
using CryStar.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace CryStar.Story.UI
{
    /// <summary>
    /// UIContents ストーリー画面のオーバーレイ上のUIを管理
    /// </summary>
    public class UIContents_OverlayContents : UIContentsBase
    {
        /// <summary>
        /// ボタンのデフォルト色
        /// </summary>
        [Header("カラー設定"), SerializeField]
        private Color _defaultColor;
        
        /// <summary>
        /// ボタンの選択中の色
        /// </summary>
        [SerializeField]
        private Color _activeColor = Color.gray;
        
        /// <summary>
        /// UI非表示ボタン
        /// </summary>
        [Header("ボタンの参照"), SerializeField]
        private Button _immersedButton;

        /// <summary>
        /// オート再生ボタン
        /// </summary>
        [SerializeField]
        private Button _autoPlayButton;
        
        /// <summary>
        /// スキップボタン
        /// </summary>
        [SerializeField]
        private Button _skipButton;
        
        // 特に初期化処理はなし
        public override void Initialize() { }
        
        private void OnDestroy()
        {
            ClearAllListeners();
        }

        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(Action skipAction, Action onImmersiveAction, Action onAutoPlayAction)
        {
            // スキップボタンを押した時の処理を登録
            SetupButton(_skipButton, skipAction);
            
            // UI非表示ボタン
            SetupButton(_immersedButton, onImmersiveAction);
            
            // オート再生ボタン
            SetupButton(_autoPlayButton, onAutoPlayAction);
        }

        /// <summary>
        /// UI非表示ボタンの色を変更する
        /// </summary>
        public void ChangeImmerseButtonColor(bool isActive)
        {
            ChangeButtonActive(_immersedButton, isActive);
        }

        /// <summary>
        /// オート再生ボタンの色を変更する
        /// </summary>
        public void ChangeAutoPlayButtonColor(bool isActive)
        {
            ChangeButtonActive(_autoPlayButton, isActive);
        }

        #region Private Methods
        
        /// <summary>
        /// ボタンのonClickイベントに安全に処理を追加する
        /// </summary>
        private void SetupButton(Button button, Action onClick)
        {
            button.onClick.SafeAddListener(() => onClick?.Invoke());
        }

        /// <summary>
        /// ボタンの状態（色）を変更する
        /// </summary>
        private void ChangeButtonActive(Button button, bool isActive)
        {
            if (button?.image == null)
            {
                // ボタンのImageの参照がない場合はreturn
                return;
            }
            
            button.image.color = isActive ? _activeColor : _defaultColor;
        }

        /// <summary>
        /// 全リスナーをクリア
        /// </summary>
        private void ClearAllListeners()
        {
            _immersedButton.onClick.RemoveAllListeners();
            _autoPlayButton.onClick.RemoveAllListeners();
            _skipButton.onClick.RemoveAllListeners();
        }
        
        #endregion
    }
}