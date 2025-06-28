using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using iCON.Constants;
using iCON.Utility;

/// <summary>
/// Googleスプレッドシートとの通信を行うクラス
/// </summary>
public class SheetsDataService : ViewBase
{
    [Header("設定")] 
    [SerializeField] private SpreadsheetConfig[] _spreadsheetConfigs;
    private SheetsService _sheetsService;
    private Dictionary<string, string> _spreadsheetIdMap = new Dictionary<string, string>();
    private bool isInitialized = false;
    
    //----------------------------------------------------------//
    /// <summary>
    /// OnAwake
    /// </summary>
    //----------------------------------------------------------//
    public override async UniTask OnAwake()
    {
        await InitializeAsync();
    }
    
    /// <summary>
    /// 初期化
    /// </summary>
    private async UniTask InitializeAsync()
    {
        try
        {
            // スプレッドシートへアクセスできるようにする
            await InitializeGoogleSheetsService();
            
            LogUtility.Info("Google Sheets API初期化完了", LogCategory.Network);
            
            RegisterSpreadsheetConfigs();
            
            await TestSpreadsheetAccess();
        }
        catch (Exception e)
        {
            LogUtility.Fatal($"Google Sheets API初期化エラー: {e.Message}", LogCategory.Network);
        }
    }
    
    /// <summary>
    /// 指定されたスプレッドシートIDを取得
    /// </summary>
    public string GetSpreadsheetId(string name)
    {
        if (_spreadsheetIdMap.TryGetValue(name, out var spreadsheetId))
        {
            return spreadsheetId;
        }
        
        Debug.LogError($"スプレッドシート '{name}' が見つかりません。利用可能な名前: {string.Join(", ", _spreadsheetIdMap.Keys)}");
        return null;
    }
    
    /// <summary>
    /// 利用可能なスプレッドシート名一覧を取得
    /// </summary>
    public string[] GetAvailableSpreadsheetNames()
    {
        var names = new string[_spreadsheetIdMap.Count];
        _spreadsheetIdMap.Keys.CopyTo(names, 0);
        return names;
    }
    
