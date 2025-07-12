using iCON.Enums;

namespace iCON.UI
{
    /// <summary>
    /// バトルシーンのCanvasManager
    /// </summary>
    public class BattleCanvasManager : SceneCanvasManagerBase
    {
        public void ShowCanvas(BattleCanvasType canvasType)
        {
            base.ShowCanvas((int)canvasType);
        }
    }

}
