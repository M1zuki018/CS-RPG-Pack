using System;
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
        /// Setup
        /// </summary>
        public void Setup(StoryView view, Action skipAction, Action onImmersiveAction, Action onAutoPlayAction)
        {
            _view = view;
            
            // UI非表示ボタンを押した時の処理を登録
            _overlayContents.SetupImmerseButton(onImmersiveAction);
            
            // オート再生ボタン
            _overlayContents.SetupAutoPlayButton(onAutoPlayAction);
            
            // スキップボタン
            _overlayContents.SetupSkipButton(skipAction);
        }
        
        /// <summary>
        /// UI非表示ボタンが押されたときの処理
        /// </summary>
        public void HandleClickImmerseButton(bool isImmersiveMode)
        {
            // ボタンの色を変える
            _overlayContents.ChangeImmerseButtonColor(isImmersiveMode);

            if (isImmersiveMode)
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
        public void HandleClickAutoPlayButton(bool isAutoPlayMode)
        {
            // ボタンの色を変える
            _overlayContents.ChangeAutoPlayButtonColor(isAutoPlayMode);
        }
    }
}