    /// <summary>
    /// Google Sheets APIサービスの初期化
    /// </summary>
    private async UniTask InitializeGoogleSheetsService()
    {
        // StreamingAssetsフォルダ内のサービスアカウントキーファイルのパスを構築
        // NOTE: サービスアカウントキーファイルは必ず「Assets/StreamingAssets」の下におく
        string keyFilePath = Path.Combine(Application.streamingAssetsPath, APIConstants.SERVICE_ACCOUNT_KEY_FILE_NAME);
        
        // サービスアカウントキーファイルの存在確認
        if (!File.Exists(keyFilePath))
        {
            throw new FileNotFoundException($"サービスアカウントキーファイルが見つかりません: {keyFilePath}");
        }
        
        // サービスアカウントキーファイルを非同期で読み込み
        string jsonContent = await File.ReadAllTextAsync(keyFilePath);
        
        // JSONからGoogleCredentialオブジェクトを作成
        GoogleCredential credential = GoogleCredential.FromJson(jsonContent);
        
        // 認証スコープを読み取り専用に設定
        credential = credential.CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
        
        // Google Sheets APIサービスを初期化
        _sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            // 認証情報を設定
            HttpClientInitializer = credential,
            ApplicationName = APIConstants.APPLICATION_NAME
        });
        
        // 初期化完了フラグを設定
        isInitialized = true;
    }
    
    /// <summary>
    /// スプレッドシート設定を辞書に登録
    /// </summary>
    private void RegisterSpreadsheetConfigs()
    {
        _spreadsheetIdMap.Clear();
        
        foreach (var config in _spreadsheetConfigs)
        {
            if (string.IsNullOrEmpty(config.Name))
            {
                Debug.LogWarning($"スプレッドシート名が設定されていません: {config.SpreadsheetId}");
                continue;
            }
            
            if (string.IsNullOrEmpty(config.SpreadsheetId))
            {
                Debug.LogWarning($"スプレッドシートID が設定されていません: {config.Name}");
                continue;
            }
            
            if (_spreadsheetIdMap.ContainsKey(config.Name))
            {
                Debug.LogWarning($"重複するスプレッドシート名: {config.Name}");
                continue;
            }
            
            _spreadsheetIdMap[config.Name] = config.SpreadsheetId;
            Debug.Log($"スプレッドシート登録: {config.Name} -> {config.SpreadsheetId}");
        }
        
        Debug.Log($"アクティブなスプレッドシート数: {_spreadsheetIdMap.Count}");
    }
    
    /// <summary>
    /// スプレッドシートアクセステスト
    /// </summary>
    private async UniTask TestSpreadsheetAccess()
    {
        var successCount = 0;
        var failureCount = 0;
        
        foreach (var kvp in _spreadsheetIdMap)
        {
            var name = kvp.Key;
            var spreadsheetId = kvp.Value;
            
            try
            {
                Debug.Log($"\n[{name}] アクセステスト開始...");
                await TestSingleSpreadsheetAccess(name, spreadsheetId);
                successCount++;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{name}] アクセステストエラー: {e.Message}");
                failureCount++;
            }
        }
        
        Debug.Log($"\n=== アクセステスト完了 ===");
        Debug.Log($"成功: {successCount}, 失敗: {failureCount}");
        
        if (failureCount > 0)
        {
            Debug.LogWarning("失敗したスプレッドシートがあります。権限設定を確認してください。");
        }
    }
    
    /// <summary>
    /// 単一スプレッドシートのアクセステスト
    /// </summary>
    private async UniTask TestSingleSpreadsheetAccess(string name, string spreadsheetId)
    {
        var request = _sheetsService.Spreadsheets.Get(spreadsheetId);
        var spreadsheet = await request.ExecuteAsync();
        
        Debug.Log($"[{name}] スプレッドシート名: {spreadsheet.Properties.Title}");
        Debug.Log($"[{name}] シート数: {spreadsheet.Sheets.Count}");
        
        foreach (var sheet in spreadsheet.Sheets)
        {
            Debug.Log($"[{name}] - シート名: '{sheet.Properties.Title}'");
        }
    }
    
    /// <summary>
    /// 指定されたスプレッドシートからデータを読み取り
    /// </summary>
    public async UniTask<IList<IList<object>>> ReadFromSpreadsheet(string spreadsheetName, string range)
    {
        var spreadsheetId = GetSpreadsheetId(spreadsheetName);
        if (string.IsNullOrEmpty(spreadsheetId))
        {
            throw new ArgumentException($"スプレッドシート '{spreadsheetName}' が見つかりません");
        }
        
        try
        {
            var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = await request.ExecuteAsync();
            
            Debug.Log($"[{spreadsheetName}] データ読み取り成功: {response.Values?.Count ?? 0} 行");
            return response.Values;
        }
        catch (Exception e)
        {
            Debug.LogError($"[{spreadsheetName}] データ読み取りエラー: {e.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// 指定されたスプレッドシートにデータを書き込み
    /// </summary>
    public async UniTask WriteToSpreadsheet(string spreadsheetName, string range, IList<IList<object>> values)
    {
        var spreadsheetId = GetSpreadsheetId(spreadsheetName);
        if (string.IsNullOrEmpty(spreadsheetId))
        {
            throw new ArgumentException($"スプレッドシート '{spreadsheetName}' が見つかりません");
        }
        
        try
        {
            var valueRange = new ValueRange
            {
                Values = values
            };
            
            var request = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            
            var response = await request.ExecuteAsync();
            Debug.Log($"[{spreadsheetName}] データ書き込み成功: {response.UpdatedCells} セル更新");
        }
        catch (Exception e)
        {
            Debug.LogError($"[{spreadsheetName}] データ書き込みエラー: {e.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// スプレッドシート設定を動的に追加
    /// </summary>
    public void AddSpreadsheetConfig(string name, string spreadsheetId, string description = "")
    {
        if (_spreadsheetIdMap.ContainsKey(name))
        {
            Debug.LogWarning($"スプレッドシート '{name}' は既に存在します");
            return;
        }
        
        _spreadsheetIdMap[name] = spreadsheetId;
        Debug.Log($"スプレッドシート追加: {name} -> {spreadsheetId}");
    }
    
    /// <summary>
    /// スプレッドシート設定を削除
    /// </summary>
    public void RemoveSpreadsheetConfig(string name)
    {
        if (_spreadsheetIdMap.Remove(name))
        {
            Debug.Log($"スプレッドシート削除: {name}");
        }
        else
        {
            Debug.LogWarning($"スプレッドシート '{name}' が見つかりません");
        }
    }
    
    /// <summary>
    /// 設定情報を表示
    /// </summary>
    [ContextMenu("設定情報を表示")]
    public void LogConfiguration()
    {
        Debug.Log("=== Google Sheets 設定情報 ===");
        Debug.Log($"登録済みスプレッドシート数: {_spreadsheetIdMap.Count}");
        
        foreach (var kvp in _spreadsheetIdMap)
        {
            Debug.Log($"- {kvp.Key}: {kvp.Value}");
        }
    }
}