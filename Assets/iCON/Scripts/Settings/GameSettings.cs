using System;
using UnityEngine;

/// <summary>
/// プレイヤーのゲーム設定
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettingsSO")]
public class GameSettings : ScriptableObject
{
    [Header("グラフィック設定")] 
    [SerializeField] private float _graphicsQuality = 1;

    [Header("サウンド設定")] 
    [SerializeField, Range(0, 1)] private float _masterVolume = 1.0f;
    [SerializeField, Range(0, 1)] private float _bgmVolume = 1.0f;
    [SerializeField, Range(0, 1)] private float _seVolume = 1.0f;
    [SerializeField, Range(0, 1)] private float _ambientVolume = 1.0f;
    [SerializeField, Range(0, 1)] private float _voiceVolume = 1.0f;

    [Header("環境設定")] 
    [SerializeField] private LanguageType _textLanguage = LanguageType.Japanese;
    [SerializeField] private LanguageType _voiceLanguage = LanguageType.Japanese;
    [SerializeField] private ScenarioSpeedType _scenarioSpeed = ScenarioSpeedType.Default;
    [SerializeField] private bool _useAuto = true;
    
    public event Action<LanguageType> OnTextLanguageChanged;
    
    /// <summary>
    /// 値の取得
    /// </summary>
    private T GetPrefsValue<T>(string key, T defaultValue)
    {
        if (typeof(T) == typeof(float))
            return (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
        if (typeof(T) == typeof(int))
            return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
        if (typeof(T) == typeof(bool))
            return (T)(object)(PlayerPrefs.GetInt(key, Convert.ToInt32((bool)(object)defaultValue)) != 0);
        if(typeof(T) == typeof(string))
            return (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue);
        if (typeof(T).IsEnum)
            return (T)Enum.ToObject(typeof(T), PlayerPrefs.GetInt(key, Convert.ToInt32(defaultValue)));

        Debug.LogError($"Unsupported type: {typeof(T)}");
        return defaultValue;
    }

    /// <summary>
    /// 設定
    /// </summary>
    private void SetPrefsValue<T>(string key, T value)
    {
        if (typeof(T) == typeof(float))
            PlayerPrefs.SetFloat(key, (float)(object)value);
        else if (typeof(T) == typeof(int))
            PlayerPrefs.SetInt(key, (int)(object)value);
        else if (typeof(T) == typeof(bool))
            PlayerPrefs.SetInt(key, Convert.ToInt32((bool)(object)value));
        else if(typeof(T) == typeof(string))
            PlayerPrefs.SetString(key, (string)(object)value);
        else if (typeof(T).IsEnum)
            PlayerPrefs.SetInt(key, Convert.ToInt32(value));
        else
            Debug.LogError($"Unsupported type: {typeof(T)}");

        PlayerPrefs.Save();
    }

    #region グラフィック設定

    /// <summary>
    /// グラフィック品質
    /// </summary>
    public float GraphicsQuality
    {
        get => GetPrefsValue(nameof(GraphicsQuality), _graphicsQuality);
        set => SetPrefsValue(nameof(GraphicsQuality), value);
    }

    #endregion

    #region サウンド設定

    /// <summary>
    /// 全体の音量
    /// </summary>
    public float MasterVolume
    {
        get => GetPrefsValue(nameof(MasterVolume), _masterVolume);
        set => SetPrefsValue(nameof(MasterVolume), value);
    }

    /// <summary>
    /// BGMの音量
    /// </summary>
    public float BGMVolume
    {
        get => GetPrefsValue(nameof(BGMVolume), _bgmVolume);
        set => SetPrefsValue(nameof(BGMVolume), value);
    }

    /// <summary>
    /// SEの音量
    /// </summary>
    public float SEVolume
    {
        get => GetPrefsValue(nameof(SEVolume), _seVolume);
        set => SetPrefsValue(nameof(SEVolume), value);
    }

    /// <summary>
    /// 環境音の音量
    /// </summary>
    public float AmbientVolume
    {
        get => GetPrefsValue(nameof(AmbientVolume), _ambientVolume);
        set => SetPrefsValue(nameof(AmbientVolume), value);
    }

    /// <summary>
    /// ボイスの音量
    /// </summary>
    public float VoiceVolume
    {
        get => GetPrefsValue(nameof(VoiceVolume), _voiceVolume);
        set => SetPrefsValue(nameof(VoiceVolume), value);
    }

    #endregion

    #region 環境設定

    /// <summary>
    /// テキスト言語
    /// </summary>
    public LanguageType TextLanguage
    {
        get => GetPrefsValue(nameof(TextLanguage), _textLanguage);
        set
        {
            SetPrefsValue(nameof(TextLanguage), value);
            OnTextLanguageChanged?.Invoke(value);
        }
    }

    /// <summary>
    /// ボイス言語
    /// </summary>
    public LanguageType VoiceLanguage
    {
        get => GetPrefsValue(nameof(VoiceLanguage), _voiceLanguage);
        set => SetPrefsValue(nameof(VoiceLanguage), value);
    }

    /// <summary>
    /// シナリオ表示速度
    /// </summary>
    public ScenarioSpeedType ScenarioSpeed
    {
        get => GetPrefsValue(nameof(ScenarioSpeed), _scenarioSpeed);
        set => SetPrefsValue(nameof(ScenarioSpeed), value);
    }

    /// <summary>
    /// 自動再生の使用
    /// </summary>
    public bool UseAuto
    {
        get => GetPrefsValue(nameof(UseAuto), _useAuto);
        set => SetPrefsValue(nameof(UseAuto), value);
    }

    #endregion

    #region プレイヤー設定

    /// <summary>
    /// レベルを設定する
    /// </summary>
    public void GetLevel()
    {
        int level = GetPrefsValue("Level", 1); // デフォルト値を1に設定
        PlayerData.SetLevel(level);
    }

    /// <summary>
    /// レベルをセットする
    /// </summary>
    public void SetLevel() => SetPrefsValue("Level", PlayerData.LevelProp.Value);

    /// <summary>
    /// 名前を設定する
    /// </summary>
    public void GetName()
    {
        string name = GetPrefsValue("Name", "Default");
        PlayerData.SetName(name);
    }

    /// <summary>
    /// レベルをセットする
    /// </summary>
    public void SetName() => SetPrefsValue("Name", PlayerData.NameProp.Value);

    #endregion
    
    /// <summary>
    /// 全ての設定を初期値にリセット
    /// </summary>
    public void ResetToDefaults()
    {
        GraphicsQuality = _graphicsQuality;
        MasterVolume = _masterVolume;
        BGMVolume = _bgmVolume;
        SEVolume = _seVolume;
        AmbientVolume = _ambientVolume;
        VoiceVolume = _voiceVolume;
        TextLanguage = _textLanguage;
        VoiceLanguage = _voiceLanguage;
        ScenarioSpeed = _scenarioSpeed;
        UseAuto = _useAuto;
        PlayerData.LevelReset();
    }
}