using UnityEngine;

namespace iCON.Input
{
    /// <summary>
    /// 入力処理を管理するインターフェース
    /// </summary>
    public interface IPlayerInputReceiver
    {
        /// <summary>移動</summary>
        void HandleMoveInput(Vector2 input);
        
        /// <summary>ダッシュ</summary>
        void HandleDashInput();
        
        /// <summary>決定</summary>
        void HandleConfirmInput();
        
        /// <summary>ポーズ</summary>
        void HandlePauseInput();
        
        /// <summary>ショートカットを開く</summary>
        void HandleShortcutInput();
        
        /// <summary>キャラクターメニューを開く</summary>
        void HandleCharaMenuInput();
    }
}