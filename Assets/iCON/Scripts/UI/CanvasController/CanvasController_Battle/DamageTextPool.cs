using CryStar.Attribute;
using UnityEngine;
using UnityEngine.Pool;

namespace iCON.UI
{
    /// <summary>
    /// 戦闘でダメージ表記のオブジェクトのオブジェクトプールを管理するクラス
    /// </summary>
    public class DamageTextPool : MonoBehaviour
    {
        [Header("初期設定")]
        [SerializeField, Comment("Prefab")] private CustomText _damageTextPrefab;
        [SerializeField, Comment("初期状態で生成する個数")] private int _defaultCapacity = 5;
        [SerializeField, Comment("最大数")] private int _maxSize = 50;
        [SerializeField, Comment("プールに戻す際に既に同一インスタンスが登録されているか調べ、あれば例外を投げる")] private bool _collectionChecks = true;

        /// <summary>
        /// オブジェクトプール
        /// </summary>
        private IObjectPool<CustomText> _damageTextPool;

        #region Life cycle

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            InitializePool();
        }
        
        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            // ダメージ量が一番手前に表示されるように子オブジェクトの一番下に移動
            transform.SetAsLastSibling();
        }
        
        /// <summary>
        /// Destroy
        /// </summary>
        private void OnDestroy()
        {
            ClearPool();
        }

        #endregion

        /// <summary>
        /// プールからダメージテキストを取得
        /// </summary>
        /// <returns>アクティブ状態のCustomTextオブジェクト</returns>
        public CustomText Get()
        {
            return _damageTextPool.Get();
        }

        /// <summary>
        /// ダメージテキストをプールに返却
        /// </summary>
        /// <param name="damageText">返却するCustomTextオブジェクト</param>
        public void Release(CustomText damageText)
        {
            if (damageText != null)
            {
                _damageTextPool.Release(damageText);
            }
        }

        /// <summary>
        /// プールをクリア
        /// </summary>
        public void ClearPool()
        {
            _damageTextPool?.Clear();
        }

        #region Private Methods

        /// <summary>
        /// プールの初期化
        /// </summary>
        private void InitializePool()
        {
            _damageTextPool = new ObjectPool<CustomText>(
                createFunc: CreateDamageText,
                actionOnGet: OnGetDamageText,
                actionOnRelease: OnReleaseDamageText,
                actionOnDestroy: OnDestroyDamageText,
                collectionCheck: _collectionChecks,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );
        }

        /// <summary>
        /// ダメージテキストオブジェクトの生成
        /// </summary>
        private CustomText CreateDamageText()
        {
            var damageText = Instantiate(_damageTextPrefab, transform);
            damageText.enabled = false;
            return damageText;
        }

        /// <summary>
        /// プールからオブジェクトを取得した際の処理
        /// </summary>
        private void OnGetDamageText(CustomText damageText)
        {
            damageText.enabled = true;
        }

        /// <summary>
        /// プールにオブジェクトを返却した際の処理
        /// </summary>
        private void OnReleaseDamageText(CustomText damageText)
        {
            damageText.enabled = false;
        }

        /// <summary>
        /// オブジェクトが破棄される際の処理
        /// </summary>
        /// <param name="damageText">破棄されるオブジェクト</param>
        private void OnDestroyDamageText(CustomText damageText)
        {
            if (damageText != null)
            {
                Destroy(damageText.gameObject);
            }
        }

        #endregion
    }
}