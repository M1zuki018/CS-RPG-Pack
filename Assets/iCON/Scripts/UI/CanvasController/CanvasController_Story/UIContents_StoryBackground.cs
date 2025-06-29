using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// UIContents ストーリーの背景
    /// </summary>
    public class UIContents_StoryBackground : MonoBehaviour
    {
        /// <summary>
        /// 背景画像
        /// </summary>
        [SerializeField] 
        private CustomImage[] _bgImages = new CustomImage[2];

        /// <summary>
        /// 現在アクティブな背景画像のインデックス
        /// </summary>
        private int _activeImageIndex = 0;

        /// <summary>
        /// 次に使用する背景画像のインデックス
        /// </summary>
        private int NextImageIndex => (_activeImageIndex + 1) % _bgImages.Length;

        #region Lifecycle

        private void Awake()
        {
            InitializeBackgroundImages();
        }

        #endregion

        /// <summary>
        /// ファイル名を元に画像を変更する
        /// </summary>
        public void SetImage(string fileName)
        {
            // 次の画像を準備
            int nextIndex = NextImageIndex;
            _bgImages[nextIndex].AssetName = fileName;
            
            // オブジェクトをhierarchyの末尾に移動させて、最前面に表示されるようにする
            _bgImages[nextIndex].transform.SetAsLastSibling();
            
            // アクティブインデックスを更新
            _activeImageIndex = nextIndex;
        }

        #region Private Method

        /// <summary>
        /// 背景画像コンポーネントの初期化
        /// </summary>
        private void InitializeBackgroundImages()
        {
            for (int i = 0; i < _bgImages.Length; ++i)
            {
                if (_bgImages[i] == null)
                {
                    // 配列がnullなら子オブジェクトから取得する
                    _bgImages[i] = transform.GetChild(i).GetComponent<CustomImage>();
                }
            }
        }

        #endregion
    }
}