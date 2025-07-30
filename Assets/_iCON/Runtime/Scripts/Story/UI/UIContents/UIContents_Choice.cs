using System;
using System.Collections.Generic;
using CryStar.Story.Data;
using CryStar.UI;
using UnityEngine;
using UnityEngine.Pool;

namespace CryStar.Story.UI
{
    /// <summary>
    /// UIContents 選択肢表示
    /// </summary>
    public class UIContents_Choice : UIContentsCanvasGroupBase, IChoice
    {
        /// <summary>
        /// 選択肢のボタンのプレハブ
        /// </summary>
        [SerializeField] 
        private CustomButton _choiceButtonPrefab;
        
        /// <summary>
        /// 再生一時停止のコールバック
        /// </summary>
        private Action _onPauseStoryAction;
        
        /// <summary>
        /// 選択肢ボタンのオブジェクトプール
        /// </summary>
        private IObjectPool<CustomButton> _buttonPool;
        
        /// <summary>
        /// 現在表示中のアクティブなボタンを追跡するリスト
        /// </summary>
        private readonly List<CustomButton> _activeButtons = new List<CustomButton>();
        
        private void OnDestroy()
        {
            _onPauseStoryAction = null;
        }
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            SetVisibility(false);
            InitializeObjectPool();
        }

        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(Action onStopAction)
        {
            _onPauseStoryAction = onStopAction;
        }

        /// <summary>
        /// 選択肢の表示を行う
        /// </summary>
        public void ShowChoices(IReadOnlyList<ChoiceViewData> choiceViewDataList)
        {
            // ストーリーを一時停止
            _onPauseStoryAction?.Invoke();

            // 現在表示されているボタンをすべてプールに返却する
            ReturnButtons();

            // 必要な数だけプールからボタンを取得してセットアップを行う
            foreach (var viewData in choiceViewDataList)
            {
                // オブジェクトプールからボタンを取得する
                var button = _buttonPool.Get();
                ButtonSetup(button, viewData);
            }

            // CanvasGroup表示
            SetVisibility(true);
        }

        #region Private Methods

        /// <summary>
        /// オブジェクトプールの初期化を行う
        /// </summary>
        private void InitializeObjectPool()
        {
            _buttonPool = new ObjectPool<CustomButton>(
                createFunc: Create, // 自身の子オブジェクトに生成
                actionOnGet: (button) => button.SetActive(true), // ゲームオブジェクトを表示する
                actionOnRelease: (button) => button.SetActive(false), // ゲームオブジェクトを非表示にする
                actionOnDestroy: (button) => Destroy(button.gameObject), // 破棄
                collectionCheck: true,
                defaultCapacity: 3,
                maxSize: 10
            );
        }
        
        /// <summary>
        /// オブジェクトプールの新規生成用メソッド
        /// </summary>
        /// <returns></returns>
        private CustomButton Create()
        {
            return Instantiate(_choiceButtonPrefab, transform);
        }
        
        /// <summary>
        /// 現在表示されているボタンをすべてプールに返却する
        /// </summary>
        private void ReturnButtons()
        {
            foreach (var button in _activeButtons)
            {
                // 返却
                _buttonPool.Release(button);
            }

            // リストをクリアしておく
            _activeButtons.Clear();
        }
        
        /// <summary>
        /// ボタンのセットアップ処理
        /// </summary>
        private void ButtonSetup(CustomButton button, ChoiceViewData viewData)
        {
            button.SetText(viewData.Message);
            button.SetClickAction(() =>
            {
                // ボタンが押されたとき、ViewDataとして渡されたアクションの実行と、キャンバスグループ非表示処理を行う
                viewData.ClickAction?.Invoke();
                SetVisibility(false);
            });
            
            // 表示中のボタンのリストに追加
            _activeButtons.Add(button);
        }

        #endregion
    }
}