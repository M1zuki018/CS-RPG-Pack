using System;
using CryStar.Attribute;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Extensions;

namespace CryStar.Effects
{
    /// <summary>
    /// めまいの演出を行うクラス
    /// </summary>
    public class DizzinessEffectController : MonoBehaviour
    {
        [Header("設定")] 
        [SerializeField] private Volume _postProcessVolume;
        [SerializeField] private Transform _targetTransform;

        [Header("アニメーションの設定")] 
        [SerializeField] private float _effectDuration = 6f;
        [SerializeField] private AnimationCurve _intensityCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("エフェクトのパラメーターの設定")] 
        [SerializeField, Comment("ビネットの最大強度")] private float _maxVignette = 0.4f;
        [SerializeField, Comment("色収差の最大強度")] private float _maxChromaticAberration = 0.25f;
        [SerializeField, Comment("レンズの歪みの最大強度")] private float _maxLensDistortion = -0.1f;
        [SerializeField, Comment("フィルターの色")] private Color _tintColor = new Color(1f, 0.8f, 0.8f, 1f);
        [SerializeField, Comment("彩度調整の最大強度")] private float _maxColorAdjustmentIntensity = -0.1f;
        [SerializeField, Comment("被写体深度の最大値")] private float _maxDepthOfFieldDistance = 10f;
        [SerializeField, Comment("ぼかしの強さの最大値")] private float _maxDepthOfFieldAperture = 0.8f;
        [SerializeField, Comment("モーションブラーの最大強度")] private float _maxMotionBlurIntensity = 0.3f;

        [Header("カメラシェイクの設定")] 
        [SerializeField, Comment("カメラ揺れの最大強度")] private float _maxShakeStrength = 0.3f;
        [SerializeField, Comment("シェイクの振動回数")] private int _shakeVibrato = 5;
        [SerializeField, Comment("シェイクのランダム性（0-180度）")] private float _shakeRandomness = 40f;
        [SerializeField, Comment("カメラの最大回転角度（度）")] private float _maxCameraTilt = 0.1f;

        /// <summary>
        /// ポストプロセスのコンポーネント
        /// </summary>
        private Vignette _vignette;
        private ChromaticAberration _chromaticAberration;
        private LensDistortion _lensDistortion;
        private ColorAdjustments _colorAdjustments;
        private DepthOfField _depthOfField;
        private MotionBlur _motionBlur;

        /// <summary>
        /// VolumeProfileの元の値のキャッシュ
        /// </summary>
        private float _originalVignetteIntensity;
        private float _originalChromaticIntensity;
        private float _originalLensDistortion;
        private float _originalSaturation;
        private float _originalExposure;
        private float _originalDofDistance;
        private float _originalDofAperture;
        private Vector3 _originalCameraPosition;
        private Vector3 _originalCameraRotation;
        
        /// <summary>
        /// Tween
        /// </summary>
        private Tween _currentShakeTween;
        private Tween _currentTiltTween;
        private Tween _effectSequence;
        
        /// <summary>
        /// UniTask制御用のCompletionSource
        /// </summary>
        private UniTaskCompletionSource _effectCompletionSource;
        
        /// <summary>
        /// ループ再生用のフラグ
        /// </summary>
        private bool _isLooping = false;
        
        #region Life cycle

        /// <summary>
        /// Destroy
        /// </summary>
        private void OnDestroy()
        {
            CleanupAllTweens();
        }

        #endregion

        /// <summary>
        /// めまいのエフェクトを再生する
        /// </summary>
        public void TriggerDizzinessEffect()
        {
            if (_effectSequence != null)
            {
                // 既存の非同期処理があれば停止する
                _effectSequence?.Kill(true);
            }

            // コンポーネントの初期化と初期値の保存を行う
            InitializeComponents();
            CacheInitialValues();

            // ループ再生を行うためのフラグを立てる
            _isLooping = true;

            // エフェクト再生開始
            PlayDizzinessEffectAsync().Forget();
        }

