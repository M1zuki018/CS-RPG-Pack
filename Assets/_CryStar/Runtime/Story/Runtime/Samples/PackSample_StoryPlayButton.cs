using CryStar.Attribute;
using CryStar.Core;
using CryStar.Utility;
using Cysharp.Threading.Tasks;
using iCON.System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PackSample_StoryPlayButton : CustomBehaviour
{
    [Header("表示の設定")]
    [SerializeField, Comment("ボタンに表示するテキスト")] private string _displayText;
    
    [Header("再生するストーリーのID")]
    [SerializeField] private int _storyId;
    
    private Button _button;
    private Text _childText;

    public override async UniTask OnUIInitialize()
    {
        await base.OnUIInitialize();
        
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
        ServiceLocator.GetLocal<InGameManager>().PlayStory(_storyId);
    }
}
