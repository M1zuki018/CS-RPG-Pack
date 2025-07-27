using CryStar.Attribute;
using Cysharp.Threading.Tasks;
using iCON.System;
using iCON.Utility;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PackSample_StoryPlayButton : ViewBase
{
    [Header("表示の設定")]
    [SerializeField, Comment("ボタンに表示するテキスト")] private string _displayText;
    
    [Header("再生するストーリーの設定")]
    [SerializeField, ExpandableSO] private StoryExecuteDataSO _storyData;
    
    private Button _button;
    private Text _childText;

    public override async UniTask OnStart()
    {
        await base.OnStart();
        
        _button = GetComponent<Button>();
        _childText = _button.GetComponentInChildren<Text>();
        
        // テキスト書き換え
        _childText.text = _displayText;
        
        // ボタンのクリックイベントに安全にストーリー再生メソッドを追加
        _button.onClick.SafeAddListener(Play);
    }

    private void OnDestroy()
    {
        _button.onClick.SafeRemoveAllListeners();
    }

    private void Play()
    {
        ServiceLocator.GetLocal<InGameManager>().PlayStory(_storyData);
    }
}
