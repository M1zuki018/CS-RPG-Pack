using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace iCON.UI
{
    /// <summary>
    /// タイトル画面の背景をランダムに抽選するクラス
    /// </summary>
    [RequireComponent(typeof(CustomImage))]
    public class TitleBackground : MonoBehaviour
    {
        /// <summary>
        /// 背景
        /// </summary>
        [SerializeField]
        private CustomImage _background;
        
        /// <summary>
        /// ランダム抽出を行うラベルの文字列
        /// </summary>
        private const string TARGET_LABEL = "Background";

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            if (_background == null)
            {
                // アサインされていなければ自動で取得する
                _background = GetComponent<CustomImage>();
            }
            
            LoadRandomBackground().Forget();
        }

        /// <summary>
        /// 背景画像をランダムにロードする
        /// </summary>
        private async UniTask LoadRandomBackground()
        {
            // ラベルでキー一覧を取得
            var locations = await Addressables.LoadResourceLocationsAsync(TARGET_LABEL).Task;
            
            if (locations.Count > 0)
            {
                // ランダム選択
                int randomIndex = Random.Range(0, locations.Count);
                var selectedBackground = locations[randomIndex].PrimaryKey;
            
                _background.AssetName = selectedBackground;
            }
        }
    }
}
