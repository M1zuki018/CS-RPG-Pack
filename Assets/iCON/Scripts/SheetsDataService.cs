using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using iCON.Utility;
using Newtonsoft.Json;

/// <summary>
/// Googleスプレッドシートとの通信を行うクラス
/// </summary>
public class SheetsDataService : ViewBase
{
    [Header("設定")]
    [SerializeField] private string _serviceAccountKeyFileName = "service-account-key.json";
    [SerializeField] private string spreadsheetIdArray = "spreadsheet-id";
    
    private SheetsService _sheetsService;
    private bool isInitialized = false;
    
    /// <summary>
    /// OnAwake
    /// </summary>
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
            await InitializeGoogleSheetsService();
            LogUtility.Info("Google Sheets API初期化完了", LogCategory.Network, this);
            
            // デバッグ：設定値を確認
            Debug.Log($"設定されたスプレッドシートID: {spreadsheetIdArray}");
            
            // デバッグ：スプレッドシートにアクセスできるかテスト
            await TestSpreadsheetAccess();
        }
        catch (Exception e)
        {
            Debug.LogError($"Google Sheets API初期化エラー: {e.Message}");
        }
    }
    
    /// <summary>
    /// スプレッドシートアクセステスト
    /// </summary>
    private async Task TestSpreadsheetAccess()
    {
        try
        {
            Debug.Log("スプレッドシートアクセステスト開始...");
            var request = _sheetsService.Spreadsheets.Get(spreadsheetIdArray);
            var spreadsheet = await request.ExecuteAsync();
            
            Debug.Log($"スプレッドシート名: {spreadsheet.Properties.Title}");
            Debug.Log($"シート数: {spreadsheet.Sheets.Count}");
            
            foreach (var sheet in spreadsheet.Sheets)
            {
                Debug.Log($"- シート名: '{sheet.Properties.Title}'");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"スプレッドシートアクセステストエラー: {e.Message}");
            Debug.LogError("原因の可能性:");
            Debug.LogError("1. スプレッドシートIDが間違っている");
            Debug.LogError("2. サービスアカウントに共有権限が付与されていない");
        }
    }
    
    private async Task InitializeGoogleSheetsService()
    {
        string keyFilePath = Path.Combine(Application.streamingAssetsPath, _serviceAccountKeyFileName);
        
        if (!File.Exists(keyFilePath))
        {
            throw new FileNotFoundException($"サービスアカウントキーファイルが見つかりません: {keyFilePath}");
        }
        
        string jsonContent = await File.ReadAllTextAsync(keyFilePath);
        
        GoogleCredential credential = GoogleCredential.FromJson(jsonContent)
            .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
        
        _sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Unity Game Master Data Loader"
        });
        
        isInitialized = true;
    }
    
    /// <summary>
    /// キャラクターステータスデータを取得
    /// </summary>
    public async Task<List<CharacterStatus>> LoadCharacterStatusData(string sheetName = "CharacterStatus")
    {
        if (!isInitialized)
        {
            Debug.LogError("Google Sheets APIが初期化されていません");
            return new List<CharacterStatus>();
        }
        
        try
        {
            string range = $"{sheetName}!A2:F"; // ヘッダー行をスキップ
            SpreadsheetsResource.ValuesResource.GetRequest request = 
                _sheetsService.Spreadsheets.Values.Get(spreadsheetIdArray, range);
            
            ValueRange response = await request.ExecuteAsync();
            
            List<CharacterStatus> characterList = new List<CharacterStatus>();
            
            if (response.Values != null)
            {
                foreach (var row in response.Values)
                {
                    if (row.Count >= 6)
                    {
                        CharacterStatus character = new CharacterStatus
                        {
                            id = int.Parse(row[0].ToString()),
                            name = row[1].ToString(),
                            hp = int.Parse(row[2].ToString()),
                            attack = int.Parse(row[3].ToString()),
                            defense = int.Parse(row[4].ToString()),
                            description = row[5].ToString()
                        };
                        characterList.Add(character);
                    }
                }
            }
            
            Debug.Log($"キャラクターデータを{characterList.Count}件読み込みました");
            return characterList;
        }
        catch (Exception e)
        {
            Debug.LogError($"キャラクターデータ読み込みエラー: {e.Message}");
            return new List<CharacterStatus>();
        }
    }
    
    /// <summary>
    /// スプレッドシートの全シート名を取得（デバッグ用）
    /// </summary>
    public async Task<List<string>> GetAllSheetNames()
    {
        if (!isInitialized)
        {
            Debug.LogError("Google Sheets APIが初期化されていません");
            return new List<string>();
        }
        
        try
        {
            var request = _sheetsService.Spreadsheets.Get(spreadsheetIdArray);
            var spreadsheet = await request.ExecuteAsync();
            
            List<string> sheetNames = new List<string>();
            foreach (var sheet in spreadsheet.Sheets)
            {
                sheetNames.Add(sheet.Properties.Title);
                Debug.Log($"見つかったシート: {sheet.Properties.Title}");
            }
            
            return sheetNames;
        }
        catch (Exception e)
        {
            Debug.LogError($"シート名取得エラー: {e.Message}");
            return new List<string>();
        }
    }
    
    /// <summary>
    /// ストーリーデータを取得
    /// </summary>
    public async Task<List<StoryData>> LoadStoryData(string sheetName = "StoryData")
    {
        if (!isInitialized)
        {
            Debug.LogError("Google Sheets APIが初期化されていません");
            return new List<StoryData>();
        }
        
        try
        {
            string range = $"{sheetName}!A2:E"; // ヘッダー行をスキップ
            SpreadsheetsResource.ValuesResource.GetRequest request = 
                _sheetsService.Spreadsheets.Values.Get(spreadsheetIdArray, range);
            
            ValueRange response = await request.ExecuteAsync();
            
            List<StoryData> storyList = new List<StoryData>();
            
            if (response.Values != null)
            {
                foreach (var row in response.Values)
                {
                    if (row.Count >= 5)
                    {
                        StoryData story = new StoryData
                        {
                            chapterId = int.Parse(row[0].ToString()),
                            chapterTitle = row[1].ToString(),
                            content = row[2].ToString(),
                            characterName = row[3].ToString(),
                            backgroundImage = row[4].ToString()
                        };
                        storyList.Add(story);
                    }
                }
            }
            
            Debug.Log($"ストーリーデータを{storyList.Count}件読み込みました");
            return storyList;
        }
        catch (Exception e)
        {
            Debug.LogError($"ストーリーデータ読み込みエラー: {e.Message}");
            return new List<StoryData>();
        }
    }
    
    /// <summary>
    /// 汎用的なデータ取得メソッド
    /// </summary>
    public async Task<List<List<object>>> GetSheetData(string sheetName, string range = "A1:Z")
    {
        if (!isInitialized)
        {
            Debug.LogError("Google Sheets APIが初期化されていません");
            return new List<List<object>>();
        }
        
        try
        {
            string fullRange = $"{sheetName}!{range}";
            SpreadsheetsResource.ValuesResource.GetRequest request = 
                _sheetsService.Spreadsheets.Values.Get(spreadsheetIdArray, fullRange);
            
            ValueRange response = await request.ExecuteAsync();
            
            List<List<object>> data = new List<List<object>>();
            
            if (response.Values != null)
            {
                foreach (var row in response.Values)
                {
                    data.Add(new List<object>(row));
                }
            }
            
            Debug.Log($"{sheetName}から{data.Count}行のデータを取得しました");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"シートデータ取得エラー ({sheetName}): {e.Message}");
            return new List<List<object>>();
        }
    }
    
    /// <summary>
    /// マスタデータをローカルにキャッシュ保存
    /// </summary>
    public void CacheDataToLocal<T>(List<T> data, string fileName)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(filePath, json);
            Debug.Log($"データをキャッシュしました: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"データキャッシュエラー: {e.Message}");
        }
    }
    
    /// <summary>
    /// ローカルキャッシュからデータを読み込み
    /// </summary>
    public List<T> LoadDataFromCache<T>(string fileName)
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"キャッシュ読み込みエラー: {e.Message}");
        }
        
        return new List<T>();
    }
}