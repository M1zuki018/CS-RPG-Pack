using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Constants;
using iCON.Enums;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

namespace iCON.UI
{
    /// <summary>
    /// Story View
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class StoryView : MonoBehaviour
    {
        /// <summary>
        /// 背景管理クラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private UIContents_StoryBackground _background;
        
        /// <summary>
        /// キャラクター立ち絵管理クラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private UIContents_StoryCharacters _characters;
        
        /// <summary>
        /// スチル管理クラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private UIContents_StorySteel _steel;
        
        /// <summary>
        /// ダイアログ管理クラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private UIContents_StoryDialog _dialog;
        
        /// <summary>
        /// フェードパネル管理クラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private UIContents_FadePanel _fadePanel;
        
        /// <summary>
        /// 選択肢表示管理クラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private UIContents_Choice _choice;
        
        /// <summary>
        /// キャンバスを揺らすクラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private CanvasShaker _canvasShaker;
        
        /// <summary>
        /// シーンで使用しているGlobal Volume
        /// </summary>
        [SerializeField, HighlightIfNull]
        private Volume _volume;
        
        public bool IsStopRequested = false;
        
        /// <summary>
        /// 会話テキストを更新する
        /// </summary>
        public Tween SetTalk(string name, string dialog, float duration)
        {
            var tween = _dialog.SetTalk(name, dialog, duration);
            
            if (!_dialog.IsVisible)
            {
                // ダイアログのオブジェクトが非表示だったら表示する
                _dialog.Show(duration / 10);
            }

            return tween;
        }

        /// <summary>
        /// 会話ダイアログをリセット
        /// </summary>
        public void ResetTalk()
        {
            _dialog.ResetTalk();
        }

        /// <summary>
        /// 地の文ダイアログをリセット
        /// </summary>
        public void ResetDescription()
        {
            _dialog.ResetDescription();
        }
        
        /// <summary>
        /// 地の文のテキストを更新する
        /// </summary>
        /// <returns></returns>
        public Tween SetDescription(string description, float duration)
        {
            if (!_dialog.IsVisible)
            {
                _dialog.Show(KStoryPresentation.DIALOG_FADE_DURATION);
            }
            
            return _dialog.SetDescription(description, duration);
        }

        /// <summary>
        /// ダイアログを表示する
        /// </summary>
        public Tween ShowDialog(float duration = 0)
        {
            return _dialog.Show(duration);
        }
        
        /// <summary>
        /// ダイアログを非表示にする
        /// </summary>
        public Tween HideDialog(float duration = 0)
        {
            return _dialog.Hide(duration);
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        public Tween FadeIn(float duration)
        {
            return _fadePanel.FadeIn(duration);
        }

        /// <summary>
        /// フェードアウト
        /// </summary>
        public Tween FadeOut(float duration)
        {
            return _fadePanel.FadeOut(duration);
        }

        /// <summary>
        /// フェードパネルの表示/非表示を即座に切り替える
        /// </summary>
        public void FadePanelSetVisible(bool visible)
        {
            _fadePanel.SetVisible(visible);
        }
        
        /// <summary>
        /// キャラクター立ち絵のSetup
        /// </summary>
        public void SetupCharacter(float scale, Vector3 position)
        {
            _characters.SetupAllCharacters(scale, position);
        }
        
        /// <summary>
        /// キャラクター登場
        /// </summary>
        public async UniTask<Tween> CharacterEntry(CharacterPositionType position, string fileName, float duration)
        {
            return await _characters.Entry(position, fileName, duration);
        }

        /// <summary>
        /// キャラクターを切り替え
        /// </summary>
        public async UniTask<Tween> ChangeCharacter(CharacterPositionType position, string fileName, float duration)
        {
            return await _characters.Change(position, fileName, duration);
        }
        
        /// <summary>
        /// キャラクター退場
        /// </summary>
        public Tween CharacterExit(CharacterPositionType position, float duration)
        {
            return _characters.Exit(position, duration);
        }

        /// <summary>
        /// 全てのキャラクターを非表示にする
        /// </summary>
        public void HideAllCharacters()
        {
            _characters.HideAllCharacters();
        }

        /// <summary>
        /// キャラクター表示のリセット
        /// </summary>
        public void ResetCharacters()
        {
            _characters.ResetAllTransforms();
        }
        
        /// <summary>
        /// スチルを表示/切り替える
        /// </summary>
        public async UniTask<Tween> SetSteel(string fileName, float duration)
        {
            await _steel.SetImageAsync(fileName);
            return _steel.FadeIn(duration);
        }
        
        /// <summary>
        /// スチルを非表示にする
        /// </summary>
        public Tween HideSteel(float duration)
        {
            return _steel.FadeOut(duration);
        }
        
        /// <summary>
        /// 背景を変更する
        /// </summary>
        public async UniTask<Tween> SetBackground(string fileName, float duration)
        {
            await _background.SetImageAsync(fileName);
            return _background.FadeIn(duration);
        }

        /// <summary>
        /// 選択肢を表示する
        /// </summary>
        public void SetupChoice(IReadOnlyList<UIContents_Choice.ViewData> viewDataList, float duration = 0)
        {
            IsStopRequested = true;
            _choice.Setup(viewDataList);
        }

        /// <summary>
        /// カメラシェイク
        /// </summary>
        public Tween CameraShake(float duration, float strengthLate)
        {
            return _canvasShaker.ExplosionShake(duration, strengthLate);
        }
        
        /// <summary>
        /// Volume変更
        /// </summary>
        public async UniTask ChangeGlobalVolume(string volumePath)
        {
            var volumeProfile = await Addressables.LoadAssetAsync<VolumeProfile>(volumePath);
            _volume.sharedProfile = volumeProfile;
        }
    }
   
}