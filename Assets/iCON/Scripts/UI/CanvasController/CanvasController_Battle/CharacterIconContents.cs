using Cysharp.Threading.Tasks;
using iCON.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace iCON.UI
{
    /// <summary>
    /// バトルのキャラクターのUIを管理するクラス
    /// </summary>
    public class CharacterIconContents : MonoBehaviour
    {
        /// <summary>
        /// 背景
        /// </summary>
        [SerializeField] 
        private Image _background;
        
        /// <summary>
        /// キャラクターアイコン
        /// </summary>
        [SerializeField]
        private CustomImage _icon;
        
        /// <summary>
        /// HPバー
        /// </summary>
        [SerializeField]
        private Slider _hpSlider;
        
        /// <summary>
        /// スキルポイントバー
        /// </summary>
        [SerializeField]
        private Slider _spSlider;
        
        /// <summary>
        /// ダメージテキストのオブジェクトプールの参照
        /// </summary>
        private DamageTextPool _damageTextPool;

        /// <summary>
        /// 自身のRectTransform
        /// </summary>
        private RectTransform _rectTransform;
        
        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            // ダメージ量のテキストの位置を変更するために自身のRectTransformの参照を取得しておく
            if (!TryGetComponent(out _rectTransform))
            {
                LogUtility.Error("RectTransform が見つかりません", LogCategory.UI, this);
            }
        }
        
        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(DamageTextPool damageTextPool)
        {
            _damageTextPool = damageTextPool;
        }

        /// <summary>
        /// アイコンを差し替える
        /// </summary>
        public void SetIcon(string spritePath)
        {
            _icon.AssetName = spritePath;
        }
        
        /// <summary>
        /// HPバーを更新する
        /// </summary>
        public void SetHpSlider(int value, int maxValue)
        {
            if (_hpSlider != null)
            {
                _hpSlider.maxValue = maxValue;
                
                // 0以下にならないようにしてvalueに代入
                _hpSlider.value = Mathf.Max(value, 0);
            }
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
            damageText.rectTransform.localPosition = _rectTransform.localPosition;
            damageText.SetText(value.ToString());
            
            await UniTask.Delay(500); // TODO: 仮置き。ここでアニメーションをする

            _damageTextPool.Release(damageText);
        }

        /// <summary>
        /// スキルポイントバーを更新する
        /// </summary>
        public void SetSpSlider(int value, int maxValue)
        {
            if (_spSlider != null)
            {
                _spSlider.maxValue = maxValue;
                _spSlider.value = Mathf.Max((float)value / maxValue, 0);
            }
        }
    }
}