        /// <summary>
        /// エフェクトを停止してからコンポーネントの位置をリセットする
        /// </summary>
        public void StopAndResetEffect()
        {
            // ループ用のフラグをfalseに
            _isLooping = false;

            // DOTweenシェイクを停止
            StopAllCameraShakes();
            ApplyEffectIntensity(0f);

            // カメラ位置をリセット
            if (_targetTransform != null)
            {
                _targetTransform.position = _originalCameraPosition;
                _targetTransform.eulerAngles = _originalCameraRotation;
            }
        }

        #region Private Methods

        #region Initialize

        /// <summary>
        /// コンポーネントの初期化
        /// </summary>
        private void InitializeComponents()
        {
            // VolumeProfileからコンポーネントを取得
            if (_postProcessVolume != null && _postProcessVolume.profile != null)
            {
                _postProcessVolume.profile.TryGet(out _vignette);
                _postProcessVolume.profile.TryGet(out _chromaticAberration);
                _postProcessVolume.profile.TryGet(out _lensDistortion);
                _postProcessVolume.profile.TryGet(out _colorAdjustments);
                _postProcessVolume.profile.TryGet(out _depthOfField);
                _postProcessVolume.profile.TryGet(out _motionBlur);
            }

            if (_targetTransform != null)
            {
                _originalCameraPosition = _targetTransform.position;
                _originalCameraRotation = _targetTransform.eulerAngles;
            }
        }

        /// <summary>
        /// VolumeProfileの初期値を保存
        /// </summary>
        private void CacheInitialValues()
        {
            _originalVignetteIntensity = _vignette?.intensity.value ?? 0f;
            _originalChromaticIntensity = _chromaticAberration?.intensity.value ?? 0f;
            _originalLensDistortion = _lensDistortion?.intensity.value ?? 0f;
            _originalSaturation = _colorAdjustments?.saturation.value ?? 0f;
            _originalExposure = _colorAdjustments?.postExposure.value ?? 0f;
            _originalDofDistance = _depthOfField?.focusDistance.value ?? 10f;
            _originalDofAperture = _depthOfField?.aperture.value ?? 5.6f;
        }

        #endregion

