using System;
using Cysharp.Threading.Tasks;

namespace CryStar.Story.Player
{
    /// <summary>
    /// オートプレイ、UI非表示などのモード管理
    /// </summary>
    public class StoryModeController : IDisposable
    {
        /// <summary>
        /// UI非表示モードが切り替わったときのイベント
        /// </summary>
        private event Action<bool> _onImmerseModeChanged;
        
        /// <summary>
        /// オート再生モードが切り替わったときのイベント
        /// </summary>
        private event Action<bool> _onAutoPlayModeChanged;
        
        /// <summary>
        /// オート再生のコントローラー
        /// </summary>
        private StoryAutoPlayController _autoPlayController;
        
        /// <summary>
        /// UI非表示モードか
        /// </summary>
        private bool _isImmerseMode = false;

        /// <summary>
        /// UI非表示モードか
        /// </summary>
        public bool IsImmerseMode => _isImmerseMode;
        
        /// <summary>
        /// オート再生モードか
        /// </summary>
        public bool IsAutoPlayMode => _autoPlayController?.IsAutoPlayMode ?? false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StoryModeController(Action processNextOrderCallback,
            Action<bool> onImmerseModeChanged, Action<bool> onAutoPlayModeChanged)
        {
            _autoPlayController = new StoryAutoPlayController(processNextOrderCallback);
            _onImmerseModeChanged = onImmerseModeChanged;
            _onAutoPlayModeChanged = onAutoPlayModeChanged;
        }

        /// <summary>
        /// UI非表示モードを切り替える
        /// </summary>
        public void ToggleImmerseMode()
        {
            _isImmerseMode = !_isImmerseMode;
            _onImmerseModeChanged?.Invoke(_isImmerseMode);
        }

        /// <summary>
        /// オート再生モードを切り替える
        /// </summary>
        public void ToggleAutoPlayMode()
        {
            bool isAutoPlay = _autoPlayController.ToggleAutoPlayMode();
            _onAutoPlayModeChanged?.Invoke(isAutoPlay);
        }

        /// <summary>
        /// オート再生モードの実行管理
        /// </summary>
        public void HandleAutoPlay(bool isExecuting)
        {
            if (!isExecuting && _autoPlayController.NotYetRequest)
            {
                _autoPlayController.AutoPlay().Forget();
            }
        }

        /// <summary>
        /// オート再生のリクエストをキャンセルする
        /// </summary>
        public void CancelAutoPlayIfRequested()
        {
            if (_autoPlayController.IsAutoPlayReserved)
            {
                _autoPlayController.CancelAutoPlay();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _autoPlayController?.Dispose();
        }
    }
}