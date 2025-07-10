using iCON.Enums;
using iCON.UI;
using UnityEngine;

namespace iCON.Field.Player
{
    /// <summary>
    /// プレイヤーの動きを管理するクラス
    /// </summary>
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField]
        private SpriteAnimationController _animController;

        /// <summary>
        /// 現在向いている方向
        /// </summary>
        private MoveDirectionType _directionType;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _directionType = MoveDirectionType.Left;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                _directionType = MoveDirectionType.Right;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                _directionType = MoveDirectionType.Up;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _directionType = MoveDirectionType.Down;
            }
        }
    }
}
