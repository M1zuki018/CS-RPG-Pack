using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace iCON.UI
{
    /// <summary>
    /// タイトル画面でマウスの動きに合わせて傾いて見えるような演出
    /// </summary>
    public class ImageParallax : ViewBase
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

        /// <summary>
        /// 初期位置を保存するためのリスト
        /// </summary>
        private List<Vector2[]> _initialPositionList = new List<Vector2[]>();

        /// <summary>
        /// 画面の中央
        /// </summary>
        private Vector2 _screenCenter;

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
                // float depthMultiplier = (i + 1) * 0.5f;
                float depthMultiplier = 1;

                // 移動量を計算
                Vector2 movement = new Vector2(
                    mouseDelta.x * settings.Factor * depthMultiplier * 100f,
                    mouseDelta.y * settings.Factor * depthMultiplier * 100f
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

        #endregion
    }
}