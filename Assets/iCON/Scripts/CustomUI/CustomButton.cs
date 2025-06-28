using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CustomButton
/// </summary>
[AddComponentMenu("Custom UI/Custom Button")]
public class CustomButton : Button
{
    [SerializeField] private string _assetName;
    [SerializeField] private string _wordingKey;
    private Text _text;

    protected override void Awake()
    {
        base.Awake();
        
        _text = GetComponentInChildren<Text>();

        if (_assetName != string.Empty)
        {
            // TODO: Assetまわりを整えたら修正
            SetSprite(AssetDatabase.LoadAssetAtPath<Sprite>(_assetName));
        }

        if (_wordingKey != string.Empty)
        {
            // TODO: Wordingをつくったら修正
            SetText("Wording Test");
        }
    }

    /// <summary>
    /// 画像の差し替えを行う
    /// </summary>
    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    /// <summary>
    /// テキストの変更を行う
    /// </summary>
    public void SetText(string text)
    {
        _text.text = text;
    }
}

