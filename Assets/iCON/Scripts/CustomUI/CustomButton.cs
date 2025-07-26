using System;
using iCON.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CustomButton
/// </summary>
[AddComponentMenu("Custom UI/Custom Button")]
[RequireComponent(typeof(CustomImage))]
public class CustomButton : Button
{
    /// <summary>
    /// CustomImageに表示したいアセット名
    /// </summary>
    [SerializeField] 
    private string _assetName;
    
    /// <summary>
    /// 子オブジェクトのCustomTextに表示したい文言キー
    /// </summary>
    [SerializeField] 
    private string _wordingKey;

    private CustomButton _button;
    private CustomImage _image;
    private CustomText _text;

    protected override void Awake()
    {
        base.Awake();
        
        _button = GetComponent<CustomButton>();
        _image = GetComponent<CustomImage>();
        _text = GetComponentInChildren<CustomText>();
        
        if (!string.IsNullOrEmpty(_assetName))
        {
            _image.AssetName = _assetName;
        }

        if (!string.IsNullOrEmpty(_wordingKey))
        {
            _text.SetWordingText(_wordingKey);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _button.onClick.SafeRemoveAllListeners();
    }

    /// <summary>
    /// 画像の差し替えを行う
    /// </summary>
    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    /// <summary>
    /// アセットのパスを指定して画像の差し替えを行う
    /// </summary>
    public void SetSprite(string assetName)
    {
        _image.AssetName = assetName;
    }

    /// <summary>
    /// テキストの変更を行う
    /// </summary>
    public void SetText(string text)
    {
        _text.text = text;
    }

    /// <summary>
    /// クリックアクションを登録する
    /// </summary>
    public void SetClickAction(Action onClick)
    {
        _button.onClick.SafeAddListener(() => onClick?.Invoke());
    }
}

