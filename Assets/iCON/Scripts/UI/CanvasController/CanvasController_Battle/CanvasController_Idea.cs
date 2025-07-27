using System;
using CryStar.Attribute;
using Cysharp.Threading.Tasks;
using iCON.Battle;
using iCON.Utility;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// CanvasController_Idea
    /// </summary>
    public partial class CanvasController_Idea : WindowBase
    {
        [SerializeField, HighlightIfNull] private IdeaContents _commandIdeaContents;
        [SerializeField, HighlightIfNull] private IdeaContents _actorIdeaContents;
        [SerializeField, HighlightIfNull] private CustomButton _command;
        [SerializeField, HighlightIfNull] private CustomButton _actor;
        
        /// <summary>
        /// Ideaを選択したときのコールバック
        /// </summary>
        public event Action<int> OnIdeaSelected;
        
        public override UniTask OnAwake()
        {
            _command.onClick.SafeReplaceListener(HandleCommand);
            _actor.onClick.SafeReplaceListener(HandleActor);
            return base.OnAwake();
        }

        public override void Hide()
        {
            base.Hide();
            
            _command.gameObject.SetActive(true);
            _actor.gameObject.SetActive(true);
            
            CanvasSetActive(_commandIdeaContents.CanvasGroup, false);
            CanvasSetActive(_actorIdeaContents.CanvasGroup, false);
            
            // リスナーのクリーンアップ
            CleanupButtonListeners();
        }
        
        /// <summary>
        /// コマンドが選択された場合の処理
        /// </summary>
        private void HandleCommand()
        {
            if (!_actor.IsActive())
            {
                // アクターボタンがアクティブでないときは、両方のボタンを表示する状態に戻したい
                CanvasSetActive(_commandIdeaContents.CanvasGroup, false);
                _actor.gameObject.SetActive(true);
                CleanupCommandButtonListeners();
                return;
            }
            
            _actor.gameObject.SetActive(false);
            CanvasSetActive(_commandIdeaContents.CanvasGroup, true);

            // 既存のリスナーをクリーンアップしてから新しいリスナーを登録
            CleanupCommandButtonListeners();
            SetupCommandButtonListeners();
        }
        
        /// <summary>
        /// アクターが選択された場合の処理
        /// </summary>
        private void HandleActor()
        {
            if (!_command.IsActive())
            {
                // コマンドボタンがアクティブでないときは、両方のボタンを表示する状態に戻したい
                CanvasSetActive(_actorIdeaContents.CanvasGroup, false);
                _command.gameObject.SetActive(true);
                CleanupActorButtonListeners();
                return;
            }
            
            _command.gameObject.SetActive(false);
            CanvasSetActive(_actorIdeaContents.CanvasGroup, true);
            
            // 既存のリスナーをクリーンアップしてから新しいリスナーを登録
            CleanupActorButtonListeners();
            SetupActorButtonListeners();
        }

        /// <summary>
        /// コマンドボタンのリスナーを設定
        /// </summary>
        private void SetupCommandButtonListeners()
        {
            for (int i = 0; i < _commandIdeaContents.IdeaButtons.Count; i++)
            {
                int index = i; // クロージャ問題を回避
                _commandIdeaContents.IdeaButtons[i].onClick.SafeReplaceListener(() => OnIdeaSelected?.Invoke(index));
            }
        }
        
        /// <summary>
        /// アクターボタンのリスナーを設定
        /// </summary>
        private void SetupActorButtonListeners()
        {
            for (int i = 0; i < _actorIdeaContents.IdeaButtons.Count; i++)
            {
                int index = i; // クロージャ問題を回避
                _actorIdeaContents.IdeaButtons[i].onClick.SafeReplaceListener(() => OnIdeaSelected?.Invoke(index));
            }
        }
        
        /// <summary>
        /// コマンドボタンのリスナーをクリーンアップ
        /// </summary>
        private void CleanupCommandButtonListeners()
        {
            for (int i = 0; i < _commandIdeaContents.IdeaButtons.Count; i++)
            {
                _commandIdeaContents.IdeaButtons[i].onClick.SafeRemoveAllListeners();
            }
        }
        
        /// <summary>
        /// アクターボタンのリスナーをクリーンアップ
        /// </summary>
        private void CleanupActorButtonListeners()
        {
            for (int i = 0; i < _actorIdeaContents.IdeaButtons.Count; i++)
            {
                _actorIdeaContents.IdeaButtons[i].onClick.SafeRemoveAllListeners();
            }
        }
        
        /// <summary>
        /// 全てのボタンリスナーをクリーンアップ
        /// </summary>
        private void CleanupButtonListeners()
        {
            CleanupCommandButtonListeners();
            CleanupActorButtonListeners();
        }

        /// <summary>
        /// CanvasGroupの表示/非表示を切り替える
        /// </summary>
        private void CanvasSetActive(CanvasGroup canvasGroup, bool isActive)
        {
            canvasGroup.alpha = isActive ? 1 : 0;
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
        }
        
        private void OnDestroy()
        {
            _command.onClick.SafeRemoveAllListeners();
            _actor.onClick.SafeRemoveAllListeners();
            CleanupButtonListeners();
        }
    }
}