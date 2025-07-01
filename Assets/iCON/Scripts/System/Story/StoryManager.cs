
using Cysharp.Threading.Tasks;
using iCON.UI;
using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// ストーリー全体の進行を管理するマネージャー
    /// </summary>
    public class StoryManager : ViewBase
    {
        /// <summary>
        /// View
        /// </summary>
        [SerializeField] 
        private StoryView _view;
        
        /// <summary>
        /// ストーリーを進行させる
        /// </summary>
        private StoryProgress _progress;
        
        /// <summary>
        /// オーダーを実行する
        /// </summary>
        private OrderExecutor _orderExecutor;

        #region Lifecycle
        
        public override async UniTask OnAwake()
        {
            await base.OnAwake();
            _progress = new StoryProgress();
            _orderExecutor = new OrderExecutor(_view);

            await _progress.Setup();
        }
        
        #endregion

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextOrder();
            }
        }

        /// <summary>
        /// 次のオーダーに進む
        /// </summary>
        private void NextOrder()
        {
            var order = _progress.NextOrder();
            _orderExecutor.Execute(order);
        }
        
        /// <summary>
        /// 次のシーンに進む
        /// </summary>
        private void NextScene() => _progress.NextScene();
        
        /// <summary>
        /// 次のチャプターに進む
        /// </summary>
        private void NextChapter() => _progress.NextChapter();
        
        /// <summary>
        /// 次のパートに進む
        /// </summary>
        private void NextPart() => _progress.NextPart();
    }
}
