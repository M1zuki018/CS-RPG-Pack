using System;
using UnityEngine.InputSystem;

namespace iCON.Field.Player
{

    /// <summary>
    /// プレイヤーの移動に関する入力を管理する
    /// </summary>
    public class PlayerMoveInput : IDisposable
    {
        private readonly InputActionReference _moveInput;
        private readonly InputActionReference _dashInput;
        private readonly Action<InputAction.CallbackContext> _onMove;
        private readonly Action<InputAction.CallbackContext> _onDash;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="moveInput">移動のInputActionReference</param>
        /// <param name="dashInput">ダッシュのInputActionReference</param>
        /// <param name="onOnMove">移動アクション</param>
        /// <param name="onOnDash">ダッシュアクション</param>
        public PlayerMoveInput(
            InputActionReference moveInput, InputActionReference dashInput,
            Action<InputAction.CallbackContext> onOnMove, Action<InputAction.CallbackContext> onOnDash)
        {
            _moveInput = moveInput;
            _dashInput = dashInput;
            _onMove = onOnMove;
            _onDash = onOnDash;
            
            _moveInput.action.performed += _onMove;
            _moveInput.action.canceled += _onMove;
            _dashInput.action.performed += _onDash;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _moveInput.action.performed -= _onMove;
            _moveInput.action.canceled -= _onMove;
            _dashInput.action.performed -= _onDash;
        }
    }
}