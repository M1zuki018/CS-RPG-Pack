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
            
            // 初期化時は全て非表示にしておく
            HideAll();
        }

        #endregion

        /// <summary>
        /// 登場
        /// </summary>
        public Tween Entry(CharacterPositionType position, string fileName, float duration)
        {
            // 指定された立ち位置の配置データを取得する
            var positionData = GetCharacterPosition(position);
            if (positionData == null)
            {
                return null;
            }
            
            // アクティブなImageに画像を設定
            var activeImage = positionData.GetActiveImage();
            activeImage.AssetName = fileName;
            
            return activeImage.DOFade(1, duration);
        }
        
        /// <summary>
        /// キャラクター画像の差し替え
        /// </summary>
        public Tween Change(CharacterPositionType position, string fileName, float duration)
        {
            // 指定された立ち位置の配置データを取得する
            var positionData = GetCharacterPosition(position);
            if (positionData == null)
            {
                return null;
            }
            
            if (!IsVisible(position))
            {
                // まだ画像が表示されていなければ通常の登場処理Tweenを実行する
                return Entry(position, fileName, duration);
            }

            // クロスフェード処理
            var currentImage = positionData.GetActiveImage();
            var nextImage = positionData.GetInactiveImage();
            
            // 次のImageに新しい画像を設定
            nextImage.AssetName = fileName;
            
            var sequence = DOTween.Sequence();
            
            // 現在のImageをフェードアウト、次のImageをフェードイン
            sequence.Join(currentImage.DOFade(0, duration));
            sequence.Join(nextImage.DOFade(1, duration));
            
            // 完了時にアクティブImageを切り替え、古いImageを非表示にする
            sequence.OnComplete(() =>
            {
                currentImage.Hide();
                positionData.SwitchActiveImage();
            });

            return sequence;
        }
        
        /// <summary>
        /// 退場
        /// </summary>
        public Tween Exit(CharacterPositionType position, float duration)
        {
            // 指定された立ち位置の配置データを取得する
            var positionData = GetCharacterPosition(position);
            if (positionData == null)
            {
                return null;
            }
            
            // 両方のImageを同時にフェードアウト
            var sequence = DOTween.Sequence()
                .Append(positionData.Image1.DOFade(0, duration))
                .Join(positionData.Image2.DOFade(0, duration));

            // フェードアウト完了後に両方を非表示にする
            sequence.OnComplete(() =>
            {
                positionData.Image1?.Hide();
                positionData.Image2?.Hide();
                positionData.ResetActiveImageIndex();
            });

            return sequence;
        }

        /// <summary>
        /// 全キャラクターを非表示
        /// </summary>
        public void HideAll()
        {
            foreach (var positionData in _characterPositions)
            {
                positionData.Image1?.Hide();
                positionData.Image2?.Hide();
                positionData.ResetActiveImageIndex();
            }
        }
        
        /// <summary>
        /// 指定位置のキャラクターが表示中かどうかを取得
        /// </summary>
        public bool IsVisible(CharacterPositionType position)
        {
            var characterData = GetCharacterPosition(position);
            return characterData?.Image1?.gameObject.activeInHierarchy ?? false;
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