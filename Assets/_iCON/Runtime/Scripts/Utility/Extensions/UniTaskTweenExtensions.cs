using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace iCON.Extensions
{
    /// <summary>
    /// UniTaskのTween拡張メソッド
    /// </summary>
    public static class UniTaskTweenExtensions
    {
        /// <summary>
        /// UniTask<Tween>をTweenに変換する拡張メソッド
        /// </summary>
        public static Tween ToTween(this UniTask<Tween> uniTask)
        {
            var sequence = DOTween.Sequence().SetAutoKill(false).Pause();
            
            // Sequenceの状態を管理するためのフラグ
            bool isSequenceActive = true;
            
            // Sequenceが手動でKillされた場合の処理
            sequence.OnKill(() => isSequenceActive = false);
            
            ConvertAsync(uniTask, sequence, () => isSequenceActive).Forget();
            
            return sequence;
        }
        
        /// <summary>
        /// UniTaskをTweenに変換する拡張メソッド（戻り値なしの場合）
        /// </summary>
        public static Tween ToTween(this UniTask uniTask, float duration = 0f)
        {
            var sequence = DOTween.Sequence();
            
            if (duration > 0f)
            {
                // ダミーのTweenを作成して期間を設定
                sequence.AppendInterval(duration);
            }
            
            // 非同期処理の完了を待つ
            ConvertAsync(uniTask, sequence).Forget();
            
            return sequence;
        }
        
        private static async UniTaskVoid ConvertAsync(UniTask<Tween> uniTask, Sequence sequence, Func<bool> isActiveCheck)
        {
            try
            {
                var tween = await uniTask;
                
                if (tween != null && sequence.IsActive())
                {
                    sequence.Append(tween);
                    // Tweenを追加した後に再生開始
                    sequence.Play();
                }
                else if (sequence.IsActive())
                {
                    // Tweenがnullの場合は空のSequenceを再生
                    sequence.Play();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"UniTask to Tween conversion failed: {e.Message}");
                
                // エラー時にSequenceがまだ有効な場合のみKill
                if (isActiveCheck() && sequence.IsActive())
                {
                    sequence?.Kill();
                }
            }
        }
        
        private static async UniTask ConvertAsync(UniTask uniTask, Sequence sequence)
        {
            try
            {
                await uniTask;
                
                // UniTaskが完了したらSequenceも完了
                sequence?.Complete();
            }
            catch (Exception e)
            {
                Debug.LogError($"UniTask to Tween conversion failed: {e.Message}");
                sequence?.Kill();
            }
        }
    }
}