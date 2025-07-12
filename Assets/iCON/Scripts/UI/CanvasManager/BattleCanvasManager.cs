using System;

namespace iCON.UI
{
    /// <summary>
    /// バトルシーンのCanvasManager
    /// </summary>
    public class BattleCanvasManager : SceneCanvasManagerBase
    {
        private void Awake()
        {
            ServiceLocator.Resister(this, ServiceType.Local);
        }
    }

}
