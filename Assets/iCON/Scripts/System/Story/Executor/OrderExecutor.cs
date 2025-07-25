using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using iCON.UI;
using iCON.Utility;
using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// ストーリーのオーダーを実行するクラス
    /// </summary>
    public class OrderExecutor : IDisposable
    {
        /// <summary>
        /// Viewを操作するクラス
        /// </summary>
        private StoryView _view;
        
        /// <summary>
        /// オーダーを実行中か
        /// </summary>
        private bool _isExecuting;
        
        /// <summary>
        /// 実行中のオーダーのSequence
        /// </summary>
        private Sequence _currentSequence;

        /// <summary>
        /// ストーリー終了時に実行するアクション
        /// </summary>
        private Action _endAction;
        
        /// <summary>
        /// オーダーを実行中か
        /// </summary>
        public bool IsExecuting => _isExecuting;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderExecutor(StoryView view)
        {
            _view = view;
        }

        /// <summary>
        /// Setup
        /// </summary>
        public void Setup(Action endAction)
        {
            _endAction = endAction;
        }

        /// <summary>
        /// オーダーを実行する
        /// </summary>
        public void Execute(OrderData data)
        {
            if (data.Sequence == SequenceType.Append)
            {
                // 念のため実行中のシーケンスがあればキルする
                _currentSequence?.Kill(true);
                _currentSequence = DOTween.Sequence();
            }
            
            _isExecuting = true;
            
            switch (data.OrderType)
            {
                #region case

                case OrderType.Start:
                    HandleStart(data);
                    break;
                case OrderType.Talk:
                    HandleTalk(data);
                    break;
                case OrderType.Descriptive:
                    HandleDescriptive(data);
                    break;
                case OrderType.End:
                    HandleEnd(data);
                    break;
                case OrderType.ChangeBGM:
                    HandleChangeBGM(data);
                    break;
                case OrderType.CharacterEntry:
                    HandleCharacterEntry(data);
                    break;
                case OrderType.CharacterChange:
                    HandleCharacterChange(data);
                    break;
                case OrderType.CharacterExit:
                    HandleCharacterExit(data);
                    break;
                case OrderType.ShowSteel:
                    HandleShowSteel(data);
                    break;
                case OrderType.HideSteel:
                    HandleHideSteel(data);
                    break;
                case OrderType.CameraShake:
                    HandleCameraShake(data);
                    break;
                case OrderType.Choice:
                    HandleChoice(data);
                    break;
                case OrderType.Effect:
                    HandleEffect(data);
                    break;
                case OrderType.ChangeBackground:
                    HandleChangeBackground(data);
                    break;
                case OrderType.Wait:
                    HandleWait(data);
                    break;
                case OrderType.Custom:
                    HandleCustom(data);
                    break;
                case OrderType.ChangeLighting:
                    HandleChangeLighting(data);
                    break;
                case OrderType.FadeIn:
                    HandleFadeIn(data);
                    break;
                case OrderType.FadeOut:
                    HandleFadeOut(data);
                    break;
                case OrderType.PlaySE:
                    HandlePlaySE(data);
                    break;

                #endregion

                default:
                    Debug.LogWarning($"未知のオーダータイプです: {data.OrderType}");
                    break;
            }
            
            if (data.Sequence == SequenceType.Append)
            {
                _currentSequence.OnComplete(() => _isExecuting = false);
            }
        }

        /// <summary>
        /// オーダーの演出をスキップする
        /// </summary>
        public void Skip()
        {
            if (_currentSequence != null && _isExecuting)
            {
                // 演出実行中であれば、シーケンスをキルしてコンプリートの状態にする
                _currentSequence.Kill(true);
                _isExecuting = false;
            }
        }

        /// <summary>
        /// Start - ストーリー開始処理
        /// </summary>
        private void HandleStart(OrderData data)
        {
            LogUtility.Verbose("Story started", LogCategory.System);
            _currentSequence.AddTween(data.Sequence, _view.FadeIn(data.Duration));
        }

        /// <summary>
        /// Talk - キャラクターのセリフ表示
        /// </summary>
        private void HandleTalk(OrderData data)
        {
            _currentSequence.AddTween(data.Sequence, _view.SetTalk(data.DisplayName, data.DialogText, data.Duration));
        }

        /// <summary>
        /// Descriptive - 地の文・説明文表示
        /// </summary>
        private void HandleDescriptive(OrderData data)
        {
            _currentSequence.AddTween(data.Sequence, _view.SetDescription(data.DialogText, data.Duration));
        }

        /// <summary>
        /// End - ストーリー終了処理
        /// </summary>
        private void HandleEnd(OrderData data)
        {
            LogUtility.Verbose("Story ended", LogCategory.System);
            
            // フェードアウト
            _currentSequence.AddTween(data.Sequence, _view.FadeOut(data.Duration));
            
            // 終了時の処理を実行
            _currentSequence.OnKill(HandleReset);
        }

        /// <summary>
        /// ChangeBGM - BGM変更
        /// </summary>
        private async void HandleChangeBGM(OrderData data)
        {
            var tween = await AudioManager.Instance.CrossFadeBGM(data.FilePath, data.Duration);
            _currentSequence.AddTween(data.Sequence, tween);
        }

        /// <summary>
        /// CharacterEntry - キャラクター登場
        /// </summary>
        private void HandleCharacterEntry(OrderData data)
        {
            _currentSequence.AddTween(data.Sequence,_view.CharacterEntry(data.Position, data.FacialExpressionPath, data.Duration));
        }

        /// <summary>
        /// CharacterChange - キャラクター切り替え
        /// </summary>
        private async UniTask HandleCharacterChange(OrderData data)
        {
            var tween = await _view.ChangeCharacter(data.Position, data.FacialExpressionPath, data.Duration);
            _currentSequence.AddTween(data.Sequence, tween);
        }

        /// <summary>
        /// CharacterExit - キャラクター退場
        /// </summary>
        private void HandleCharacterExit(OrderData data)
        {
            _currentSequence.AddTween(data.Sequence,_view.CharacterExit(data.Position, data.Duration));
        }

        /// <summary>
        /// ShowSteel - スチル画像表示
        /// </summary>
        private async void HandleShowSteel(OrderData data)
        {
            var tween = await _view.SetSteel(data.FilePath, data.Duration);
            _currentSequence.AddTween(data.Sequence, tween);
        }

        /// <summary>
        /// HideSteel - スチル画像非表示
        /// </summary>
        private void HandleHideSteel(OrderData data)
        {
            _currentSequence.AddTween(data.Sequence,_view.HideSteel(data.Duration));
        }

        /// <summary>
        /// CameraShake - カメラシェイク
        /// </summary>
        private void HandleCameraShake(OrderData data)
        {
            _currentSequence.AddTween(data.Sequence, _view.CameraShake(data.Duration, data.OverrideTextSpeed));
        }

        /// <summary>
        /// Choice - 選択肢表示
        /// </summary>
        private void HandleChoice(OrderData data)
        {
            // TODO
        }

        /// <summary>
        /// Effect - エフェクト再生
        /// </summary>
        private void HandleEffect(OrderData data)
        {
            // TODO
        }

        /// <summary>
        /// ChangeBackground - 背景変更
        /// </summary>
        private async void HandleChangeBackground(OrderData data)
        {
            var tween = await _view.SetBackground(data.FilePath, data.Duration);
            _currentSequence.AddTween(data.Sequence, tween);
        }

        /// <summary>
        /// Wait - 待機処理
        /// </summary>
        private void HandleWait(OrderData data)
        {
            _currentSequence.AppendInterval(data.Duration);
        }
        
        /// <summary>
        /// ChangeLighting - Global Volume変更処理
        /// </summary>
        private void HandleChangeLighting(OrderData data)
        {
            _view.ChangeGlobalVolume(data.FilePath);
        }
        
        /// <summary>
        /// FadeIn - フェードイン
        /// </summary>
        private void HandleFadeIn(OrderData data)
        {
            _currentSequence.AddTween(data.Sequence, _view.FadeIn(data.Duration));
        }
        
        /// <summary>
        /// FadeOut - フェードアウト
        /// </summary>
        private void HandleFadeOut(OrderData data)
        {
            _currentSequence.AddTween(data.Sequence, _view.FadeOut(data.Duration));
        }

        /// <summary>
        /// PlaySE - SEを再生する
        /// </summary>
        private void HandlePlaySE(OrderData data)
        {
            AudioManager.Instance.PlaySE(data.FilePath, data.OverrideTextSpeed).Forget();
        }
        
        /// <summary>
        /// Custom - カスタムオーダー処理
        /// </summary>
        private void HandleCustom(OrderData data)
        {
            // TODO
        }
        
        /// <summary>
        /// ストーリー終了時のリセット処理
        /// </summary>
        private void HandleReset()
        {
            // 全キャラクター非表示
            _view.HideAllCharacters();
            // スチル非表示
            _view.HideSteel(0);
            //ダイアログをリセット
            _view.ResetTalk();
            _view.ResetDescription();

            // BGMのAudioSourceの音量をゼロに変更
            AudioManager.Instance.SetBGMVolume(0);
            
            _endAction?.Invoke();
        }

        public void Dispose()
        {
            if (_endAction != null)
            {
                // アクションが登録されていたら破棄する
                _endAction = null;
            }
        }
    }
}
