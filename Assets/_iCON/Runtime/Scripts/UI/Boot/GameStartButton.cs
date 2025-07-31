using CryStar.Core;
using Cysharp.Threading.Tasks;
using iCON.System;
using iCON.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace iCON.UI
{
    /// <summary>
    /// ゲーム開始ボタンのコンポーネント
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class GameStartButton : MonoBehaviour
    {
        /// <summary>
        /// 開始シーンを選択するドロップダウン
        /// </summary>
        [SerializeField] 
        private SceneSelectionDropdown _sceneSelector;
        private Button _button;

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.SafeReplaceListener(HandleGameStart);
        }

        /// <summary>
        /// ゲーム開始
        /// </summary>
        private void HandleGameStart()
        {
            ServiceLocator.Get<SceneLoader>().LoadSceneAsync(
                new SceneTransitionData((SceneType)_sceneSelector.SelectedSceneIndex, true, true)).Forget();
        }
    }

}
