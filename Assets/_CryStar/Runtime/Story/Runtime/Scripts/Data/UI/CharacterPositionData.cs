using System;
using CryStar.Story.Enums;
using CryStar.Utility;
using CryStar.Utility.Enum;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CryStar.Story.Data
{
    /// <summary>
    /// ストーリー演出用のキャラクター配置データ
    /// キャラクターの立ち位置とUI表示用画像の組み合わせを管理する
    /// </summary>
    [Serializable]
    public class CharacterPositionData
    {
        /// <summary>
        /// 立ち位置
        /// </summary>
        [Header("Position Settings") , SerializeField]
        private CharacterPositionType _positionType;

        /// <summary>
        /// メインとなるCustomImage
        /// </summary>
        [Header("Image Objects"), SerializeField] 
        private CustomImage _primaryImage;

        #region Private Fields

        /// <summary>
        /// プライマリイメージのコピー。フェードでのキャラクター入れ替え用
        /// </summary>
        private CustomImage _secondaryImage;
        
        /// <summary>
        /// 現在アクティブなImageのインデックス（0: PrimaryImage, 1: SecondaryImage）
        /// </summary>
        private int _activeImageIndex = 0;
        
        /// <summary>
        /// 初期拡大率
        /// </summary>
        private float _initialScale = 1;
        
        /// <summary>
        /// 初期座標
        /// </summary>
        private Vector3 _initialPosition;
        
        /// <summary>
        /// 初期化が完了しているか
        /// </summary>
        private bool _isInitialized = false;

        #endregion

        #region Public Properties
        
        /// <summary>
        /// キャラクターの立ち位置を取得
        /// </summary>
        public CharacterPositionType PositionType => _positionType;
        
        /// <summary>
        /// プライマリImageオブジェクト
        /// </summary>
        public CustomImage PrimaryImage => _primaryImage;
        
        /// <summary>
        /// セカンダリImageオブジェクト
        /// </summary>
        public CustomImage SecondaryImage => _secondaryImage;
        
        /// <summary>
        /// 初期拡大率
        /// </summary>
        public float InitialScale => _initialScale;
        
        /// <summary>
        /// 初期座標
        /// </summary>
        public Vector3 InitialPosition => _initialPosition;
        
        /// <summary>
        /// アセットが既に設定されているかを確認
        /// </summary>
        public bool HasAsset => PrimaryImage != null && PrimaryImage.HasAsset;

        /// <summary>
        /// 初期化が完了しているか
        /// </summary>
        public bool IsInitialized => _isInitialized;
        
        #endregion
        
        /// <summary>
        /// 初期設定をキャッシュし、セカンダリイメージを生成する
        /// </summary>
        /// <param name="siblingIndex">セカンダリイメージの挿入位置</param>
        public void Initialize(int siblingIndex = -1)
        {
            if (_primaryImage == null)
            {
                LogUtility.Warning("プライマリImageがアサインされていません", LogCategory.UI);
                return;
            }

            CacheInitialTransform();
            CreateSecondaryImage(siblingIndex);

            // 初期化完了
            _isInitialized = true;
        }

        /// <summary>
        /// 両方のImageのTransformを初期状態にリセット
        /// </summary>
        public void ResetTransform()
        {
            if (!_isInitialized)
            {
                LogUtility.Warning($"CharacterPositionData{_positionType} の 初期化が終了していません", LogCategory.UI);
                return;
            }
            
            // Transformのリセット
            ResetImageTransform(_primaryImage);
            ResetImageTransform(_secondaryImage);
        }

        /// <summary>
        /// 現在アクティブなImageを取得
        /// </summary>
        public CustomImage GetActiveImage()
        {
            return _activeImageIndex == 0 ? _primaryImage : _secondaryImage;
        }
        
        /// <summary>
        /// 現在非アクティブなImageを取得
        /// </summary>
        public CustomImage GetInactiveImage()
        {
            return _activeImageIndex == 0 ? _secondaryImage : _primaryImage;
        }
        
        /// <summary>
        /// アクティブImageを切り替える
        /// </summary>
        public void SwitchActiveImage()
        {
            _activeImageIndex = _activeImageIndex == 0 ? 1 : 0;
        }
        
        /// <summary>
        /// アクティブImageインデックスをリセット
        /// </summary>
        public void ResetActiveImageIndex()
        {
            _activeImageIndex = 0;
        }
        
        /// <summary>
        /// リソースをクリーンアップ
        /// </summary>
        public void Cleanup()
        {
            if (_secondaryImage != null)
            {
                Object.DestroyImmediate(_secondaryImage.gameObject);
                _secondaryImage = null;
            }
            
            _isInitialized = false;
        }

        #region Private Methods

        /// <summary>
        /// 初期のスケールと座標をキャッシュする
        /// </summary>
        private void CacheInitialTransform()
        {
            if (_primaryImage == null)
            {
                // プライマリイメージが設定されていなかったら以降の処理は行い
                return;
            }
            
            _initialScale = _primaryImage.transform.localScale.x;
            _initialPosition = _primaryImage.transform.localPosition;
        }

        /// <summary>
        /// セカンダリイメージを生成する
        /// </summary>
        private void CreateSecondaryImage(int siblingIndex)
        {
            if (_primaryImage == null && _secondaryImage != null)
            {
                return;
            }
            
            // セカンダリイメージのオブジェクトを生成
            var secondaryObject = Object.Instantiate(_primaryImage.gameObject, _primaryImage.transform.parent);
            
            if (secondaryObject.TryGetComponent(out _secondaryImage))
            {
                if (siblingIndex >= 0)
                {
                    // 挿入位置が設定されている場合はその位置に挿入
                    secondaryObject.transform.SetSiblingIndex(siblingIndex);
                }
                
                // セカンダリイメージは初期状態では非表示
                _secondaryImage.Hide();
            }
            else
            {
                LogUtility.Error($"{_positionType} セカンダリイメージの生成に失敗しました", LogCategory.UI);
                
                // オブジェクトを削除
                Object.DestroyImmediate(secondaryObject);
            }
            
        }

        /// <summary>
        /// 指定されたImageのTransformを初期状態にリセット
        /// </summary>
        private void ResetImageTransform(CustomImage image)
        {
            if (image == null)
            {
                return;
            }
            
            image.transform.localPosition = _initialPosition;
            image.transform.localScale = Vector3.one * _initialScale;
        }

        #endregion
    }
}