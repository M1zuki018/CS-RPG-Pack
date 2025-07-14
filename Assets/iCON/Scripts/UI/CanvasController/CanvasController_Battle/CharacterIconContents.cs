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
        private Image _icon;
        
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
        /// HPバーを更新する
        /// </summary>
        public void SetHpSlider(int value, int maxValue)
        {
            if (_hpSlider != null)
            {
                // HP
                _hpSlider.maxValue = maxValue;
                _hpSlider.value = Mathf.Max(value, 0);
            }
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
