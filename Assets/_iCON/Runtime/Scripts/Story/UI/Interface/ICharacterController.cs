using CryStar.Story.Enums;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace CryStar.Story.UI
{
    /// <summary>
    /// キャラクターの画像制御を行うUIContentsが継承すべきインターフェース
    /// </summary>
    public interface ICharacterController
    {
        /// <summary>
        /// 全キャラクターの位置とスケールを一括設定
        /// </summary>
        void SetupAllCharacters(float scale, Vector3 positionOffset);
        
        /// <summary>
        /// 登場
        /// </summary>
        UniTask<Tween> Entry(CharacterPositionType position, string fileName, float duration);
        
        /// <summary>
        /// キャラクター画像の差し替え
        /// </summary>
        UniTask<Tween> Change(CharacterPositionType position, string fileName, float duration);
        
        /// <summary>
        /// 退場
        /// </summary>
        Tween Exit(CharacterPositionType position, float duration);
        
        /// <summary>
        /// 全キャラクターを非表示
        /// </summary>
        void HideAllCharacters();
        
        /// <summary>
        /// 全キャラクターの座標とスケールを初期状態にリセット
        /// </summary>
        void ResetAllTransforms();
        
        /// <summary>
        /// 全キャラクターに対してクリーンアップを行う
        /// </summary>
        void CleanupAllCharacters();
        
        /// <summary>
        /// 指定位置のキャラクターが表示中かどうかを取得
        /// </summary>
        bool IsCharacterVisible(CharacterPositionType position);
    }
}