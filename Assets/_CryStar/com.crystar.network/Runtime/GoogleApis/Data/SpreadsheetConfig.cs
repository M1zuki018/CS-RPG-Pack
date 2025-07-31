using System;
using UnityEngine;

namespace CryStar.Network
{
    /// <summary>
    /// 取得したいスプレッドシートの設定
    /// </summary>
    [Serializable]
    public class SpreadsheetConfig
    {
        [SerializeField, Tooltip("スプレッドシートの識別名（コード内で使用）")]
        private string _name;

        [SerializeField, Tooltip("スプレッドシートID")]
        private string _spreadsheetId;

        [SerializeField, Tooltip("説明（任意）")] 
        private string _description;

        /// <summary>
        /// スプレッドシートの識別名
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// スプレッドシートID
        /// </summary>
        public string SpreadsheetId => _spreadsheetId;

        /// <summary>
        /// 説明
        /// </summary>
        public string Description => _description;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SpreadsheetConfig(string name, string spreadsheetId, string description)
        {
            _name = name;
            _spreadsheetId = spreadsheetId;
            _description = description;
        }
    }
}