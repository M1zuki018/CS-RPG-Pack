using Cysharp.Threading.Tasks;
using iCON.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace iCON.UI
{
    /// <summary>
    /// バトルの敵キャラクターのUIを管理するクラス
    /// </summary>
    public class EnemyIconContents : MonoBehaviour
    {
        /// <summary>
        /// キャラクターアイコン
        /// </summary>
        [SerializeField] 
        private Image _icon;
        
        /// <summary>
        /// ダメージテキストの表示位置
        /// </summary>
        [SerializeField]
        private Vector3 _viewPosition = Vector3.one;
        
        /// <summary>
        /// ダメージテキストのオブジェクトプールの参照
        /// </summary>
        private DamageTextPool _damageTextPool;
        
        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(DamageTextPool damageTextPool)
        {
            _damageTextPool = damageTextPool;
        }
        
        /// <summary>
        /// ダメージ量のテキストを表示する
        /// </summary>
        public async UniTask SetDamageText(int value)
        {
            if (_damageTextPool == null)
            {
                LogUtility.Warning("DamageTextPool が null です", LogCategory.UI, this);
                return;
            }

            // ダメージ量のテキストオブジェクトをオブジェクトプールから取得
            var damageText = _damageTextPool.Get();
            
            // 位置を調整し表示を変更
            damageText.rectTransform.localPosition = _viewPosition;
            damageText.SetText(value.ToString());
            
            await UniTask.Delay(500); // TODO: 仮置き。ここでアニメーションをする

            _damageTextPool.Release(damageText);
        }
    }
}
