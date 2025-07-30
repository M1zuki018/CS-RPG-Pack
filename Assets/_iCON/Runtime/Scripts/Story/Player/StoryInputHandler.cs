using System;
using UnityEngine;

namespace CryStar.Story.Player
{
    /// <summary>
    /// ストーリー進行の入力処理を担当
    /// </summary>
    public class StoryInputHandler : IDisposable
    {
        /// <summary>
        /// 次のオーダーに進むことが要求された時のイベント
        /// </summary>
        private event Action _onNextOrderRequested;

        /// <summary>
        /// 再生状態を管理するステートマシン
        /// </summary>
        private readonly StoryPlayerStateMachine _stateMachine;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StoryInputHandler(StoryPlayerStateMachine stateMachine, Action onNextOrderRequested)
        {
            _stateMachine = stateMachine;
            _onNextOrderRequested = onNextOrderRequested;
        }

        /// <summary>
        /// 入力処理の監視を行う
        /// </summary>
        public void HandleInput()
        {
            if (!_stateMachine.CanProcessInput)
            {
                // 入力処理を受け付けない状態であればreturn
                return;
            }

            // スペースキーで次のオーダーへ
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _onNextOrderRequested?.Invoke();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _onNextOrderRequested = null;
        }
    }
}