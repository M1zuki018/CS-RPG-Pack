using System;
using Cysharp.Threading.Tasks;
using iCON.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace iCON.System
{

    /// <summary>
    /// Audioを管理するManagerクラス
    /// </summary>
    public class AudioManager : ViewBase
    {
        /// <summary>
        /// シングルトン
        /// </summary>
        public static AudioManager Instance { get; private set; }

        [Header("AudioMixer")] 
        [SerializeField, HighlightIfNull]
        private AudioMixer _mixer;

        [SerializeField, HighlightIfNull] 
        private GameSettings _gameSettings;

        [Header("AudioSource")] 
        private AudioSource _bgmSource; // BGM用
        private AudioSource _ambienceSource; // 環境音用
        private IObjectPool<AudioSource> _seSourcePool; // SE用のAudioSource
        private IObjectPool<AudioSource> _voiceSourcePool; // Voice用のAudioSource

        /// <summary>
        /// Addressable
        /// </summary>
        private AsyncOperationHandle<AudioClip> _loadHandle;
        private bool _isLoading = false;

        /// <summary>
        /// SEのオブジェクトプールからAudioSourceを取得する
        /// </summary>
        public AudioSource GetSEAudioSource() => _seSourcePool.Get();

        /// <summary>
        /// VoiceのオブジェクトプールからAudioSourceを取得する
        /// </summary>
        public AudioSource GetVoiceAudioSource() => _voiceSourcePool.Get();

        /// <summary>
        /// SEのオブジェクトプールから引数で渡したAudioSourceを解除する
        /// </summary>
        public void SESourceRelease(AudioSource source) => _seSourcePool.Release(source);

        /// <summary>
        /// Voiceのオブジェクトプールから引数で渡したAudioSourceを解除する
        /// </summary>
        public void VoiceSourceRelease(AudioSource source) => _voiceSourcePool.Release(source);

        #region Lifecycle
        
        public override UniTask OnAwake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return base.OnAwake();
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _bgmSource = CreateAudioSource(Enums.AudioType.BGM);
            _ambienceSource = CreateAudioSource(Enums.AudioType.Ambience);
            _seSourcePool = CreateAudioSourcePool(Enums.AudioType.SE, 3, 100); // SE用のオブジェクトプール初期化
            _voiceSourcePool = CreateAudioSourcePool(Enums.AudioType.Voice, 3, 20); // Voice用のオブジェクトプール初期化

            for (int i = 0; i < 3; i++)
            {
                _seSourcePool.Get();
                _voiceSourcePool.Get();
            }

            SetVolume("Master", _gameSettings.MasterVolume);
            SetVolume("BGM", _gameSettings.BGMVolume);
            SetVolume("SE", _gameSettings.SEVolume);
            SetVolume("Ambience", _gameSettings.AmbientVolume);
            SetVolume("Voice", _gameSettings.VoiceVolume);

            return base.OnAwake();
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
        
        #endregion

        /// <summary>
        /// BGMを再生する
        /// </summary>
        public async UniTask PlayBGM(string filePath)
        {
            var clip = await LoadAudioClipAsync(filePath);

            // NOTE: nullの可能性があるのでnullチェックを行う
            if (clip != null)
            {
                _bgmSource.gameObject.SetActive(true);
                _bgmSource.clip = clip;
                _bgmSource.Play();
            }
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        public async UniTask PlaySE(string filePath)
        {
            var clip = await LoadAudioClipAsync(filePath);

            if (clip != null)
            {
                AudioSource source = _seSourcePool.Get();
                source.PlayOneShot(clip);

                await UniTask.WaitForSeconds(clip.length);

                _seSourcePool.Release(source);
            }
        }

        /// <summary>
        /// 環境音を再生する
        /// </summary>
        public async UniTask PlayAmbience(string filePath)
        {
            var clip = await LoadAudioClipAsync(filePath);
            if (clip != null)
            {
                _ambienceSource.clip = clip;
                _ambienceSource.Play();
            }
        }

        /// <summary>
        /// ボイスを再生する
        /// </summary>
        public async UniTask PlayVoice(string filePath)
        {
            var clip = await LoadAudioClipAsync(filePath);
            if (clip != null)
            {
                AudioSource source = _voiceSourcePool.Get();
                source.PlayOneShot(clip);

                await UniTask.WaitForSeconds(clip.length);

                _voiceSourcePool.Release(source);
            }
        }

        #region Private Methods

        /// <summary>
        /// AudioMixerの音量を調整する
        /// </summary>
        private void SetVolume(string type, float volume)
        {
            float volumeInDb = volume > 0 ? Mathf.Log10(volume) * 20 : -80;
            _mixer.SetFloat($"{type}Volume", volumeInDb);

            // TODO: Debug用
            _mixer.GetFloat($"{type}Volume", out volume);
            Debug.Log($"{type}Volume: {volume}");
        }

        /// <summary>
        /// AudioSourceのオブジェクトプールを作成
        /// </summary>
        private IObjectPool<AudioSource> CreateAudioSourcePool(iCON.Enums.AudioType type, int defaultCapacity, int maxSize)
        {
            return new ObjectPool<AudioSource>(
                createFunc: () => CreateAudioSource(type),
                actionOnGet: source => source.gameObject.SetActive(true),
                actionOnRelease: source => source.gameObject.SetActive(false),
                actionOnDestroy: Destroy,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        /// <summary>
        /// 新しくGameObjectとAudioSourceを生成する
        /// </summary>
        private AudioSource CreateAudioSource(iCON.Enums.AudioType type)
        {
            GameObject obj = new GameObject(type.ToString());
            obj.transform.SetParent(transform);
            AudioSource source = obj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = _mixer.FindMatchingGroups(type.ToString())[0];
            obj.SetActive(false);
            return source;
        }

        /// <summary>
        /// AudioClipを非同期で読み込む
        /// </summary>
        private async UniTask<AudioClip> LoadAudioClipAsync(string filePath)
        {
            if (_isLoading || string.IsNullOrEmpty(filePath))
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
                _loadHandle = Addressables.LoadAssetAsync<AudioClip>(filePath);
                return await _loadHandle.ToUniTask();
            }
            catch (Exception e)
            {
                LogUtility.Error($"AudioClipのロードに失敗しました: {filePath}, Error: {e.Message}", LogCategory.Audio);
            }
            finally
            {
                _isLoading = false;
            }

            return null;
        }

        #endregion
    }
}