using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CustomText
/// </summary>
[AddComponentMenu("Custom UI/Custom Text")]
public class CustomText : Text
{
    [SerializeField] private string _wordingKey;

    protected override void Awake()
    {
        base.Awake();

        if (_wordingKey != string.Empty)
        {
            // TODO: Wordingから定型文をセットする処理
        }
    }

    /// <summary>
    /// テキストを設定する
    /// </summary>
    public void SetText(string text)
    {
        base.text = text;
    }
}