        /// <summary>
        /// めまいエフェクトのsequenceを作成する
        /// </summary>
        private async UniTask PlayDizzinessEffectAsync()
        {
            // 初期値からアニメーションカーブの最初の値まで自然に繋がるようにアニメーションする
            await DOTween.To(() => 0f, ApplyEffectIntensity, _intensityCurve.Evaluate(0), _effectDuration * 0.1f).ToUniTask();
            
            // ループが有効な間は繰り返し実行
            while (_isLooping)
            {
                _effectCompletionSource = new UniTaskCompletionSource();

                try
                {
                    var sequence = DOTween.Sequence();

                    // VolumeProfileの値変更を行う
                    sequence.Append(
                        DOTween.To(() => 0f, ApplyEffectIntensity, 1f, _effectDuration).SetEase(_intensityCurve));

                    sequence.OnComplete(() =>
                    {
                        // アニメーションカーブの終了時の値をセットしなおす
                        ApplyEffectIntensity(_intensityCurve.Evaluate(1));
                        _effectSequence = null;

                        // UniTaskCompletionSourceを完了させる
                        _effectCompletionSource?.TrySetResult();
                    });

                    _effectSequence = sequence;

                    // シーケンスの完了を待機
                    await _effectCompletionSource.Task;
                }
                catch (OperationCanceledException)
                {
                    // キャンセルされた場合は正常終了
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"めまいエフェクトの再生中にエラーが発生しました: {ex.Message}");
                    _effectCompletionSource?.TrySetException(ex);
                    break;
                }
            }
        }

        /// <summary>
        /// エフェクト強度を適用する
        /// </summary>
        private void ApplyEffectIntensity(float intensity)
        {
            // ポストプロセス
            ApplyPostProcessEffects(intensity);

            // カメラ
            ApplyCameraEffects(intensity);
        }

        #region PostProcessEffects

        /// <summary>
        /// ポストプロセスのエフェクトを適用する
        /// </summary>
        private void ApplyPostProcessEffects(float intensity)
        {
            // Vignette効果
            if (_vignette != null)
            {
                _vignette.intensity.value = Mathf.Lerp(_originalVignetteIntensity, _maxVignette, intensity);
            }

            // Chromatic Aberration効果
            if (_chromaticAberration != null)
            {
                _chromaticAberration.intensity.value =
                    Mathf.Lerp(_originalChromaticIntensity, _maxChromaticAberration, intensity);
            }

            // Lens Distortion効果
            if (_lensDistortion != null)
            {
                _lensDistortion.intensity.value = Mathf.Lerp(_originalLensDistortion, _maxLensDistortion, intensity);
            }

            // Color Adjustments効果
            if (_colorAdjustments != null)
            {
                _colorAdjustments.saturation.value =
                    Mathf.Lerp(_originalSaturation, _maxColorAdjustmentIntensity, intensity);
                _colorAdjustments.postExposure.value = Mathf.Lerp(_originalExposure, -0.2f, intensity);

                Color currentTint = Color.Lerp(Color.white, _tintColor, intensity);
                _colorAdjustments.colorFilter.value = currentTint;
            }

            // Motion Blur効果（軽微に）
            if (_motionBlur != null)
            {
                _motionBlur.intensity.value = Mathf.Lerp(0f, _maxMotionBlurIntensity, intensity);
            }

            // Depth of Field効果（ぼかし）
            if (_depthOfField != null)
            {
                _depthOfField.focusDistance.value =
                    Mathf.Lerp(_originalDofDistance, _maxDepthOfFieldDistance, intensity);
                _depthOfField.aperture.value = Mathf.Lerp(_originalDofAperture, _maxDepthOfFieldAperture, intensity);
            }
        }

        #endregion

        #region CameraEffects

        /// <summary>
        /// カメラエフェクトを適用する
        /// </summary>
        private void ApplyCameraEffects(float intensity)
        {
            // カメラシェイク効果
            ApplyContinuousShake(intensity);

            // カメラの傾き効果
            ApplyCameraTilt(intensity);
        }

        /// <summary>
        /// 継続的なシェイク効果
        /// </summary>
        private void ApplyContinuousShake(float intensity)
        {
            if (_targetTransform == null || intensity <= 0f)
            {
                _currentShakeTween?.Kill();
                return;
            }

            // 現在のシェイクを停止
            _currentShakeTween?.Kill();

            // 新しいシェイクを開始
            float shakeStrength = _maxShakeStrength * intensity;
            _currentShakeTween = _targetTransform
                .DOShakePosition(0.1f, shakeStrength, _shakeVibrato, _shakeRandomness)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.InOutSine);
        }

        /// <summary>
        /// カメラの傾き効果
        /// </summary>
        private void ApplyCameraTilt(float intensity)
        {
            if (_targetTransform == null) return;

            _currentTiltTween?.Kill();

            if (intensity > 0f)
            {
                float tiltAngle = _maxCameraTilt * intensity;
                Vector3 targetRotation = _originalCameraRotation;
                targetRotation.z = tiltAngle * Mathf.Sin(Time.time * 0.5f);

                _currentTiltTween = _targetTransform
                    .DORotate(targetRotation, 0.1f)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.InOutSine);
            }
            else
            {
                _targetTransform.eulerAngles = _originalCameraRotation;
            }
        }

        #endregion

        /// <summary>
        /// すべてのカメラシェイクを停止
        /// </summary>
        private void StopAllCameraShakes()
        {
            _currentShakeTween?.Kill();
            _currentTiltTween?.Kill();

            if (_targetTransform != null)
            {
                _targetTransform.position = _originalCameraPosition;
                _targetTransform.eulerAngles = _originalCameraRotation;
            }
        }

        /// <summary>
        /// すべてのTweenをクリーンアップ
        /// </summary>
        private void CleanupAllTweens()
        {
            _effectSequence?.Kill();
            StopAllCameraShakes();
            
            // UniTaskCompletionSourceもクリーンアップ
            if (_effectCompletionSource != null)
            {
                _effectCompletionSource?.TrySetCanceled();
            }
        }

        #endregion
    }
}