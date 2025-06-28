using System;
using UnityEngine;

/// <summary>
/// 取得したいスプレッドシートの設定
/// </summary>
[Serializable]
public class SpreadsheetConfig
{
    [SerializeField, Comment("スプレッドシートの識別名（コード内で使用）")]
    private string _name;
        
    [SerializeField, Comment("スプレッドシートID")]
    private string _spreadsheetId;
        
    [SerializeField, Comment("説明（任意）")]
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
}