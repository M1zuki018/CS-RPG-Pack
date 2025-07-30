using UnityEngine;

namespace CryStar.UI
{
    /// <summary>
    /// UIContentsクラスの最もベーシックなベースクラス
    /// </summary>
    public abstract class UIContentsBase : MonoBehaviour, IUIContents
    {
        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Destory
        /// </summary>
        private void OnDestroy()
        {
            Dispose();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// 解除処理
        /// </summary>
        protected virtual void Dispose(){}
    }
}
