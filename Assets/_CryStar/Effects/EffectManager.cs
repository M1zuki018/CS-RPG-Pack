using CryStar.Core;
using CryStar.Core.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CryStar.Effects
{
    public class EffectManager : CustomBehaviour
    {
        [SerializeField] private DizzinessEffectController _dizzinessEffectController;

        public override UniTask OnAwake()
        {
            ServiceLocator.Register(this, ServiceType.Local);
            return base.OnAwake();
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