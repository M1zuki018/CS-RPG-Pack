using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace iCON.System
{
    /// <summary>
    /// SceneLoader
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;

        private void Awake()
        {
            // シングルトンパターン
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// ロード画面を挟まずに画面遷移を行う
        /// </summary>
        public async UniTask LoadSceneAsync(SceneType sceneType)
        {
            await SceneManager.LoadSceneAsync((int)sceneType);
        }

        /// <summary>
        /// ロード画面を挟み、画面遷移を行う
        /// </summary>
        public async UniTask LoadSceneAsyncWithLoadScreen(SceneType sceneType)
        {
            // 追加でロードシーンを読み込む
            SceneManager.LoadScene(SceneType.Load.ToString(), LoadSceneMode.Additive);
            
            await SceneManager.LoadSceneAsync((int)sceneType);
            
            // ロードシーンをアンロード
        }
    }
}