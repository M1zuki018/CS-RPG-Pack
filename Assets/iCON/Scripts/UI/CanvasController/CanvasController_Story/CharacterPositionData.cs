using System;
using UnityEngine;

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
        [SerializeField]
        private StoryCharacterPositionType _positionType;

        /// <summary>
        /// CustomImage
        /// </summary>
        [SerializeField] 
        private CustomImage _image;

        /// <summary>
        /// キャラクターの立ち位置を取得
        /// </summary>
        public StoryCharacterPositionType PositionType => _positionType;
        
        /// <summary>
        /// 表示用画像コンポーネントを取得
        /// </summary>
        public CustomImage Image => _image;
        
        /// <summary>
        /// アセットが既に設定されているかを確認
        /// </summary>
        public bool HasAsset => Image != null && !string.IsNullOrEmpty(Image.AssetName);
    }
}