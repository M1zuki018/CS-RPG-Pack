using System;
using System.Collections.Generic;
using CryStar.Core;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.Execution;
using CryStar.Story.UI;
using CryStar.Utility;
using CryStar.Utility.Enum;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CryStar.Story.Player
{
    /// <summary>
    /// ストーリー全体の進行を管理するマネージャー
    /// </summary>
    public class StoryPlayer : CustomBehaviour
    {
        /// <summary>
        /// View
        /// </summary>
        [SerializeField] 
        private StoryView _view;
        
        /// <summary>
        /// ストーリーの再生の状態を管理するステートマシン
        /// </summary>
        private StoryPlayerStateMachine _stateMachine;
        
        /// <summary>
        /// 入力処理の監視を担当
        /// </summary>
        private StoryInputHandler _inputHandler;
        
        /// <summary>
        /// ストーリーの進行位置とオーダー取得を担当
        /// </summary>
        private StoryNavigator _navigator;
        
        /// <summary>
        /// オートプレイ、UI非表示などのモード管理を担当
        /// </summary>
        private StoryModeController _modeController;
        
        /// <summary>
        /// オーダー実行担当
        /// </summary>
        private OrderExecutor _orderExecutor;

        #region Lifecycle
        
        /// <summary>
        /// Awake
        /// </summary>
        public override async UniTask OnAwake()
        {
            await base.OnAwake();
            InitializeComponents();
        }
        
        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            // 入力を監視
            _inputHandler.HandleInput();
            
            if (_stateMachine.IsPlaying)
            {
                // ストーリー再生中のみ、オートモードの監視を行う
                _modeController.HandleAutoPlay(_orderExecutor.IsExecuting);
            }
        }
        
        /// <summary>
        /// Destroy
        /// </summary>
        private void OnDestroy()
        {
            _inputHandler?.Dispose();
            _navigator?.Dispose();
            _orderExecutor?.Dispose();
            _modeController?.Dispose();
        }
        
        #endregion
        
        /// <summary>
        /// ストーリー再生を開始する
        /// </summary>
        public void PlayStory(StorySceneData sceneData, IReadOnlyList<OrderData> orders, Action endAction)
        {
            // 各コンポーネントのセットアップ
            _navigator.Setup(orders);
            _orderExecutor.Setup(() =>
            {
                endAction?.Invoke();
                _stateMachine.ResetStory();
            });

            // ビューのセットアップ
            _view.SetupCharacter(sceneData.CharacterScale, sceneData.PositionCorrection);

            // ストーリー開始
            _stateMachine.StartStory();
        }

        #region Private Methods
        
        /// <summary>
        /// 各コンポーネントの初期化
        /// </summary>
        private void InitializeComponents()
        {
            // 各専門コンポーネントの初期化
            _stateMachine = new StoryPlayerStateMachine(OnStateChanged);
            _inputHandler = new StoryInputHandler(_stateMachine, ProcessNextOrder);
            _navigator = new StoryNavigator(HandleComplete);
            _modeController = new StoryModeController(ProcessNextOrder, _view.SetImmerseMode, _view.SetAutoPlayMode);
            _orderExecutor = new OrderExecutor(_view, HandleBranchOrResume);

            // ビューの初期化
            _view.SetupChoice(HandleChoicePause);
            _view.SetupOverlay(HandleSkip, HandleImmerseModeToggle, HandleAutoPlayToggle);
        }
        
        /// <summary>
        /// 次のオーダーを実行する
        /// </summary>
        private void ProcessNextOrder()
        {
            if (!_stateMachine.CanProcessInput)
            {
                // 入力を受け付けない状態であればreturn
                return;
            }

            // オート再生がリクエストされていたらキャンセル
            _modeController.CancelAutoPlayIfRequested();

            if (_orderExecutor.IsExecuting)
            {
                // 実行中であればスキップ
                _orderExecutor.Skip();
            }
            else
            {
                // 実行中でなければ次のオーダー群の実行を行う
                ExecuteNextOrderSequence();
            }
        }

        /// <summary>
        /// 次のオーダーの実行を行う
        /// </summary>
        private void ExecuteNextOrderSequence()
        {
            var orders = _navigator.GetNextOrderSequence();
            
            if (orders.Count > 0)
            {
                _orderExecutor.Execute(orders).Forget();
            }
            else
            {
                LogUtility.Error("次のオーダーが見つかりません", LogCategory.System);
            }
        }
        
        /// <summary>
        /// ステートが切り替わった時のイベント
        /// </summary>
        private void OnStateChanged(PlaybackStateType oldState, PlaybackStateType newState)
        {
            LogUtility.Verbose($"Story ステート変更: {oldState} -> {newState}", LogCategory.System);
        }

        /// <summary>
        /// 選択肢表示中はポーズ状態にする
        /// </summary>
        private void HandleChoicePause()
        {
            _stateMachine.PauseStory();
        }

        /// <summary>
        /// 選択肢の分岐処理とストーリー再開を行う
        /// </summary>
        private void HandleBranchOrResume(int orderIndex = -1)
        {
            if (orderIndex != -1)
            {
                // 分岐したオーダーに遷移する
                _navigator.JumpToOrder(orderIndex);
            }

            // ポーズ状態を解除
            _stateMachine.ResumeStory();
            
            // 次のオーダーを実行
            ProcessNextOrder();
        }

        /// <summary>
        /// ストーリースキップ処理
        /// </summary>
        private void HandleSkip()
        {
            // 最後のオーダーへジャンプ
            _navigator.JumpToEnd();
            
            if (_orderExecutor.IsExecuting)
            {
                // ストーリーが実行中であれば強制的にスキップする
                _orderExecutor.Skip();
            }
            
            // 最後のオーダーを実行する
            ExecuteNextOrderSequence();
        }

        /// <summary>
        /// UI非表示モードに変更する
        /// </summary>
        private void HandleImmerseModeToggle()
        {
            _stateMachine.ToggleImmerseMode();
            _modeController.ToggleImmerseMode();
        }

        /// <summary>
        /// オート再生モードに変更する
        /// </summary>
        private void HandleAutoPlayToggle()
        {
            _modeController.ToggleAutoPlayMode();
        }

        /// <summary>
        /// ストーリー再生終了
        /// </summary>
        private void HandleComplete()
        {
            _stateMachine.CompleteStory();
        }
        
        #endregion
    }
}
