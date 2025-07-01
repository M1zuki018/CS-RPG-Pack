using System;
using Cysharp.Threading.Tasks;
using iCON.Enums;
using iCON.UI;
using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// ストーリーのオーダーを実行するクラス
    /// </summary>
    public class OrderExecutor
    {
        private StoryView _view;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderExecutor(StoryView view)
        {
            _view = view;
        }

        /// <summary>
        /// オーダーを実行する
        /// </summary>
        public void Execute(OrderData data)
        {
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

                #endregion

                default:
                    Debug.LogWarning($"未知のオーダータイプです: {data.OrderType}");
                    break;
            }
        }

        /// <summary>
        /// Start - ストーリー開始処理
        /// </summary>
        private void HandleStart(OrderData data)
        {
            Debug.Log("Story started");
            _view.HideAll();
            // TODO
        }

        /// <summary>
        /// Talk - キャラクターのセリフ表示
        /// </summary>
        private void HandleTalk(OrderData data)
        {
            _view.SetTalk(data.DisplayName, data.DialogText);
        }

        /// <summary>
        /// Descriptive - 地の文・説明文表示
        /// </summary>
        private void HandleDescriptive(OrderData data)
        {
            _view.SetDescription(data.DialogText);
        }

        /// <summary>
        /// End - ストーリー終了処理
        /// </summary>
        private void HandleEnd(OrderData data)
        {
            Debug.Log("Story ended");
            _view.HideAll();
            // TODO
        }

        /// <summary>
        /// ChangeBGM - BGM変更
        /// </summary>
        private void HandleChangeBGM(OrderData data)
        {
            // TODO
        }

        /// <summary>
        /// CharacterEntry - キャラクター登場
        /// </summary>
        private void HandleCharacterEntry(OrderData data)
        {
            _view.InCharacter(data.Position, data.FilePath);
        }

        /// <summary>
        /// CharacterExit - キャラクター退場
        /// </summary>
        private void HandleCharacterExit(OrderData data)
        {
            _view.OutCharacter(data.Position);
        }

        /// <summary>
        /// ShowSteel - スチル画像表示
        /// </summary>
        private void HandleShowSteel(OrderData data)
        {
            _view.SetSteel(data.FilePath);
        }

        /// <summary>
        /// HideSteel - スチル画像非表示
        /// </summary>
        private void HandleHideSteel(OrderData data)
        {
            _view.HideSteel();
        }

        /// <summary>
        /// CameraShake - カメラシェイク
        /// </summary>
        private void HandleCameraShake(OrderData data)
        {
            // TODO
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
        private void HandleChangeBackground(OrderData data)
        {
            _view.SetBackground(data.FilePath);
        }

        /// <summary>
        /// Wait - 待機処理
        /// </summary>
        private async UniTask HandleWait(OrderData data)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(data.Duration));
        }

        /// <summary>
        /// Custom - カスタムオーダー処理
        /// </summary>
        private void HandleCustom(OrderData data)
        {
            // TODO
        }
    }
}
