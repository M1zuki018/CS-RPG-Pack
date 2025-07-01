using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Addressableを利用する際のアセットローダー
/// </summary>
public class AddressableLoader : MonoBehaviour
{
    public static AddressableLoader Instance;
    
    private AsyncOperationHandle<Sprite> _loadHandle;
    private bool _isLoading;
    
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// スプライトを非同期で読み込む
    /// </summary>
    public async UniTask<Sprite> LoadSpriteAsync(string address)
    {
        if (_isLoading || string.IsNullOrEmpty(address))
            return null;

        _isLoading = true;

        try
        {
            // 既存のハンドルがあれば解放
            if (_loadHandle.IsValid())
            {
                Addressables.Release(_loadHandle);
            }

            // 新しいアセットを読み込み
            _loadHandle = Addressables.LoadAssetAsync<Sprite>(address);
            var loadedSprite = await _loadHandle.ToUniTask();
            
            return loadedSprite;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load sprite: {address}, Error: {e.Message}");
        }
        finally
        {
            _isLoading = false;
        }
        
        return null;
    }
    
    /// <summary>
    /// オブジェクト破棄時にAddressableハンドルを解放
    /// </summary>
    private void OnDestroy()
    {
        if (_loadHandle.IsValid())
        {
            Addressables.Release(_loadHandle);
        }
    }
}
