using iCON.Enums;

namespace iCON.UI
{
    /// <summary>
    /// バトルシーンのCanvasManager
    /// </summary>
    public class BattleCanvasManager : SceneCanvasManagerBase
    {
        /// <summary>
        /// 現在表示中のウィンドウを取得する
        /// </summary>
        public WindowBase CurrentCanvas => _canvasObjects[_currentCanvasIndex];
        
        /// <summary>
        /// キャンバスを切り替える
        /// </summary>
        public void ShowCanvas(BattleCanvasType canvasType)
        {
            base.ShowCanvas((int)canvasType);
        }
    }

}
