using UnityEngine;

namespace CryStar.Effects
{
    public class EffectsManager : MonoBehaviour
    {
        // TODO: 仮実装
        public static EffectsManager Instance;
        [SerializeField] private DizzinessEffectController _dizzinessEffectController;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// めまいエフェクトを再生/停止する
        /// </summary>
        public void DizzinessEffect(bool isActive)
        {
            if (isActive)
            {
                _dizzinessEffectController.TriggerDizzinessEffect();
            }
            else
            {
                _dizzinessEffectController.StopAndResetEffect();
            }
        }
    }
}