using DG.Tweening;
using UnityEngine;

namespace CryStar.Effects
{
    /// <summary>
    /// キャンバスを揺らす
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CanvasShaker : MonoBehaviour
    {
        /// <summary>
        /// 揺らす対象のキャンバス
        /// </summary>
        private Canvas _targetCanvas;
        
        /// <summary>
        /// オブジェクトの初期位置
        /// </summary>
        private Vector3 _initializePos;
        
        /// <summary>
        /// Shake演出のシーケンス
        /// </summary>
        private Sequence _shakeSequence;

        #region Lifecycle

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            _targetCanvas = GetComponent<Canvas>();
            _initializePos = _targetCanvas.transform.position;
        }

        /// <summary>
        /// Destroy
        /// </summary>
        private void OnDestroy()
        {
            _shakeSequence?.Kill();
        }

        #endregion

        /// <summary>
        /// 爆発のような激しい振動演出
        /// </summary>
        public Tween ExplosionShake(float duration, float strengthLate)
        {
            _shakeSequence?.Kill();
            _shakeSequence = DOTween.Sequence();
            
            _shakeSequence
                .Append(_targetCanvas.transform.DOShakePosition(duration * 0.2f, strengthLate * 30f, 30, 90f))
                .Append(_targetCanvas.transform.DOShakePosition(duration * 0.5f, strengthLate * 15f, 20, 90f))
                .Append(_targetCanvas.transform.DOShakePosition(duration * 0.3f, strengthLate * 5f, 10, 90f))
                .OnComplete(() => _targetCanvas.transform.position = _initializePos);
            
            return _shakeSequence;
        }
        
        /// <summary>
        /// 地震のような継続的な振動
        /// </summary>
        public Tween EarthquakeShake()
        {
            _shakeSequence?.Kill();
            return _targetCanvas.transform.DOShakePosition(2f, 8f, 40, 30f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => _targetCanvas.transform.position = _initializePos);
        }
    }
}