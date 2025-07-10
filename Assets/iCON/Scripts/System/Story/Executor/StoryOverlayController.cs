using System;
using System.Threading;
using iCON.UI;
using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// ストーリーのオーバーレイを管理する
    /// </summary>
    public class StoryOverlayController : MonoBehaviour
    {
        /// <summary>
        /// オーバーレイ上のUIボタンを管理するクラス
        /// </summary>
        [SerializeField]
        private UIContents_OverlayContents _overlayContents;
        
        /// <summary>
        /// View
        /// </summary>
        private StoryView _view;
        
        /// <summary>
        /// オート再生をキャンセルするアクション
        /// </summary>
        private Action _cancelAutoPlayAction;
        
        /// <summary>
        /// UI非表示モード
        /// </summary>
        private bool _isImmerseMode = false;
        
        /// <summary>
        /// オート再生モード
        /// </summary>
        private bool _autoPlayMode = false;
        
        /// <summary>
        /// UI非表示モード
        /// </summary>
        public bool IsImmerseMode => _isImmerseMode;
        
        /// <summary>
        /// オート再生モード
        /// </summary>
        public bool AutoPlayMode => _autoPlayMode;
        
        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(StoryView view, Action cancelAutoPlayAction)
        {
            _view = view;
            _cancelAutoPlayAction = cancelAutoPlayAction;
            
            // UI非表示ボタン
            _overlayContents.SetupImmerseButton(HandleClickImmerseButton);
            
            // オート再生ボタン
            _overlayContents.SetupAutoPlayButton(HandleClickAutoPlayButton);
        }
        
        /// <summary>
        /// UI非表示ボタンが押されたときの処理
        /// </summary>
        private void HandleClickImmerseButton()
        {
            // UI非表示状態かフラグを切り替える
            _isImmerseMode = !_isImmerseMode;

            if (_isImmerseMode)
            {
                // 非表示状態であれば、ダイアログを非表示にする
                _view.HideDialog();
            }
            else
            {
                _view.ShowDialog();
            }
        }
        
        /// <summary>
        /// オート再生ボタンが押されたときの処理
        /// </summary>
        private void HandleClickAutoPlayButton()
        {
            _autoPlayMode = !_autoPlayMode;

            if (!_autoPlayMode)
            {
                // オートプレイをやめるアクションを実行
                _cancelAutoPlayAction?.Invoke();
            }
        }
    }
}
