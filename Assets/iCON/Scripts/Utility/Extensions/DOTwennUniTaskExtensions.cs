using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace iCON.Extensions
{
    /// <summary>
    /// DOTweenのUniTask拡張メソッド
    /// </summary>
    public static class DOTweenUniTaskExtensions
    {
        /// <summary>
        /// TweenをUniTaskに変換する
        /// </summary>
        public static UniTask ToUniTask(this Tween tween, CancellationToken cancellationToken = default)
        {
            if (tween == null || !tween.IsActive())
                return UniTask.CompletedTask;

            var completionSource = new UniTaskCompletionSource();
            
            // キャンセレーション処理
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    tween?.Kill();
                    if (!completionSource.Task.Status.IsCompleted())
                        completionSource.TrySetCanceled(cancellationToken);
                });
            }

            // Tween完了時の処理
            tween.OnComplete(() =>
            {
                if (!completionSource.Task.Status.IsCompleted())
                    completionSource.TrySetResult();
            });

            // Tween強制終了時の処理
            tween.OnKill(() =>
            {
                if (!completionSource.Task.Status.IsCompleted())
                    completionSource.TrySetCanceled();
            });

            return completionSource.Task;
        }

        /// <summary>
        /// SequenceをUniTaskに変換する
        /// </summary>
        public static UniTask ToUniTask(this Sequence sequence, CancellationToken cancellationToken = default)
        {
            if (sequence == null || !sequence.IsActive())
                return UniTask.CompletedTask;

            var completionSource = new UniTaskCompletionSource();

            // キャンセレーション処理
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    sequence?.Kill();
                    if (!completionSource.Task.Status.IsCompleted())
                        completionSource.TrySetCanceled(cancellationToken);
                });
            }

            // Sequence完了時の処理
            sequence.OnComplete(() =>
            {
                if (!completionSource.Task.Status.IsCompleted())
                    completionSource.TrySetResult();
            });

            // Sequence強制終了時の処理
            sequence.OnKill(() =>
            {
                if (!completionSource.Task.Status.IsCompleted())
                    completionSource.TrySetCanceled();
            });

            return completionSource.Task;
        }

        /// <summary>
        /// Tweenの完了を待つ（シンプル版）
        /// </summary>
        public static async UniTask WaitForCompletion(this Tween tween)
        {
            if (tween == null || !tween.IsActive())
                return;

            while (tween.IsActive() && !tween.IsComplete())
            {
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// Sequenceの完了を待つ（シンプル版）
        /// </summary>
        public static async UniTask WaitForCompletion(this Sequence sequence)
        {
            if (sequence == null || !sequence.IsActive())
                return;

            while (sequence.IsActive() && !sequence.IsComplete())
            {
                await UniTask.Yield();
            }
        }
    }
}