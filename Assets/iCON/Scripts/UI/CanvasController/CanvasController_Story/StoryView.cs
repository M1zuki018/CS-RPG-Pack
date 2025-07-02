using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Enums;
using UnityEngine;

namespace iCON.UI
{
    /// <summary>
    /// Story View
    /// </summary>
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
        /// キャンバスを揺らすクラス
        /// </summary>
        [SerializeField, HighlightIfNull]
        private CanvasShaker _canvasShaker;

        /// <summary>
        /// 会話テキストを更新する
        /// </summary>
        public Tween SetTalk(string name, string dialog, float duration)
        {
            var tween = _dialog.SetTalk(name, dialog, duration);
            
            if (!_dialog.IsVisible)
            {
                // ダイアログのオブジェクトが非表示だったら表示する
                _dialog.Show();
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
                _dialog.Show();
            }
            
            return _dialog.SetDescription(description, duration);
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
        /// キャラクター登場
        /// </summary>
        public Tween CharacterEntry(CharacterPositionType position, string fileName, float duration)
        {
            return _characters.Entry(position, fileName, duration);
        }

        /// <summary>
        /// キャラクター退場
        /// </summary>
        public Tween CharacterExit(CharacterPositionType position, float duration)
        {
            return _characters.Exit(position, duration);
        }
        
        /// <summary>
        /// キャラクターを切り替える
        /// </summary>
        public void ChangeCharacter(CharacterPositionType position, string fileName)
        {
            _characters.ChangeSprite(position, fileName);
        }

        /// <summary>
        /// 全てのキャラクターを非表示にする
        /// </summary>
        public void HideAllCharacters()
        {
            _characters.HideAll();
        }
        
        /// <summary>
        /// スチルを表示/切り替える
        /// </summary>
        public async UniTask SetSteel(string fileName)
        {
            await _steel.SetImageAsync(fileName);
    
            if (!_steel.IsVisible)
            {
                _steel.Show();
            }
            
            _steel.FadeIn();
        }
        
        /// <summary>
        /// スチルを非表示にする
        /// </summary>
        public void HideSteel()
        {
            _steel.FadeOut();
        }
        
        /// <summary>
        /// 背景を変更する
        /// </summary>
        public async UniTask SetBackground(string fileName)
        {
            await _background.SetImageAsync(fileName);
            _background.FadeIn();
        }

        /// <summary>
        /// カメラシェイク
        /// </summary>
        public Tween CameraShake(float duration, float strengthLate)
        {
            return _canvasShaker.ExplosionShake(duration, strengthLate);
        }
    }
   
}