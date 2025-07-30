using CryStar.UI;
using DG.Tweening;
using UnityEngine;

namespace CryStar.Story.UI
{
    /// <summary>
    /// UIContents フェードパネル
    /// </summary>
    public class UIContents_FadePanel : UIContentsImageBase
    {
        /// <summary>
        /// デフォルトの色
        /// </summary>
        [SerializeField]
        private Color _defaultColor = Color.black;
        
        /// <summary>
        /// ゲーム開始時にα値を1にするか
        /// </summary>
        [SerializeField] 
        private bool _startVisible = false;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            SetColorWithAlpha(_defaultColor, _startVisible ? 1f : 0f);
        }

        /// <summary>
        /// 画面のフラッシュ演出
        /// </summary>
        public Tween Flash(float duration, Color panelColor)
        {
            // アルファ値はゼロのまま、色を切り替える
            SetColorWithAlpha(panelColor, 0);

            var sequence = DOTween.Sequence()
                
                // NOTE: アルファ値を1にしてしまうとその後ろが見えなくなってしまうので、1までは上げないようにする
                .Append(FadeToAlpha(0.5f, duration * 0.3f, Ease.InQuart))
                .Append(FadeOut(duration * 0.7f, Ease.OutSine))
                
                // NOTE: デフォルトの色に戻しておく
                .OnComplete(() => SetColorWithAlpha(_defaultColor, 0));
            
            return sequence;
        }
    }
   
}