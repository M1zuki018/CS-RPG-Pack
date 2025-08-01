using System;
using UnityEngine;

namespace CryStar.Editor
{
    /// <summary>
    /// Hierarchy Highlighterで使用するルールデータ
    /// </summary>
    [Serializable]
    public class HighlightRule
    {
        /// <summary>
        /// 認識用のプレフィックス
        /// </summary>
        public string Prefix = "#Folder";
    
        /// <summary>
        /// 変更する背景色
        /// </summary>
        public Color BackgroundColor = new Color(0.7f, 0f, 0f, 1f);
    
        /// <summary>
        /// テキストの色
        /// </summary>
        public Color TextColor = Color.white;
    
        /// <summary>
        /// フォントスタイル
        /// </summary>
        public FontStyle FontStyle = FontStyle.Normal;
    
        /// <summary>
        /// 設定を適用するか
        /// </summary>
        public bool Enabled = true;
    }
}