using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<StoryCharacterPositionType, CharacterPositionData> _positionCache;

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
        /// 表示
        /// </summary>
        public void Show(StoryCharacterPositionType position)
        {
            GetCharacterPosition(position).Image.Show();
        }
        
        /// <summary>
        /// 非表示
        /// </summary>
        public void Hide(StoryCharacterPositionType position)
        {
            GetCharacterPosition(position).Image.Hide();
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
        public bool ChangeSprite(StoryCharacterPositionType position, string fillName)
        {
            // データクラスを取得する
            var characterData = GetCharacterPosition(position);
            if (characterData?.Image == null)
            {
                LogUtility.Error($"{position} の画像が設定されていません", LogCategory.UI, this);
                return false;
            }

            // nullではなかったら画像を差し替える
            characterData.Image.AssetName = fillName;
            return true;
        }
        
        /// <summary>
        /// 指定位置のキャラクターが表示中かどうかを取得
        /// </summary>
        public bool IsVisible(StoryCharacterPositionType position)
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
            _positionCache = new Dictionary<StoryCharacterPositionType, CharacterPositionData>();
            _positionCache = _characterPositions
                .Where(position => position != null)
                .ToDictionary(position => position.PositionType, position => position);
        }
        
        /// <summary>
        /// 指定位置のキャラクターデータを取得
        /// </summary>
        private CharacterPositionData GetCharacterPosition(StoryCharacterPositionType position)
        {
            return _positionCache.GetValueOrDefault(position);
        }
        
        #endregion
    }
}