using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.Utility;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// UIContents ストーリーのキャラクター画像管理
    /// </summary>
    public class UIContents_StoryCharacters : MonoBehaviour
    {
        /// <summary>
        /// 各立ち位置のキャラクター表示データ配列
        /// </summary>
        [SerializeField]
        private CharacterPositionData[] _characterPositions;
        
        /// <summary>
        /// キャッシュ用Dictionary
        /// </summary>
        private Dictionary<CharacterPositionType, CharacterPositionData> _positionCache;

        #region Lifecycle

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            InitializePositionCache();
        }

        #endregion

        /// <summary>
        /// 登場
        /// </summary>
        public Tween Entry(CharacterPositionType position, string fileName, float duration)
        {
            ChangeSprite(position,fileName);
            return GetCharacterPosition(position).Image.DOFade(1, duration);
        }
        
        /// <summary>
        /// 退場
        /// </summary>
        public Tween Exit(CharacterPositionType position, float duration)
        {
            return GetCharacterPosition(position).Image.DOFade(0, duration);
        }

        /// <summary>
        /// 全キャラクターを非表示
        /// </summary>
        public void HideAll()
        {
            foreach (var positionData in _characterPositions)
            {
                positionData.Image?.Hide();
            }
        }
        
        /// <summary>
        /// キャラクター画像の差し替え
        /// </summary>
        public bool ChangeSprite(CharacterPositionType position, string fillName)
        {
            // データクラスを取得する
            // var characterData = GetCharacterPosition(position);
            // if (characterData?.Image == null)
            // {
            //     LogUtility.Error($"{position} の画像が設定されていません", LogCategory.UI, this);
            //     return false;
            // }

            // nullではなかったら画像を差し替える
            _positionCache[position].Image.AssetName = fillName;
            return true;
        }
        
        /// <summary>
        /// 指定位置のキャラクターが表示中かどうかを取得
        /// </summary>
        public bool IsVisible(CharacterPositionType position)
        {
            var characterData = GetCharacterPosition(position);
            return characterData?.Image?.gameObject.activeInHierarchy ?? false;
        }
        
        #region Private Methods

        /// <summary>
        /// 位置キャッシュの初期化
        /// </summary>
        private void InitializePositionCache()
        {
            if (_characterPositions == null) return;

            // NOTE: 検索でパフォーマンスが落ちないようにDictionaryを使う
            _positionCache = new Dictionary<CharacterPositionType, CharacterPositionData>();
            _positionCache = _characterPositions
                .Where(position => position != null)
                .ToDictionary(position => position.PositionType, position => position);
        }
        
        /// <summary>
        /// 指定位置のキャラクターデータを取得
        /// </summary>
        private CharacterPositionData GetCharacterPosition(CharacterPositionType position)
        {
            return _positionCache.GetValueOrDefault(position);
        }
        
        #endregion
    }
}