using iCON.Enums;
using iCON.UI;
using iCON.Utility;
using UnityEngine;

namespace iCON.Field.Player
{
    /// <summary>
    /// プレイヤーの動きを管理するクラス
    /// </summary>
    public class PlayerMover : MonoBehaviour
    {
        /// <summary>
        /// スプライトアニメーションのコントローラー
        /// </summary>
        [SerializeField]
        private SpriteAnimationController _animController;

        /// <summary>
        /// 移動速度
        /// </summary>
        [SerializeField] 
        private float _moveSpeed = 5f;

        /// <summary>
        /// 現在向いている方向
        /// </summary>
        private MoveDirectionType _directionType;
        
        private void Update()
        {
            if (UnityEngine.Input.GetKey(KeyCode.A))
            {
                _directionType = MoveDirectionType.Left;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.D))
            {
                _directionType = MoveDirectionType.Right;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.W))
            {
                _directionType = MoveDirectionType.Up;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.S))
            {
                _directionType = MoveDirectionType.Down;
            }
            else
            {
                _directionType = MoveDirectionType.None;
            }
            
            var direction = DirectionUtility.GetVector2(_directionType);
            // Time.deltaTimeと移動速度を掛けて適切な移動量にする
            Vector3 movement = new Vector3(direction.x, direction.y, 0) * _moveSpeed * Time.deltaTime;
            transform.position += movement;
        }
    }
}
