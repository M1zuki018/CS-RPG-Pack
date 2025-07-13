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
            string wordingText = WordingMaster.GetText(_wordingKey);
            if (wordingText != null)
            {
                m_Text = wordingText;
            }
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
