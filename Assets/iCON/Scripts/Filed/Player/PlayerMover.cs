using System.Collections.Generic;
using iCON.Enums;
using iCON.UI;
using iCON.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

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
        /// 移動のInputActionReference
        /// </summary>
        [Header("入力関連の設定")] 
        [SerializeField]
        private InputActionReference _moveInput;
        
        /// <summary>
        /// ダッシュ切り替えのInputActionReference
        /// </summary>
        [SerializeField] 
        private InputActionReference _dashInput;
        
        /// <summary>
        /// プレイヤーの入力を管理するクラス
        /// </summary>
        private PlayerMoveInput _input;
        
        /// <summary>
        /// 現在向いている方向
        /// </summary>
        private MoveDirectionType _directionType;

        #region Life cycle

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            // 入力を受け取るクラスを生成
            _input = new PlayerMoveInput(_moveInput, _dashInput, UpdateDirection, HandleDash);
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            var direction = DirectionUtility.GetVector2(_directionType);
            // Time.deltaTimeと移動速度を掛けて適切な移動量にする
            Vector3 movement = new Vector3(direction.x, direction.y, 0) * _moveSpeed * Time.deltaTime;
            transform.position += movement;
        }
        
        /// <summary>
        /// Destroy
        /// </summary>
        private void OnDestroy()
        {
            // 入力購読を破棄
            _input.Dispose();
        }

        #endregion

        /// <summary>
        /// 入力に基づいて方向を更新
        /// </summary>
        private void UpdateDirection(InputAction.CallbackContext ctx)
        {
            var moveInput = ctx.ReadValue<Vector2>();
            
            // 入力がない場合
            if (moveInput == Vector2.zero)
            {
                _directionType = MoveDirectionType.None;
                return;
            }
            
            // 水平移動が優先（左右の判定）
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                if (moveInput.x > 0)
                {
                    if (_directionType != MoveDirectionType.Right)
                    {
                        _directionType = MoveDirectionType.Right;
                        _animController.ChangeSprites(_rightSprites);
                    }
                }
                else
                {
                    if (_directionType != MoveDirectionType.Left)
                    {
                        _directionType = MoveDirectionType.Left;
                        _animController.ChangeSprites(_leftSprites);
                    }
                }
            }
            // 垂直移動（上下の判定）
            else
            {
                if (moveInput.y > 0)
                {
                    if (_directionType != MoveDirectionType.Up)
                    {
                        _directionType = MoveDirectionType.Up;
                        _animController.ChangeSprites(_upSprites);
                    }
                }
                else
                {
                    if (_directionType != MoveDirectionType.Down)
                    {
                        _directionType = MoveDirectionType.Down;
                        _animController.ChangeSprites(_downSprites);
                    }
                }
            }
        }

        /// <summary>
        /// ダッシュ切り替え
        /// </summary>
        private void HandleDash(InputAction.CallbackContext ctx)
        {
            // TODO
        }
    }
}
