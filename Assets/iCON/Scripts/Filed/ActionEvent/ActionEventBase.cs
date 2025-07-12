using iCON.Enums;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace iCON.Field.System
{
    /// <summary>
    /// アクションイベントのベースクラス
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public abstract class ActionEventBase : MonoBehaviour
    {
        /// <summary>
        /// インタラクション後の振る舞い
        /// </summary>
        [Header("インタラクション設定")]
        [SerializeField] 
        private InteractionBehaviorType _behaviorType;
        
        /// <summary>
        /// Collider2D
        /// </summary>
        private Collider2D _col;

        #region Life cycle

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            // コライダーを取得。トリガーに設定しておく
            if (TryGetComponent(out _col))
            {
                _col.isTrigger = true;
            }
        }

        #endregion
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            // プレイヤーか判定
            if (!IsValidPlayer(other))
            {
                return;
            }
            
            OnPlayerEnter(other);
            PostExecute();
        }
        
        /// <summary>
        /// プレイヤーかどうかを判定
        /// </summary>
        protected virtual bool IsValidPlayer(Collider2D other)
        {
            return other.CompareTag("Player");
        }
        
        /// <summary>
        /// プレイヤーが範囲に入った時の処理（継承先でオーバーライド）
        /// </summary>
        protected virtual void OnPlayerEnter(Collider2D playerCollider) { }
        
        /// <summary>
        /// イベント実行後の処理
        /// </summary>
        protected virtual void PostExecute()
        {
            switch (_behaviorType)
            {
                case InteractionBehaviorType.OneTime:
                    Destroy(gameObject);
                    break;
            }
        }
    }
}