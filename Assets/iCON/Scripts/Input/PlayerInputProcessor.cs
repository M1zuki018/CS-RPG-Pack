using System;
using UnityEngine;

namespace iCON.Input
{
    /// <summary>
    /// プレイヤーの各入力ごとに呼び出す処理をまとめているクラス
    /// </summary>
    public class PlayerInputProcessor : IPlayerInputReceiver
    {
        private IPlayerInputReceiver _playerInputReceiverImplementation;
        public event Action OnLockOn;
        
        /// <summary>移動入力処理。Vector3への変換だけ行う</summary>
        public void HandleMoveInput(Vector2 input)
        {
        }

        /// <summary>歩き状態にする入力処理</summary>
        public void HandleShortcutInput()
        {
            
        }

        public void HandleCharaMenuInput()
        {
            _playerInputReceiverImplementation.HandleCharaMenuInput();
        }

        public void HandleConfirmInput()
        {
            _playerInputReceiverImplementation.HandleConfirmInput();
        }

        /// <summary>ポーズ入力処理</summary>
        public void HandlePauseInput()
        {
        }
        
        /// <summary>ロックオン入力処理</summary>
        public void HandleLockOnInput() => OnLockOn?.Invoke();

        /// <summary>ジャンプ入力処理</summary>
        public void HandleDashInput()
        {
            
        }
        
    }
}