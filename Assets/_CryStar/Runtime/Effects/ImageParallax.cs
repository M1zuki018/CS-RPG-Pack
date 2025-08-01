using System.Collections.Generic;
using System.Linq;
using CryStar.Attribute;
using CryStar.Core;
using UnityEngine;

namespace CryStar.Effects
{
    /// <summary>
    /// タイトル画面でマウスの動きに合わせて傾いて見えるような演出
    /// 長押しでズーム効果も追加（Lerpによる実装）
    /// </summary>
    public class ImageParallax : CustomBehaviour
    {
        [Header("パララックス設定")] 
        [SerializeField] 
        private ParallaxLayer[] _parallaxSettings = new ParallaxLayer[]
        {
            new ParallaxLayer { Factor = 0.02f, Name = "弱" },
            new ParallaxLayer { Factor = 0.03f, Name = "中" },
            new ParallaxLayer { Factor = 0.05f, Name = "強" },
        };

        [SerializeField, Comment("移動のスムーズさ")] 
        private float _smoothTime = 0.2f;

        [Header("ズーム設定")]
        [SerializeField, Comment("ズーム倍率")]
        private float _zoomScale = 1.1f;
        
        [SerializeField, Comment("ズームアニメーション時間")]
        private float _zoomDuration = 0.5f;
        
        [SerializeField, Comment("長押し判定時間（秒）")]
        private float _longPressThreshold = 0.3f;
        
        [SerializeField, Comment("ズーム時のパララックス強度倍率")]
        private float _zoomParallaxMultiplier = 1.5f;

        /// <summary>
        /// 初期位置を保存するためのリスト
        /// </summary>
        private List<Vector2[]> _initialPositionList = new List<Vector2[]>();

        /// <summary>
        /// 各オブジェクトの初期スケールを保存
        /// </summary>
        private List<Vector3[]> _initialScaleList = new List<Vector3[]>();

        /// <summary>
        /// 画面の中央
        /// </summary>
        private Vector2 _screenCenter;

        /// <summary>
        /// 長押し中
        /// </summary>
        private bool _isPressed = false;
        
        /// <summary>
        /// 長押しを始めた時間
        /// </summary>
        private float _pressStartTime = 0f;
        
        /// <summary>
        /// ズーム中
        /// </summary>
        private bool _isZoomed = false;

        /// <summary>
        /// ズームアニメーション中
        /// </summary>
        private bool _isZoomAnimating = false;

        /// <summary>
        /// ズームアニメーション開始時間
        /// </summary>
        private float _zoomAnimationStartTime = 0f;

        /// <summary>
        /// ズームイン中かズームアウト中かのフラグ
        /// </summary>
        private bool _isZoomingIn = false;

        /// <summary>
        /// 現在のパララックス強度倍率
        /// </summary>
        private float _currentParallaxMultiplier = 1f;

        /// <summary>
        /// パララックス強度倍率のアニメーション用
        /// </summary>
        private float _targetParallaxMultiplier = 1f;
        private float _startParallaxMultiplier = 1f;

        #region Life cycle

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            HandleInput();
            UpdateZoomAnimation();
            
            // マウスの相対位置を計算
            var mouseDelta = CalculateMouseDelta();
            
            // パララックス効果を適用
            ApplyParallaxEffect(mouseDelta);
        }

        /// <summary>
        /// Validate
        /// </summary>
        private void OnValidate()
        {
            // エディタでの変更時に再初期化を行う（実行時のみ）
            if (Application.isPlaying)
            {
                Initialize();
            }
        }

        #endregion

