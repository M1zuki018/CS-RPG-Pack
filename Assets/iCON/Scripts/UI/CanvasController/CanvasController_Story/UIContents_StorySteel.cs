using System;
using iCON.Utility;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// UIContents ストーリーのスチル表示を管理する
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIContents_StorySteel : MonoBehaviour
    {
        /// <summary>
        /// スチル画像
        /// </summary>
        [SerializeField] 
        private CustomImage[] _steelImages = new CustomImage[2];
        
        /// <summary>
        /// CanvasGroup
        /// </summary>
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// 現在アクティブなスチル画像のインデックス
        /// </summary>
        private int _activeImageIndex = 0;

        /// <summary>
        /// 次に使用するスチル画像のインデックス
        /// </summary>
        private int NextSteelIndex => (_activeImageIndex + 1) % _steelImages.Length;
        
        /// <summary>
        /// 現在表示中かどうか
        /// </summary>
        public bool IsVisible => _canvasGroup != null && _canvasGroup.alpha > 0;
        
        #region Lifecycle

        private void Awake()
        {
            InitializeSteelImages();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            // 初期状態で非表示にしておく
            SetVisibility(false);
        }

        #endregion

        /// <summary>
        /// ファイル名を元に画像を変更する
        /// </summary>
        public void SetImage(string fileName)
        {
            try
            {
                // 次の画像を準備
                int nextIndex = NextSteelIndex;
                _steelImages[nextIndex].AssetName = fileName;

                // オブジェクトをhierarchyの末尾に移動させて、最前面に表示されるようにする
                _steelImages[nextIndex].transform.SetAsLastSibling();

                // アクティブインデックスを更新
                _activeImageIndex = nextIndex;

                // CanvasGroupが非表示だったら表示する
                if (!IsVisible)
                {
                    SetVisibility(true);
                }
            }
            catch (Exception ex)
            {
                LogUtility.Error($"画像設定中にエラーが発生しました: {ex.Message}", LogCategory.UI, _steelImages[_activeImageIndex]);
            }
        }

        /// <summary>
        /// 非表示にする
        /// </summary>
        public void Hide()
        {
            SetVisibility(false);
        }
        
        #region Private Methods

        /// <summary>
        /// スチル画像コンポーネントの初期化
        /// </summary>
        private void InitializeSteelImages()
        {
            for (int i = 0; i < _steelImages.Length; ++i)
            {
                if (_steelImages[i] == null)
                {
                    // 配列がnullなら子オブジェクトから取得する
                    _steelImages[i] = transform.GetChild(i).GetComponent<CustomImage>();
                }
            }
        }
        
        /// <summary>
        /// 表示状態を設定する
        /// </summary>
        private void SetVisibility(bool isActive)
        {
            _canvasGroup.alpha = isActive ? 1 : 0;
            _canvasGroup.interactable = isActive;
            _canvasGroup.blocksRaycasts = isActive;
            
            foreach (var steel in _steelImages)
            {
                // 各画像も非表示にしておく
                steel.Hide();
            }
        }

        #endregion
    }
}