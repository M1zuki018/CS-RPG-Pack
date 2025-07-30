using CryStar.UI;
using DG.Tweening;
using iCON.Utility;
using UnityEngine;

namespace CryStar.Story.UI
{
    /// <summary>
    /// UIContents 名前なしの地の文のダイアログ
    /// ベースクラス内でCanvasGroupでのフェード処理にも対応している
    /// </summary>
    public class UIContents_DialogDescriptionLayout : UIContentsCanvasGroupBase, IDescriptionLayout
    {
        /// <summary>
        /// 地の文のText
        /// </summary>
        [SerializeField] 
        private CustomText _description;
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            if (_description == null)
            {
                LogUtility.Error($"_description が null です。割り当てを行ってください", LogCategory.UI, this);
            }
        }

        /// <summary>
        /// 表示テキストを変更する
        /// </summary>
        public Tween SetText(string description, float duration = 0)
        {
            if (!IsVisible)
            {
                SetVisibility(true);
            }

            // 一度テキストボックスを空にする
            _description.text = string.Empty;
            return _description.DOText(description ?? string.Empty, duration).SetEase(Ease.Linear);
        }
        
        /// <summary>
        /// テキストをクリアする
        /// </summary>
        public void ClearText()
        {
            SetText(string.Empty);
        }
    }
}