        /// <summary>
        /// Active状態を切り替える
        /// </summary>
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        #region Private Method

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            // スクリーンの中央のポジションの取得と、オブジェクトの初期位置のキャッシュを取得する
            _screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            _initialPositionList = CreatePositionList();
            _initialScaleList = CreateScaleList();
        }
        
        /// <summary>
        /// 初期化時に各オブジェクトの初期位置をまとめたリストを作成する
        /// </summary>
        private List<Vector2[]> CreatePositionList()
        {
            // ParallaxSettingsの数に合わせて先にリストを作成しておく
            var initialPositionList = new List<Vector2[]>(_parallaxSettings.Length);

            foreach (var layer in _parallaxSettings)
            {
                // それぞれのオブジェクトの初期位置をリストに追加
                initialPositionList.Add(layer.Objects.Select(rectTransform => rectTransform.anchoredPosition).ToArray());
            }
            
            return initialPositionList;
        }

        /// <summary>
        /// 初期化時に各オブジェクトの初期スケールをまとめたリストを作成する
        /// </summary>
        private List<Vector3[]> CreateScaleList()
        {
            var initialScaleList = new List<Vector3[]>(_parallaxSettings.Length);

            foreach (var layer in _parallaxSettings)
            {
                // それぞれのオブジェクトの初期スケールをリストに追加
                initialScaleList.Add(layer.Objects.Select(rectTransform => rectTransform.localScale).ToArray());
            }
            
            return initialScaleList;
        }
        
        /// <summary>
        /// マウスの現在位置から相対的な位置を計算 (-1 ~ 1の範囲)
        /// </summary>
        private Vector2 CalculateMouseDelta()
        {
            Vector2 mousePos = UnityEngine.Input.mousePosition;
            return new Vector2(
                (mousePos.x - _screenCenter.x) / _screenCenter.x,
                (mousePos.y - _screenCenter.y) / _screenCenter.y
            );
        }
        
        /// <summary>
        /// パララックス効果を全レイヤーに適用
        /// </summary>
        private void ApplyParallaxEffect(Vector2 mouseDelta)
        {
            for (int i = 0; i < _initialPositionList.Count; i++)
            {
                // 各レイヤータイプに対応する設定を取得して適用
                ApplyParallaxToLayers(_parallaxSettings[i], _initialPositionList[i], mouseDelta);
            }
        }
        
        /// <summary>
        /// 指定レイヤーにパララックス効果を適用
        /// </summary>
        private void ApplyParallaxToLayers(ParallaxLayer settings, Vector2[] initialPositions, Vector2 mouseDelta)
        {
            if (settings == null || settings.Objects == null)
            {
                return;
            }

            var validObjects = settings.Objects.Where(obj => obj != null).ToArray();
            int minCount = Mathf.Min(validObjects.Length, initialPositions.Length);

            for (int i = 0; i < minCount; i++)
            {
                var rectTransform = validObjects[i];
                if (rectTransform == null) continue;

                // 深度係数（配列のインデックスによって動きの強さを変える）
                float depthMultiplier = 1;

                // 移動量を計算（現在のパララックス強度倍率を適用）
                Vector2 movement = new Vector2(
                    mouseDelta.x * settings.Factor * depthMultiplier * 100f * _currentParallaxMultiplier,
                    mouseDelta.y * settings.Factor * depthMultiplier * 100f * _currentParallaxMultiplier
                );

                // 逆方向設定の場合は符号を反転
                if (settings.Inverse)
                {
                    movement = -movement;
                }

                Vector2 targetPos = initialPositions[i] + movement;
                
                // スムーズに移動
                Vector2 currentPos = rectTransform.anchoredPosition;
                Vector2 newPos = Vector2.Lerp(currentPos, targetPos, _smoothTime * Time.deltaTime);
                rectTransform.anchoredPosition = newPos;
            }
        }

        /// <summary>
        /// 入力処理（マウス・タッチ対応）
        /// </summary>
        private void HandleInput()
        {
            bool currentPressed = UnityEngine.Input.GetMouseButton(0) || (UnityEngine.Input.touchCount > 0);

            // 押し始め
            if (currentPressed && !_isPressed)
            {
                // 押し始めの時間を記録
                _isPressed = true;
                _pressStartTime = Time.time;
            }
            // 押している間
            else if (currentPressed && _isPressed)
            {
                float pressDuration = Time.time - _pressStartTime;
                
                // 長押し判定でズーム開始
                if (pressDuration >= _longPressThreshold && !_isZoomed)
                {
                    StartZoom();
                }
            }
            // 離した時
            else if (!currentPressed && _isPressed)
            {
                _isPressed = false;
                
                if (_isZoomed)
                {
                    EndZoom();
                }
            }
        }

        /// <summary>
        /// ズームアニメーション更新処理
        /// </summary>
        private void UpdateZoomAnimation()
        {
            if (!_isZoomAnimating) return;

            float elapsed = Time.time - _zoomAnimationStartTime;
            float duration = _isZoomingIn ? _zoomDuration : _zoomDuration * 0.7f;
            float progress = Mathf.Clamp01(elapsed / duration);

            // Ease.OutQuartの近似（より滑らかな動き）
            float easedProgress = 1f - Mathf.Pow(1f - progress, 4f);

            // スケールアニメーション
            UpdateScaleAnimation(easedProgress);

            // パララックス強度アニメーション
            _currentParallaxMultiplier = Mathf.Lerp(_startParallaxMultiplier, _targetParallaxMultiplier, easedProgress);

            // アニメーション完了チェック
            if (progress >= 1f)
            {
                _isZoomAnimating = false;
            }
        }

        /// <summary>
        /// スケールアニメーション更新
        /// </summary>
        private void UpdateScaleAnimation(float progress)
        {
            for (int layerIndex = 0; layerIndex < _parallaxSettings.Length; layerIndex++)
            {
                var layer = _parallaxSettings[layerIndex];
                if (layer.Objects == null || layerIndex >= _initialScaleList.Count) continue;

                var initialScales = _initialScaleList[layerIndex];
                var validObjects = layer.Objects.Where(obj => obj != null).ToArray();
                int minCount = Mathf.Min(validObjects.Length, initialScales.Length);

                for (int i = 0; i < minCount; i++)
                {
                    var rectTransform = validObjects[i];
                    if (rectTransform == null) continue;

                    Vector3 targetScale = _isZoomingIn ? 
                        initialScales[i] * _zoomScale : 
                        initialScales[i];

                    Vector3 startScale = _isZoomingIn ? 
                        initialScales[i] : 
                        initialScales[i] * _zoomScale;

                    rectTransform.localScale = Vector3.Lerp(startScale, targetScale, progress);
                }
            }
        }

        /// <summary>
        /// ズーム開始
        /// </summary>
        private void StartZoom()
        {
            _isZoomed = true;
            _isZoomAnimating = true;
            _isZoomingIn = true;
            _zoomAnimationStartTime = Time.time;

            // パララックス強度倍率の設定
            _startParallaxMultiplier = _currentParallaxMultiplier;
            _targetParallaxMultiplier = _zoomParallaxMultiplier;
        }

        /// <summary>
        /// ズーム終了
        /// </summary>
        private void EndZoom()
        {
            _isZoomed = false;
            _isZoomAnimating = true;
            _isZoomingIn = false;
            _zoomAnimationStartTime = Time.time;

            // パララックス強度倍率の設定
            _startParallaxMultiplier = _currentParallaxMultiplier;
            _targetParallaxMultiplier = 1f;
        }

        #endregion
    }
}