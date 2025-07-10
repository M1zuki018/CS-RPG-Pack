using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace iCON.Performance
{
    /// <summary>
    /// タイトルスプラッシュの演出を管理するクラス
    /// 【演出フロー】
    /// 1. ロゴ表示 → フェードイン/アウト
    /// 2. ブルースクリーン演出 → 背景色を青に変更
    /// 3. 年齢制限表示 → フェードイン/アウト
    /// 4. 終了フェード → タイトルシーンへ遷移
    /// </summary>
    public class TitleSplashManager : ViewBase
    {
        /// <summary>
        ///  タイトルスプラッシュのアニメーション終了
        /// </summary>
        public event Action OnFinishedTitleSplash; 
        
        [FormerlySerializedAs("_prefabCanvasGroup")] [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField, Comment("演出終了時のフェードアウトにかける秒数")] private float _endFadeDuration = 3f;
        
        [Header("ロゴ関連の設定")]
        [SerializeField] private Image _logo;
        [SerializeField, Comment("表示/非表示にかける秒数")] private float _logoFadeDuration = 1f;
        [SerializeField, Comment("表示時間")] private float _logoDisplayTime = 3f;
        
        [Header("背景の設定")]
        [SerializeField] private Image _background;
        [SerializeField, Comment("デフォルト色")] private Color _defaultColor = Color.white;
        [SerializeField, Comment("ブルースクリーン時に使用する色")] private Color _blueScreenColor = Color.blue;
        [SerializeField, Comment("ブルースクリーン状態の待機時間")] private float _blueScreenDisplayTime = 2f;

        [Header("注意書きの設定")]
        [SerializeField] private CanvasGroup _cautionaryNote;
        [SerializeField, Comment("表示/非表示にかける秒数")] private float _cautionaFadeDuration = 1f;
        [SerializeField, Comment("表示時間")] private float _cautionDisplayTime = 5f;
        
        /// <summary>
        /// 現在再生中のシーケンス
        /// </summary>
        private Sequence _sequence;

        #region Life cycle

        /// <summary>
        /// Awake
        /// </summary>
        public override UniTask OnAwake()
        {
            // 非表示/デフォルト色にセット
            _logo.DOFade(0f, 0f);
            _cautionaryNote.DOFade(0f, 0f);
            _background.color = _defaultColor;
            
            return base.OnAwake();
        }
        
        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        #endregion
        
        /// <summary>
        /// 演出を開始する
        /// </summary>
        public void Play()
        {
            LogoAnimation();
        }

        /// <summary>
        /// 演出をスキップする
        /// </summary>
        public void Skip()
        {
            _sequence?.Kill(true);
        }

        #region 演出

        /// <summary>
        /// 1. ロゴの演出
        /// </summary>
        private void LogoAnimation()
        {
            var seq = DOTween.Sequence()
                
                // フェードイン
                .Append(_logo.DOFade(1f, _logoFadeDuration))
                
                // 指定秒数待つ
                .AppendInterval(_logoDisplayTime)
                
                // フェードアウト
                .Append(_logo.DOFade(0f, _logoDisplayTime));
            
            _sequence = seq;

            // キルされたらブルースクリーンアニメーションへ遷移
            seq.OnKill(BlueScreenAnimation);
        }

        /// <summary>
        /// 2. ブルースクリーンの演出
        /// </summary>
        private void BlueScreenAnimation()
        {
            var seq = DOTween.Sequence()

                // 即座に背景色を変更して驚かせる
                .Append(_background.DOColor(_blueScreenColor, 0.1f))
                
                // 指定秒数待つ
                .AppendInterval(_blueScreenDisplayTime);
            
            _sequence = seq;
            
            // 注意書きアニメーションへ遷移
            seq.OnKill(ShowCautionaryNoteAnimation);
        }

        /// <summary>
        /// 3. 注意書きのアニメーション
        /// </summary>
        private void ShowCautionaryNoteAnimation()
        {
            var seq = DOTween.Sequence()
                
                // フェードイン
                .Append(_cautionaryNote.DOFade(1f, _cautionaFadeDuration))
                
                // 指定秒数待つ
                .AppendInterval(_cautionDisplayTime)
                
                // フェードアウト
                .Append(_cautionaryNote.DOFade(0f, _cautionaFadeDuration));
            
            _sequence = seq;
            
            seq.OnKill(EndAnimation);
        }

        /// <summary>
        /// 4. 演出終了
        /// </summary>
        private void EndAnimation()
        {
            var seq = DOTween.Sequence()
         
                // フェードアウト
                .Append(_canvasGroup.DOFade(0f, _endFadeDuration));
            
            _sequence = seq;
            
            seq.OnKill(FinishedAnimation);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// アニメーション終了後の処理
        /// </summary>
        private void FinishedAnimation()
        {
            OnFinishedTitleSplash?.Invoke();
            gameObject.SetActive(false);
        }

        #endregion
    }
}
