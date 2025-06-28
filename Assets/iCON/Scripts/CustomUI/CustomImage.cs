using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CustomImage
/// </summary>
[AddComponentMenu("Custom UI/Custom Image")]
public class CustomImage : Image
{
    [SerializeField] private string _assetName;

    /// <summary>
    /// アセット名
    /// </summary>
    public string AssetName
    {
        get => _assetName;
        set
        {
            _assetName = value;
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>(_assetName);
        }
    }

    protected override void Awake()
    {
        if (_assetName != null)
        {
            // TODO: アセットを読み込む処理
            sprite = Resources.Load<Sprite>(_assetName);
        }
    }
}
