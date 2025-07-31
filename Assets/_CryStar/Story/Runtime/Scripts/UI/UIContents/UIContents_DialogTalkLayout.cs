using CryStar.UI;
using CryStar.Utility;
using CryStar.Utility.Enum;
using DG.Tweening;
using UnityEngine;

namespace CryStar.Story.UI
{
    /// <summary>
    /// UIContents 名前つきのダイアログ
    /// ベースクラス内でCanvasGroupでのフェード処理にも対応している
    /// </summary>
    public class UIContents_DialogTalkLayout : UIContentsCanvasGroupBase, ITalkLayout
    {
        /// <summary>
        /// 名前のText
        /// </summary>
        [SerializeField]
        private CustomText _name;

        /// <summary>
        /// 会話文のText
        /// </summary>
        [SerializeField] 
        private CustomText _dialog;

        /// <summary>
        /// 初期化済みか
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            // エラーがあるか、この変数に記録する
            bool hasError = false;
            
            if (_name == null)
            {
                LogUtility.Error($"_name が null です。割り当てを行ってください", LogCategory.UI, this);
                hasError = true;
            }

            if (_dialog == null)
            {
                LogUtility.Error($"_dialog が null です。割り当てを行ってください", LogCategory.UI, this);
                hasError = true;
            }
            
            _isInitialized = !hasError;
        }

        /// <summary>
        /// 表示テキストを変更する
        /// </summary>
        public Tween SetTalk(string name, string dialog, float duration = 0)
        {
            if (!_isInitialized)
            {
                // NOTE: 初期化が正しく行われていない場合はコンポーネントがnullになるためreturnしておく
                return null;    
            }

            if (!IsVisible)
            {
                SetVisibility(true);
            }
            
            SetName(name);
            return SetDialog(dialog, duration);
        }

        /// <summary>
        /// 名前のみを設定する
        /// </summary>
        public void SetName(string name)
        {
            if (!_isInitialized) return;
            
            if (!IsVisible)
            {
                SetVisibility(true);
            }
            
            _name.text = name ?? string.Empty;
        }
        
        /// <summary>
        /// 会話文のみを設定する
        /// </summary>
        public Tween SetDialog(string dialog, float duration = 0)
        {
            if (!_isInitialized)
            {
                return null;
            }
            
            if (!IsVisible)
            {
                SetVisibility(true);
            }

            // テキストボックスを空にしてから始める
            _dialog.text = string.Empty;
            return _dialog.DOText(dialog ?? string.Empty, duration).SetEase(Ease.Linear);
        }

        /// <summary>
        /// テキストをクリアする
        /// </summary>
        public void ClearText()
        {
            SetTalk(string.Empty, string.Empty);
        }
        
        public Tween SetText(string text, float duration = 0) => SetDialog(text, duration);
    }
}