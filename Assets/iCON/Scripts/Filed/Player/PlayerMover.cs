using System.Collections.Generic;
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

        [SerializeField] private List<Sprite> _leftSprites = new List<Sprite>();
        [SerializeField] private List<Sprite> _rightSprites = new List<Sprite>();
        [SerializeField] private List<Sprite> _upSprites = new List<Sprite>();
        [SerializeField] private List<Sprite> _downSprites = new List<Sprite>();
        
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
            if (UnityEngine.Input.GetKeyDown(KeyCode.A))
            {
                _directionType = MoveDirectionType.Left;
                _animController.ChangeSprites(_leftSprites);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.D))
            {
                _directionType = MoveDirectionType.Right;
                _animController.ChangeSprites(_rightSprites);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.W))
            {
                _directionType = MoveDirectionType.Up;
                _animController.ChangeSprites(_upSprites);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.S))
            {
                _directionType = MoveDirectionType.Down;
                _animController.ChangeSprites(_downSprites);
            }
            if (!UnityEngine.Input.GetKey(KeyCode.A) && !UnityEngine.Input.GetKey(KeyCode.D) && !UnityEngine.Input.GetKey(KeyCode.W) && !UnityEngine.Input.GetKey(KeyCode.S))
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
