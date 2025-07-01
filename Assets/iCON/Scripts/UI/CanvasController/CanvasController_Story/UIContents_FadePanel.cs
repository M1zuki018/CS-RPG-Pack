using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace iCON.UI
{
    /// <summary>
    /// UIContents フェードパネル
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UIContents_FadePanel : MonoBehaviour
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
        
        private Image _image;
        private Tween _currentTween;
        
        /// <summary>
        /// 現在のアルファ値
        /// </summary>
        public float Alpha => _image.color.a;

        /// <summary>
        /// パネルが表示状態かどうか
        /// </summary>
        public bool IsVisible => Alpha > 0f;

        /// <summary>
        /// 現在のパネルカラー
        /// </summary>
        public Color Color
        {
            get => _image.color;
            set => _image.color = value;
        }

        #region Lifecycle

        private void Awake()
        {
            _image = GetComponent<Image>();
            Initialize();
        }

        #endregion

        /// <summary>
        /// フェードイン/アウトアニメーション
        /// </summary>
        /// <param name="targetAlpha">目標アルファ値 (0-1)</param>
        /// <param name="duration">アニメーション時間</param>
        /// <param name="ease">イージング</param>
        public Tween FadeToAlpha(float targetAlpha, float duration, Ease ease = Ease.Linear)
        {
            targetAlpha = Mathf.Clamp01(targetAlpha);
            
            _currentTween?.Kill();
            _currentTween = _image.DOFade(targetAlpha, duration).SetEase(ease);
            
            return _currentTween;
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        public Tween FadeIn(float duration, Ease ease = Ease.Linear)
        {
            return FadeToAlpha(0f, duration, ease);
        }

        /// <summary>
        /// フェードアウト
        /// </summary>
        public Tween FadeOut(float duration, Ease ease = Ease.Linear)
        {
            return FadeToAlpha(1f, duration, ease);
        }

        /// <summary>
        /// 即座にアルファ値を設定
        /// </summary>
        public void SetAlpha(float alpha)
        {
            _currentTween?.Kill();
            
            var color = _image.color;
            color.a = Mathf.Clamp01(alpha);
            _image.color = color;
        }

        /// <summary>
        /// 表示/非表示を即座に切り替え
        /// </summary>
        public void SetVisible(bool visible)
        {
            SetAlpha(visible ? 1f : 0f);
        }

        /// <summary>
        /// カラーとアルファを同時に設定
        /// </summary>
        public void SetColorWithAlpha(Color color, float alpha)
        {
            _currentTween?.Kill();
            
            color.a = Mathf.Clamp01(alpha);
            _image.color = color;
        }

        /// <summary>
        /// アルファを保持してカラーのみ変更
        /// </summary>
        public void SetColorKeepAlpha(Color color)
        {
            color.a = _image.color.a;
            _image.color = color;
        }
        
        /// <summary>
        /// 現在のアニメーションを停止
        /// </summary>
        public void StopAnimation()
        {
            _currentTween?.Kill();
            _currentTween = null;
        }

        #region Private Methods

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            SetColorWithAlpha(_defaultColor, _startVisible ? 1f : 0f);
        }

        #endregion
    }
   
}