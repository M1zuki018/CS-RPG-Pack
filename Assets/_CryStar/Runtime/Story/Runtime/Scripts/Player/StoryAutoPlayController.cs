using System;
using System.Threading;
using CryStar.Story.Constants;
using Cysharp.Threading.Tasks;

namespace CryStar.Story.Player
{
    /// <summary>
    /// オート再生の処理を担当するクラス
    /// </summary>
    public class StoryAutoPlayController : IDisposable
    {
        /// <summary>
        /// シナリオ再生アクション
        /// </summary>
        private event Action _onAutoPlayTriggered;
        
        /// <summary>
        /// オート再生モード
        /// </summary>
        private bool _isAutoPlayMode;
        
        /// <summary>
        /// オート再生開始予約済み
        /// </summary>
        private bool _isAutoPlayReserved = false;
        
        /// <summary>
        /// CancellationTokenSource
        /// </summary>
        private CancellationTokenSource _cts = new CancellationTokenSource();
        
        /// <summary>
        /// オート再生モード
        /// </summary>
        public bool IsAutoPlayMode => _isAutoPlayMode;
        
        /// <summary>
        /// オート再生開始予約済み
        /// </summary>
        public bool IsAutoPlayReserved => _isAutoPlayReserved;
        
        /// <summary>
        /// まだオート再生を予約されていない
        /// NOTE: オート再生モードなのにまだ予約がない場合Trueになる
        /// </summary>
        public bool NotYetRequest => !_isAutoPlayReserved && _isAutoPlayMode;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StoryAutoPlayController(Action executeAction)
        {
            _onAutoPlayTriggered = executeAction;
        }
        
        /// <summary>
        /// オート再生
        /// </summary>
        public async UniTask AutoPlay()
        {
            _isAutoPlayReserved = true;
            
            // 念のためキャンセル処理を挟んでから始める
            CancelAutoPlay();
            
            // 新しいCancellationTokenSourceを作成
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            
            try
            {
                // 定数で設定しているインターバル分待機してから次のオーダーを実行する
                await UniTask.Delay(TimeSpan.FromSeconds(KStoryPresentation.AUTO_PLAY_INTERVAL), cancellationToken: token);
                
                // キャンセルされていない場合のみ次のオーダーを実行
                if (!token.IsCancellationRequested)
                {
                    _onAutoPlayTriggered?.Invoke();
                }
            }
            catch (OperationCanceledException)
            {
                // オート再生がキャンセルされた
            }
            finally
            {
                // 予約が実行されたのでフラグを戻す
                _isAutoPlayReserved = false;
            }
        }

        /// <summary>
        /// オート再生モードの切り替え処理
        /// </summary>
        public bool ToggleAutoPlayMode()
        {
            _isAutoPlayMode = !_isAutoPlayMode;
            if (!_isAutoPlayMode)
            {
                // オート再生をキャンセル
                CancelAutoPlay();
            }
            
            return _isAutoPlayMode;
        }
        
        /// <summary>
        /// オート再生をキャンセルする処理
        /// </summary>
        public void CancelAutoPlay()
        {
            if (_cts != null)
            {
                // オート再生用のUniTaskをキャンセルする
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            CancelAutoPlay();
        }
    }
}
