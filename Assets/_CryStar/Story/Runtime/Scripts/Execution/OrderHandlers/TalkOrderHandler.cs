using CryStar.Story.Attributes;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.Story.UI;
using DG.Tweening;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// Talk - キャラクターのセリフ表示
    /// </summary>
    [OrderHandler(OrderType.Talk)]
    public class TalkOrderHandler : OrderHandlerBase
    {
        public override OrderType SupportedOrderType => OrderType.Talk;
        
        public override Tween HandleOrder(OrderData data, StoryView view)
        {
            // テキストの上書きスピードが設定されていたらそのスピードを使用
            // 設定されていない場合は定数を使用する
            var multiply = data.OverrideTextSpeed != 0 ? data.OverrideTextSpeed : data.CharacterData.TextSpeed;
            
            // テキスト更新にかける時間を計算
            var duration = data.DialogText.Length * multiply;
            
            // 名前付きのダイアログを表示する
            return view.SetTalk(data.DisplayName, data.DialogText, duration);
        }
    }
}