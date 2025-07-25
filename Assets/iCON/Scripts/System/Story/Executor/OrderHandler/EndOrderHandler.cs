using System;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;
using iCON.Utility;

namespace iCON.System
{
    /// <summary>
    /// End - ストーリー終了処理
    /// </summary>
    [OrderHandler(OrderType.End)]
    public class EndOrderHandler : BaseOrderHandler
    {
        /// <summary>
        /// ストーリー終了を通知するコールバック
        /// </summary>
        private Action _endAction;
        
        public override OrderType SupportedOrderType => OrderType.End;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EndOrderHandler(Action endAction)
        {
            _endAction = endAction;
        }
        
        public override Tween Handler(OrderData data, StoryView view)
        {
            // ログを流す
            LogUtility.Verbose("Story ended", LogCategory.System);

            // フェードアウト実行後、ストーリー終了処理を実行する
            var tween = view.FadeOut(data.Duration);
            tween.OnComplete(() => HandleReset(view));
            
            return tween;
        }

        /// <summary>
        /// ストーリー終了時のリセット処理
        /// </summary>
        private void HandleReset(StoryView view)
        {
            // 全キャラクター非表示
            view.HideAllCharacters();
            
            // スチル非表示
            view.HideSteel(0);
            
            //ダイアログをリセット
            view.ResetTalk();
            view.ResetDescription();
            
            _endAction?.Invoke();
        }
    }
}
