using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using iCON.Utility;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// 汎用のスプライトアニメーション管理クラス
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimationController : MonoBehaviour
    {
        /// <summary>
        /// スプライトアニメーションに使用する画像
        /// </summary>
        [SerializeField]
        private List<Sprite> _sprites;

        /// <summary>
        /// スプライトを切り替えるInterval
        /// </summary>
        [SerializeField] 
        private float _interval = 0.2f;
        
        /// <summary>
        /// ループするかどうか
        /// </summary>
        [SerializeField]
        private bool _loop = true;

        /// <summary>
        /// 自動再生するかどうか
        /// </summary>
        [SerializeField]
        private bool _playOnStart = true;

        /// <summary>
        /// SpriteRenderer
        /// </summary>
        private SpriteRenderer _spriteRenderer;
        
        /// <summary>
        /// 現在表示中のスプライトのIndex
        /// </summary>
        private int _currentIndex = 0;

        /// <summary>
        /// アニメーションが再生中かどうか
        /// </summary>
        private bool _isPlaying = false;

        /// <summary>
        /// 一時停止中かどうか
        /// </summary>
        private bool _isPaused = false;
        
        /// <summary>
        /// CancellationTokenSource
        /// </summary>
        private CancellationTokenSource _cts = new CancellationTokenSource();
        
        /// <summary>
        /// アニメーション開始時のイベント
        /// </summary>
        public event Action OnAnimationStart;

        /// <summary>
        /// アニメーション終了時のイベント
        /// </summary>
        public event Action OnAnimationEnd;
        
        /// <summary>
        /// 再生中
        /// </summary>
        public bool IsPlaying => _isPlaying;
        
        /// <summary>
        /// 一時停止中
        /// </summary>
        public bool IsPaused => _isPaused;
        
        /// <summary>
        /// 現在のフレーム
        /// </summary>
        public int CurrentFrame => _currentIndex;
        
        /// <summary>
        /// スプライトアニメーションのフレーム数
        /// </summary>
        public int TotalFrames => _sprites?.Count ?? 0;
        
        /// <summary>
        /// 進み具合
        /// </summary>
        public float Progress => TotalFrames > 0 ? (float)_currentIndex / TotalFrames : 0f;

        #region Life cycle

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _cts = new CancellationTokenSource();
        }
        
        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            if (_playOnStart && _sprites.Count > 0)
            {
                Play().Forget();
            }
        }

        /// <summary>
        /// OnDestory
        /// </summary>
        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        #endregion
        
        /// <summary>
        /// スプライトアニメーションに使用するスプライトリストを差し替える
        /// </summary>
        public void ChangeSprites(List<Sprite> sprites)
        {
            _sprites = sprites ?? new List<Sprite>();
            _currentIndex = 0;
        }

        /// <summary>
        /// スプライトを切り替えるIntervalを変更する
        /// </summary>
        public void ChangeInterval(float interval)
        {
            _interval = Mathf.Max(0.01f, interval);
        }

        /// <summary>
        /// スプライトの切り替えを止める
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            _isPaused = false;
            _cts?.Cancel();
        }
        
        /// <summary>
        /// アニメーションを一時停止
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// アニメーションを再開
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }

        /// <summary>
        /// 指定フレームに設定
        /// </summary>
        public void SetFrame(int frame)
        {
            if (_sprites == null || _sprites.Count == 0) return;
            
            _currentIndex = Mathf.Clamp(frame, 0, _sprites.Count - 1);
            UpdateSprite();
        }
        

        /// <summary>
        /// ループ設定を変更
        /// </summary>
        public void SetLoop(bool loop)
        {
            _loop = loop;
        }

        /// <summary>
        /// アニメーションをリセット（最初のフレームに戻る）
        /// </summary>
        public void Reset()
        {
            Stop();
            _currentIndex = 0;
            UpdateSprite();
        }
        
        /// <summary>
        /// 外部から直接再生を開始
        /// </summary>
        public void PlayAnimation()
        {
            Play().Forget();
        }

        #region Private Methods

        /// <summary>
        /// スプライトアニメーションを再生
        /// </summary>
        private async UniTask Play()
        {
            if (_sprites == null || _sprites.Count == 0)
            {
                LogUtility.Warning("SpriteAnimationController: スプライトリストが空です", LogCategory.Gameplay, this);
                return;
            }
            
            _spriteRenderer.sprite = _sprites[_currentIndex];

            _isPlaying = true;
            _isPaused = false;
            
            // 新しいCancellationTokenSourceを作成
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            OnAnimationStart?.Invoke();

            try
            {
                // メインアニメーションループ
                while (_isPlaying && !_cts.IsCancellationRequested)
                {
                    if (!_isPaused)
                    {
                        // スプライトを更新
                        UpdateSprite();
                        
                        // 次のフレームに進む
                        _currentIndex++;
                        
                        // ループ処理
                        if (_currentIndex >= _sprites.Count)
                        {
                            if (_loop)
                            {
                                _currentIndex = 0;
                            }
                            else
                            {
                                _isPlaying = false;
                                OnAnimationEnd?.Invoke();
                                break;
                            }
                        }
                    }

                    // 指定されたインターバルだけ待機
                    await UniTask.Delay(TimeSpan.FromSeconds(_interval), cancellationToken: _cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセルされた場合は正常終了
            }
            catch (Exception e)
            {
                Debug.LogError($"SpriteAnimationController: アニメーション中にエラーが発生しました: {e.Message}");
            }
            finally
            {
                _isPlaying = false;
            }
        }

        /// <summary>
        /// スプライトを更新
        /// </summary>
        private void UpdateSprite()
        {
            if (_spriteRenderer != null && _sprites != null && _currentIndex < _sprites.Count)
            {
                _spriteRenderer.sprite = _sprites[_currentIndex];
            }
        }

        #endregion
    }

}
