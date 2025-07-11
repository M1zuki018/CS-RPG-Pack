using iCON.Enums;
using UnityEngine;

namespace iCON.Utility
{
    /// <summary>
    /// 方向に関するUtilityクラス
    /// </summary>
    public class DirectionUtility : MonoBehaviour
    {
        /// <summary>
        /// 移動方向からVector2を取得する
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <returns>正規化されたVector2</returns>
        public static Vector2 GetVector2(MoveDirectionType direction)
        {
            return direction switch
            {
                MoveDirectionType.None => Vector2.zero,
                MoveDirectionType.Left => Vector2.left,
                MoveDirectionType.Up => Vector2.up,
                MoveDirectionType.Right => Vector2.right,
                MoveDirectionType.Down => Vector2.down,
                MoveDirectionType.LeftUp => new Vector2(-1, 1).normalized,
                MoveDirectionType.RightUp => new Vector2(1, 1).normalized,
                MoveDirectionType.LeftDown => new Vector2(-1, -1).normalized,
                MoveDirectionType.RightDown => new Vector2(1, -1).normalized,
                _ => Vector2.zero
            };
        }
    }

}
