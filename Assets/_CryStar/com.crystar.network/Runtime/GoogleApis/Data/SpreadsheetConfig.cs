using System;
using UnityEngine;

namespace CryStar.Network
{
    /// <summary>
    /// Google Spreadsheetとの連携に必要なシートの設定データを管理するクラス
    /// </summary>
    [Serializable]
    public class SpreadsheetConfig
    {
        [SerializeField, Tooltip("スプレッドシートの識別名\nコード内で使用する任意の文字列")]
        private string _name;

        [SerializeField, Tooltip("スプレッドシートID\n" +
                                 "スプレッドシートのURLから取得できます\n" +
                                 "URLの「/spreadsheets/d/」と「/edit」の間の文字列です")]
        private string _spreadsheetId;

        [SerializeField, Tooltip("このスプレッドシートの用途や内容の説明（任意）")] 
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
        
        /// <summary>
        /// 設定情報の文字列
        /// </summary>
        public override string ToString()
        {
            // 説明を設定していない場合は「説明なし」を出力
            var desc = string.IsNullOrWhiteSpace(_description) ? "説明なし" : _description;
            return $"SpreadsheetConfig [Name: {_name}, ID: {_spreadsheetId}, Description: {desc}]";
        }
    }
}