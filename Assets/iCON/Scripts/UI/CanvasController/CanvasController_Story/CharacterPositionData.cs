using System;
using iCON.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace iCON.UI
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
        /// CustomImage
        /// </summary>
        [Header("Image Objects"), SerializeField] 
        private CustomImage _image1;
        
        /// <summary>
        /// CustomImage
        /// </summary>
        [SerializeField] 
        private CustomImage _image2;
        
        /// <summary>
        /// 現在アクティブなImageのインデックス（0: Image1, 1: Image2）
        /// </summary>
        private int _activeImageIndex = 0;

        /// <summary>
        /// キャラクターの立ち位置を取得
        /// </summary>
        public CharacterPositionType PositionType => _positionType;
        
        /// <summary>
        /// 第1のImageオブジェクト
        /// </summary>
        public CustomImage Image1 => _image1;
        
        /// <summary>
        /// 第2のImageオブジェクト
        /// </summary>
        public CustomImage Image2 => _image2;
        
        /// <summary>
        /// アセットが既に設定されているかを確認
        /// </summary>
        public bool HasAsset => Image1 != null && !string.IsNullOrEmpty(Image1.AssetName);
        
        /// <summary>
        /// 現在アクティブなImageを取得
        /// </summary>
        public CustomImage GetActiveImage()
        {
            return _activeImageIndex == 0 ? _image1 : _image2;
        }
        
        /// <summary>
        /// 現在非アクティブなImageを取得
        /// </summary>
        public CustomImage GetInactiveImage()
        {
            return _activeImageIndex == 0 ? _image2 : _image1;
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
    }
}