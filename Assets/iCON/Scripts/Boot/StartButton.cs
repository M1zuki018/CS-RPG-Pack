using iCON.System;
using UnityEngine;
using UnityEngine.UI;

namespace iCON.Boot
{
    /// <summary>
    /// スタートボタン
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class StartButton : MonoBehaviour
    {
        [SerializeField] private SceneSelector _sceneSelector;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(HandleStart);
        }

        private void HandleStart()
        {
            SceneLoader.Instance.LoadScene(_sceneSelector.SelectedSceneIndex);
        }
    }

}
