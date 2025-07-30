using CryStar.Attribute;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// PackSample_CanvasController_StorySelect
    /// </summary>
    public partial class PackSample_CanvasController_StorySelect : WindowBase
    {
        [SerializeField, HighlightIfNull] private PackSample_StoryPlayButton[] _buttons;

        public override UniTask OnStart()
        {
            Setup();
            return base.OnStart();
        }
        
        public void Setup()
        {
            
        }
    }
}