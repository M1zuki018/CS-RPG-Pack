using System;
using CryStar.Utility;
using CryStar.Utility.Enum;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// ParticleSystemの再生データ
/// </summary>
[Serializable]
public class ParticleData : IDisposable
{
    /// <summary>
    /// プレハブ
    /// </summary>
    [SerializeField]
    private ParticleSystem _particlePrefab;
    
    /// <summary>
    /// 生成されたParticleSystemインスタンス
    /// </summary>
    private ParticleSystem _particleInstance;
    
    /// <summary>
    /// インスタンスが生成済みかどうか
    /// </summary>
    public bool IsInstantiated => _particleInstance != null;
    
    /// <summary>
    /// Transform操作用のプロパティ
    /// </summary>
    public Transform Transform => _particleInstance?.transform;
    
    /// <summary>
    /// ParticleSystemインスタンスを生成する
    /// </summary>
    public ParticleData Instantiate(Transform parent)
    {
        if (_particlePrefab == null)
        {
            LogUtility.Error("ParticleData: プレハブが設定されていません", LogCategory.UI);
            return this;
        }
        
        if (_particleInstance != null)
        {
            LogUtility.Warning("ParticleData: 既にインスタンスが存在します", LogCategory.UI);
            return this;
        }
        
        var particleObject = Object.Instantiate(_particlePrefab, parent);
        _particleInstance = particleObject.GetComponent<ParticleSystem>();
        return this;
    }
    
    /// <summary>
    /// Play
    /// </summary>
    public void Play()
    {
        if (!ValidateInstance())
        {
            return;
        }
        _particleInstance.Play();
    }

    /// <summary>
    /// Stop
    /// </summary>
    public void Stop()
    {
        if (!ValidateInstance())
        {
            return;
        }
        _particleInstance.Stop();
    }

    /// <summary>
    /// Pause
    /// </summary>
    public void Pause()
    {
        if (!ValidateInstance())
        {
            return;
        }
        _particleInstance.Pause();
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear()
    {
        if (!ValidateInstance())
        {
            return;
        }
        _particleInstance.Clear();
    }

    /// <summary>
    /// オブジェクトのActive状態を切り替える
    /// </summary>
    public void SetActive(bool isActive)
    {
        _particleInstance.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        if (!ValidateInstance())
        {
            return;
        }
        
        _particleInstance = null;
    }
    
    #region Private Methods

    /// <summary>
    /// インスタンスの有効性をチェック
    /// </summary>
    private bool ValidateInstance()
    {
        if (_particleInstance == null)
        {
            Debug.LogWarning("ParticleData: インスタンスが生成されていません。先にInstantiate()を呼んでください");
            return false;
        }
        return true;
    }

    #endregion
}
