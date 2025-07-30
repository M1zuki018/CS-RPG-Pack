using CryStar.Story.Constants;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CryStar.UI
{
    /// <summary>
    /// Imageを利用するUIContentsのベースクラス
    /// </summary>
    [RequireComponent(typeof(Image))]
    public abstract class UIContentsImageBase : UIContentsFadeableBase, IColorControllable
    {
        /// <summary>
        /// Imageコンポーネント
        /// </summary>
        protected Image _image;
        
        /// <summary>
        /// Imageが表示状態か
        /// </summary>
        public override bool IsVisible => _image != null && _image.enabled && _image.color.a > 0;
        
        /// <summary>
        /// Imageの透明度
        /// </summary>
        public override float Alpha => _image != null ? _image.color.a : 0;
        
        /// <summary>
        /// Imageの色
        /// </summary>
        public Color Color
        {
            get => _image.color;
            set => _image.color = value;
        }
        
        /// <summary>
        /// 初期化
        /// </summary>
        public override void Initialize()
        {
            _image = GetComponent<Image>();
        }
        
        /// <summary>
        /// 表示/非表示を即座に切り替える
        /// </summary>
        public override void SetVisibility(bool isVisible)
        {
            SetAlpha(isVisible ? 1f : 0f);
        }

        /// <summary>
        /// 終わりの透明度を指定してフェードアニメーションを実行する
        /// </summary>
        public override Tween FadeToAlpha(float targetAlpha, float duration, Ease ease = KStoryPresentation.FADE_EASE)
        {
            targetAlpha = Mathf.Clamp01(targetAlpha);
            
            _currentTween?.Kill();
            _currentTween = _image.DOFade(targetAlpha, duration).SetEase(ease);
            
            return _currentTween;
        }
        
        /// <summary>
        /// 即座にアルファ値を設定
        /// </summary>
        public override void SetAlpha(float alpha)
        {
            _currentTween?.Kill();
            
            var color = _image.color;
            color.a = Mathf.Clamp01(alpha);
            _image.color = color;
        }
        
        /// <summary>
        /// カラーとアルファを同時に設定
        /// </summary>
        public void SetColorWithAlpha(Color color, float alpha)
        {
            // Tweenがあれば止める
            _currentTween?.Kill();
            
            // Tweenを止めたあとに確実に色を変更する
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
    }
}