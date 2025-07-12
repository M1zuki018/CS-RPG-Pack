using System.Collections.Generic;
using iCON.Enums;
using UnityEngine;

namespace iCON.Field.Player
{
    /// <summary>
    /// プレイヤーの歩行アニメーションのSpriteを設定するスクリプタブルオブジェクト
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerAnimationSetting", menuName = "Scriptable Objects/Player Animation Setting")]
    public class PlayerAnimationSetting : ScriptableObject
    {
        [SerializeField] private List<Sprite> _leftSprites = new List<Sprite>();
        [SerializeField] private List<Sprite> _rightSprites = new List<Sprite>();
        [SerializeField] private List<Sprite> _upSprites = new List<Sprite>();
        [SerializeField] private List<Sprite> _downSprites = new List<Sprite>();

        /// <summary>
        /// 引数で指定した方向に対応した画像リストを取得する
        /// </summary>
        public List<Sprite> GetSprites(MoveDirectionType direction)
        {
            var sprites = direction switch
            {
                MoveDirectionType.Left => _leftSprites,
                MoveDirectionType.Right => _rightSprites,
                MoveDirectionType.Up => _upSprites,
                MoveDirectionType.Down => _downSprites,
                _ => null,
            };
            
            return sprites;
        }
    }

}
