using Cysharp.Threading.Tasks;
using iCON.Performance;
using iCON.Utility;
using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// タイトルシーンのマネージャー
    /// </summary>
    public class TitleSceneManager : ViewBase
    {
        /// <summary>
        /// タイトルスプラッシュの演出マネージャー
        /// </summary>
        [SerializeField, HighlightIfNull]
        private TitleSplashManager _titleSplashManager;
        
        /// <summary>
        /// Start
        /// </summary>
        public override UniTask OnStart()
        {
            if (_titleSplashManager == null)
            {
                LogUtility.Error("TitleSplashManagerがアサインされていません", LogCategory.System);
                return base.OnStart();
            }
            
            _titleSplashManager.gameObject.SetActive(true);
            
            // タイトルスプラッシュの演出を開始
            _titleSplashManager.Play();
            return base.OnStart();
        }
        
        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                _titleSplashManager.Skip();
            }
        }
    }
}