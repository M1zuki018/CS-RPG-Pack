using CryStar.Story.Attributes;
using CryStar.Story.Constants;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// Descriptive - 地の文・説明文表示
    /// </summary>
    [OrderHandler(OrderType.Descriptive)]
    public class DescriptiveOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Descriptive;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            // テキストの上書きスピードが設定されていたらそのスピードを使用
            // 設定されていない場合は定数を使用する
            var multiply = data.OverrideTextSpeed != 0 ? data.OverrideTextSpeed : KStoryPresentation.DIALOG_TEXT_SPEED;
            
            // テキスト更新にかける時間を計算
            var duration = data.DialogText.Length * multiply;
            
            // 名前なしのダイアログを表示する
            return view.SetDescription(data.DialogText, duration);
        }
    }
}