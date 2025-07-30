using System;
using CryStar.Story.Enums;

namespace CryStar.Story.Player
{
    /// <summary>
    /// ストーリーの再生の状態を管理するステートマシン
    /// </summary>
    public class StoryPlayerStateMachine
    {
        /// <summary>
        /// ステート変更時のコールバック
        /// </summary>
        private event Action<PlaybackStateType, PlaybackStateType> _onStateChanged;
        
        /// <summary>
        /// 現在のステート
        /// </summary>
        private PlaybackStateType _currentState = PlaybackStateType.Idle;

        #region Public Property

        /// <summary>
        /// 現在のステート
        /// </summary>
        public PlaybackStateType CurrentState => _currentState;
        
        /// <summary>
        /// 再生中
        /// </summary>
        public bool IsPlaying => _currentState == PlaybackStateType.Playing;
        
        /// <summary>
        /// 入力処理が可能かどうか
        /// </summary>
        public bool CanProcessInput => _currentState == PlaybackStateType.Playing;
        
        /// <summary>
        /// ストーリーが完了しているかどうか
        /// </summary>
        public bool IsComplete => _currentState == PlaybackStateType.Complete;
        
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StoryPlayerStateMachine(Action<PlaybackStateType, PlaybackStateType> onStateChanged)
        {
            _onStateChanged = onStateChanged;
        }
        
        /// <summary>
        /// ストーリーを開始する
        /// </summary>
        public bool StartStory() => TransitionTo(PlaybackStateType.Playing);
        
        /// <summary>
        /// ストーリーを一時停止する
        /// </summary>
        public bool PauseStory() => TransitionTo(PlaybackStateType.Paused);
        
        /// <summary>
        /// ストーリーを再開する
        /// </summary>
        public bool ResumeStory() => TransitionTo(PlaybackStateType.Playing);
        
        /// <summary>
        /// ストーリーを完了状態にする
        /// </summary>
        public bool CompleteStory() => TransitionTo(PlaybackStateType.Complete);
        
        /// <summary>
        /// UI非表示モードを切り替える
        /// </summary>
        public bool ToggleImmerseMode() => TransitionTo(_currentState == PlaybackStateType.Immersed ? PlaybackStateType.Playing : PlaybackStateType.Immersed);
        
        /// <summary>
        /// ストーリーをリセットして待機状態にする
        /// </summary>
        public bool ResetStory() => TransitionTo(PlaybackStateType.Idle);

        #region Private Methods

        /// <summary>
        /// 指定した状態に変更する
        /// </summary>
        private bool TransitionTo(PlaybackStateType newState)
        {
            // 状態遷移が無効・状態の変更がない場合は遷移失敗。falseを返す
            if (!IsValidTransition(_currentState, newState) || _currentState == newState)
            {
                return false;
            }
            
            // ステートを更新
            var oldState = _currentState;
            _currentState = newState;
            
            // ステート変更時のイベントを実行
            _onStateChanged?.Invoke(oldState, newState);
            
            return true;
        }
        
        /// <summary>
        /// 状態遷移が有効かどうかを判定する
        /// </summary>
        private bool IsValidTransition(PlaybackStateType from, PlaybackStateType to)
        {
            return (from, to) switch
            {
                // 待機状態からは再生開始のみ可能
                (PlaybackStateType.Idle, PlaybackStateType.Playing) => true,
                
                // 再生中からは一時停止、没入モード、完了が可能
                (PlaybackStateType.Playing, PlaybackStateType.Paused) => true,
                (PlaybackStateType.Playing, PlaybackStateType.Immersed) => true,
                (PlaybackStateType.Playing, PlaybackStateType.Complete) => true,
                
                // 一時停止中からは再開、完了が可能
                (PlaybackStateType.Paused, PlaybackStateType.Playing) => true,
                (PlaybackStateType.Paused, PlaybackStateType.Complete) => true,
                
                // 没入モードからは通常再生、完了が可能
                (PlaybackStateType.Immersed, PlaybackStateType.Playing) => true,
                (PlaybackStateType.Immersed, PlaybackStateType.Complete) => true,
                
                // 完了状態からは待機状態へのリセットのみ可能
                (PlaybackStateType.Complete, PlaybackStateType.Idle) => true,
                
                // その他の遷移は無効
                _ => false
            };
        }

        #endregion
    }
}