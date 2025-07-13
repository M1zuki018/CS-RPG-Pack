using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MasterDataManager : ViewBase
{
    [Header("参照")]
    [SerializeField] private SheetsDataService _sheetsDataService;
    
    // マスタデータの保持
    public List<CharacterStatus> CharacterStatusList { get; private set; }
    public List<StoryData> StoryDataList { get; private set; }
    
    // シングルトンパターン
    public static MasterDataManager Instance { get; private set; }
    
    public override async UniTask OnAwake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        await LoadAllMasterData();
    }
    
    /// <summary>
    /// デバッグ用：スプレッドシートの情報を確認
    /// </summary>
    [ContextMenu("Debug Spreadsheet Info")]
    public async void DebugSpreadsheetInfo()
    {
        // Debug.Log("=== スプレッドシート情報確認開始 ===");
        //
        // // 全シート名を取得
        // var sheetNames = await _sheetsDataService.GetAllSheetNames();
        // Debug.Log($"利用可能なシート数: {sheetNames.Count}");
        //
        // // 最初のシートのデータを汎用メソッドで取得してみる
        // if (sheetNames.Count > 0)
        // {
        //     string firstSheetName = sheetNames[0];
        //     Debug.Log($"最初のシート '{firstSheetName}' のデータを取得中...");
        //     
        //     var data = await _sheetsDataService.GetSheetData(firstSheetName, "A1:Z10");
        //     Debug.Log($"取得したデータ行数: {data.Count}");
        //     
        //     // 最初の数行を表示
        //     for (int i = 0; i < Mathf.Min(3, data.Count); i++)
        //     {
        //         string rowData = string.Join(" | ", data[i]);
        //         Debug.Log($"行{i + 1}: {rowData}");
        //     }
        // }
        //
        // Debug.Log("=== スプレッドシート情報確認終了 ===");
    }
    
    /// <summary>
    /// 全マスタデータを読み込み
    /// </summary>
    public async Task LoadAllMasterData()
    {
        // Debug.Log("マスタデータ読み込み開始...");
        //
        // // まずキャッシュから読み込みを試行
        // CharacterStatusList = _sheetsDataService.LoadDataFromCache<CharacterStatus>("character_status_cache.json");
        // StoryDataList = _sheetsDataService.LoadDataFromCache<StoryData>("story_data_cache.json");
        //
        // // オンラインでデータを更新
        // try
        // {
        //     var characterTask = _sheetsDataService.LoadCharacterStatusData();
        //     var storyTask = _sheetsDataService.LoadStoryData();
        //     
        //     await UniTask.WhenAll(characterTask, storyTask);
        //     
        //     CharacterStatusList = await characterTask;
        //     StoryDataList = await storyTask;
        //     
        //     // 取得したデータをキャッシュに保存
        //     _sheetsDataService.CacheDataToLocal(CharacterStatusList, "character_status_cache.json");
        //     _sheetsDataService.CacheDataToLocal(StoryDataList, "story_data_cache.json");
        //     
        //     Debug.Log("マスタデータ読み込み完了");
        // }
        // catch (System.Exception e)
        // {
        //     Debug.LogWarning($"オンラインデータ取得失敗、キャッシュを使用: {e.Message}");
        // }
    }
    
    /// <summary>
    /// キャラクターステータスをIDで取得
    /// </summary>
    public CharacterStatus GetCharacterById(int id)
    {
        return CharacterStatusList?.Find(c => c.id == id);
    }
    
    /// <summary>
    /// ストーリーデータをチャプターIDで取得
    /// </summary>
    public List<StoryData> GetStoryByChapter(int chapterId)
    {
        return StoryDataList?.FindAll(s => s.chapterId == chapterId);
    }
    
    /// <summary>
    /// 手動でマスタデータを再読み込み
    /// </summary>
    [ContextMenu("Reload Master Data")]
    public async void ReloadMasterData()
    {
        await LoadAllMasterData();
    }
}