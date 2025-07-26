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

        if (!string.IsNullOrEmpty(_wordingKey))
        {
            SetWordingText(_wordingKey);
        }
    }

    /// <summary>
    /// テキストを設定する
    /// </summary>
    public void SetText(string text)
    {
        base.text = text;
    }

    /// <summary>
    /// 文言キーを使ってテキストを設定する
    /// </summary>
    public void SetWordingText(string wordingKey)
    {
        string wordingText = WordingMaster.GetText(wordingKey);
        if (wordingText != null)
        {
            m_Text = wordingText;
        }
    }
}
