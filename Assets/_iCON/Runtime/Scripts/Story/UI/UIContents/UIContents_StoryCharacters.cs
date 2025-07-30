using System.Collections.Generic;
using System.Linq;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using CryStar.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace CryStar.Story.UI
{
    /// <summary>
    /// UIContents ストーリーのキャラクター画像管理
    /// </summary>
    public class UIContents_StoryCharacters : UIContentsBase, ICharacterController
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
        
        /// <summary>
        /// 全キャラクターの位置とスケールを一括設定
        /// </summary>
        public void SetupAllCharacters(float scale, Vector3 positionOffset)
        {
            foreach (var positionData in _characterPositions)
            {
                SetupCharacterTransform(positionData, scale, positionOffset);
            }
        }

        /// <summary>
        /// 登場
        /// </summary>
        public async UniTask<Tween> Entry(CharacterPositionType position, string fileName, float duration)
        {
            // 指定された立ち位置の配置データを取得する
            var positionData = GetCharacterPosition(position);
            if (positionData == null)
            {
                return null;
            }
            
            // アクティブなImageに画像を設定
            var activeImage = positionData.GetActiveImage();
            
            await activeImage.ChangeSpriteAsync(fileName);
            
            return activeImage.DOFade(1, duration);
        }
        
        /// <summary>
        /// キャラクター画像の差し替え
        /// </summary>
        public async UniTask<Tween> Change(CharacterPositionType position, string fileName, float duration)
        {
            // 指定された立ち位置の配置データを取得する
            var positionData = GetCharacterPosition(position);
            if (positionData == null)
            {
                return null;
            }
            
            if (!IsCharacterVisible(position))
            {
                // まだ画像が表示されていなければ通常の登場処理Tweenを実行する
                return await Entry(position, fileName, duration);
            }

            // クロスフェード処理
            var currentImage = positionData.GetActiveImage();
            var nextImage = positionData.GetInactiveImage();
            
            // 次のImageに新しい画像を設定
            await nextImage.ChangeSpriteAsync(fileName);
            
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
            
            // 念のため両方のImageを同時にフェードアウト
            var sequence = DOTween.Sequence()
                .Append(positionData.PrimaryImage.DOFade(0, duration))
                .Join(positionData.SecondaryImage.DOFade(0, duration));

            // フェードアウト完了後に両方を非表示にする
            sequence.OnComplete(() =>
            {
                positionData.PrimaryImage?.Hide();
                positionData.SecondaryImage?.Hide();
                positionData.ResetActiveImageIndex();
            });

            return sequence;
        }

        /// <summary>
        /// 全キャラクターを非表示
        /// </summary>
        public void HideAllCharacters()
        {
            foreach (var positionData in _characterPositions)
            {
                positionData.PrimaryImage?.Hide();
                positionData.SecondaryImage?.Hide();
                positionData.ResetActiveImageIndex();
            }
        }

        /// <summary>
        /// 全キャラクターの座標とスケールを初期状態にリセット
        /// </summary>
        public void ResetAllTransforms()
        {
            foreach (var positionData in _characterPositions)
            {
                positionData.ResetTransform();
            }
        }

        /// <summary>
        /// 全キャラクターに対してクリーンアップを行う
        /// </summary>
        public void CleanupAllCharacters()
        {
            foreach (var positionData in _characterPositions)
            {
                positionData.Cleanup();
            }
        }
        
        /// <summary>
        /// 指定位置のキャラクターが表示中かどうかを取得
        /// </summary>
        public bool IsCharacterVisible(CharacterPositionType position)
        {
            var characterData = GetCharacterPosition(position);
            return characterData?.PrimaryImage?.gameObject.activeInHierarchy ?? false;
        }
        
        #region Private Methods
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Initialize()
        {
            InitializePositionCache();
            InitializeCharacterPositions();
            
            // 初期化時は全て非表示にしておく
            HideAllCharacters();
        }

        /// <summary>
        /// 位置キャッシュの初期化
        /// </summary>
        private void InitializePositionCache()
        {
            if (_characterPositions == null)
            {
                return;
            }

            // NOTE: 検索でパフォーマンスが落ちないようにDictionaryを使う
            _positionCache = _characterPositions
                .Where(position => position != null)
                .ToDictionary(position => position.PositionType, position => position);
        }
        
        /// <summary>
        /// キャラクター位置データの初期化
        /// </summary>
        private void InitializeCharacterPositions()
        {
            for (int i = 0; i < _characterPositions.Length; i++)
            {
                // セカンダリイメージの挿入位置を計算
                // 重なり順の都合でプライマリイメージの1つ次の子にしたい
                var siblingIndex = i * 2 + 1;
                _characterPositions[i].Initialize(siblingIndex);
            }
        }
        
        /// <summary>
        /// 個別キャラクター位置の設定
        /// </summary>
        private void SetupCharacterTransform(CharacterPositionData positionData, float scale, Vector3 positionOffset)
        {
            // スケール設定
            SetupScale(positionData, scale);

            // Transform設定
            SetupTransform(positionData, positionOffset);
        }

        /// <summary>
        /// スケール設定
        /// </summary>
        private void SetupScale(CharacterPositionData positionData, float scale)
        {
            var scaleVector = Vector3.one * scale;
            
            // 各オブジェクトに適用
            positionData.PrimaryImage.transform.localScale = scaleVector;
            positionData.SecondaryImage.transform.localScale = scaleVector;
        }
        
        /// <summary>
        /// Transform設定
        /// </summary>
        private void SetupTransform(CharacterPositionData positionData, Vector3 positionOffset)
        {
            // 位置計算と設定
            var basePosition = positionData.PrimaryImage.transform.localPosition;
            var newPosition = CalculatePositionOffset(positionData.PositionType, basePosition, positionOffset);
            
            // 各オブジェクトに適用
            positionData.PrimaryImage.transform.localPosition = newPosition;
            positionData.SecondaryImage.transform.localPosition = newPosition;
        }
        
        /// <summary>
        /// 位置タイプに応じた座標オフセットを計算
        /// </summary>
        private Vector3 CalculatePositionOffset(CharacterPositionType positionType, Vector3 basePosition, Vector3 offset)
        {
            return positionType switch
            {
                CharacterPositionType.Center => new Vector3(basePosition.x, basePosition.y + offset.y, basePosition.z),
                CharacterPositionType.Near => new Vector3(basePosition.x, basePosition.y + offset.y, basePosition.z),
                CharacterPositionType.Left => new Vector3(basePosition.x - offset.x, basePosition.y + offset.y, basePosition.z),
                CharacterPositionType.Right => new Vector3(basePosition.x + offset.x, basePosition.y + offset.y, basePosition.z),
                _ => basePosition
            };
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