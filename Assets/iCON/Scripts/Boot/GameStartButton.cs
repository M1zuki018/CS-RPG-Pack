using Cysharp.Threading.Tasks;
using iCON.System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace iCON.UI
{
    /// <summary>
    /// スタートボタン
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class StartButton : MonoBehaviour
    {
        [FormerlySerializedAs("_sceneSelector")] [SerializeField] private SceneSelectionDropdown sceneSelectionDropdown;
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(HandleStart);
        }

        private void HandleStart()
        {
            ServiceLocator.Get<SceneLoader>().LoadSceneAsync(
                new SceneTransitionData((SceneType)sceneSelectionDropdown.SelectedSceneIndex, true, true)).Forget();
        }
    }

}
