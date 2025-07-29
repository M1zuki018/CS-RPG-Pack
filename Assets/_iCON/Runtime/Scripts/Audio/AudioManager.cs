using System;
using System.Collections.Generic;
using CryStar.Attribute;
using CryStar.Story.Constants;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Extensions;
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

        /// <summary>
        /// AudioMixer
        /// </summary>
        [SerializeField, HighlightIfNull]
        private AudioMixer _mixer;

        /// <summary>
        /// ゲーム設定（AudioMixerの調節用）
        /// </summary>
        [SerializeField, HighlightIfNull] 
        private GameSettings _gameSettings;

        // NOTE: クロスフェード用に2つ用意する
        private AudioSource _bgmSource; // BGM用
        private AudioSource _bgmSourceSecondary; // クロスフェード用セカンダリBGM
        private AudioSource _ambienceSource; // 環境音用
        private AudioSource _ambienceSourceSecondary; // クロスフェード用セカンダリ環境音

        // NOTE: 複数同時に音が鳴るものはObjectPoolで管理
        private IObjectPool<AudioSource> _seSourcePool; // SE用のAudioSource
        private IObjectPool<AudioSource> _voiceSourcePool; // Voice用のAudioSource

        // NOTE: フェード管理用
        private Tween _bgmFadeTween;
        private Tween _ambienceFadeTween;
        private bool _isUsingSecondaryBGM = false;
        private bool _isUsingSecondaryAmbience = false;
        
        /// <summary>
        /// Addressable
        /// </summary>
        private Dictionary<string, AudioClip> _audioClipCache = new Dictionary<string, AudioClip>();
        private Dictionary<string, AsyncOperationHandle<AudioClip>> _loadHandles = new Dictionary<string, AsyncOperationHandle<AudioClip>>();

        #region Properties

        /// <summary>
        /// 現在のBGMソース
        /// </summary>
        private AudioSource CurrentBGMSource => _isUsingSecondaryBGM ? _bgmSourceSecondary : _bgmSource;
        
        /// <summary>
        /// 次のBGMソース（クロスフェード用）
        /// </summary>
        private AudioSource NextBGMSource => _isUsingSecondaryBGM ? _bgmSource : _bgmSourceSecondary;

        /// <summary>
        /// 現在の環境音ソース
        /// </summary>
        private AudioSource CurrentAmbienceSource => _isUsingSecondaryAmbience ? _ambienceSourceSecondary : _ambienceSource;
        
        /// <summary>
        /// 次の環境音ソース（クロスフェード用）
        /// </summary>
        private AudioSource NextAmbienceSource => _isUsingSecondaryAmbience ? _ambienceSource : _ambienceSourceSecondary;
        
        /// <summary>
        /// SEのオブジェクトプールからAudioSourceを取得する
        /// </summary>
        public AudioSource GetSEAudioSource() => _seSourcePool.Get();
        
        /// <summary>
        /// VoiceのオブジェクトプールからAudioSourceを取得する
        /// </summary>
        public AudioSource GetVoiceAudioSource() => _voiceSourcePool.Get();

        #endregion
        
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

            // BGM用のオブジェクト生成
            _bgmSource = CreateAudioSource(CryStar.Enums.AudioType.BGM, "BGM_Primary");
            _bgmSourceSecondary = CreateAudioSource(CryStar.Enums.AudioType.BGM, "BGM_Secondary");
            
            // 環境音用のオブジェクト生成
            _ambienceSource = CreateAudioSource(CryStar.Enums.AudioType.Ambience, "Ambience_Primary");
            _ambienceSourceSecondary = CreateAudioSource(CryStar.Enums.AudioType.Ambience, "Ambience_Secondary");
            
            // SE用のオブジェクトプール初期化
            _seSourcePool = CreateAudioSourcePool(CryStar.Enums.AudioType.SE, 3, 100);
            
            // Voice用のオブジェクトプール初期化
            _voiceSourcePool = CreateAudioSourcePool(CryStar.Enums.AudioType.Voice, 3, 20);

            // 初期状態はボリューム0に設定
            _bgmSource.volume = 0f;
            _bgmSourceSecondary.volume = 0f;
            _ambienceSource.volume = 0f;
            _ambienceSourceSecondary.volume = 0f;
            
            for (int i = 0; i < 3; i++)
            {
                _seSourcePool.Get();
                _voiceSourcePool.Get();
            }

            // GameSettingsを元にAudioMixerの音量を設定
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
            foreach (var loadHandle in _loadHandles.Values)
            {
                if (loadHandle.IsValid())
                {
                    Addressables.Release(loadHandle);
                }
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
                CurrentBGMSource.gameObject.SetActive(true);
                CurrentBGMSource.clip = clip;
                CurrentBGMSource.volume = 1f;
                CurrentBGMSource.loop = true;
                CurrentBGMSource.Play();
            }
        }
        
        /// <summary>
        /// BGMをフェードインで再生する
        /// </summary>
        public async UniTask PlayBGMWithFadeIn(string filePath, float fadeDuration = -1f)
        {
            if (fadeDuration < 0)
            {
                // フェード時間が特に設定されておらず負の値の場合は、Constantで宣言している値を使用する
                fadeDuration = KStoryPresentation.BGM_FADE_DURATION;
            }
            
            var clip = await LoadAudioClipAsync(filePath);
            if (clip != null)
            {
                _bgmFadeTween?.Kill();
                
                CurrentBGMSource.gameObject.SetActive(true);
                CurrentBGMSource.clip = clip;
                CurrentBGMSource.volume = 0f;
                CurrentBGMSource.loop = true;
                CurrentBGMSource.Play();
                
                _bgmFadeTween = CurrentBGMSource.DOFade(1f, fadeDuration).SetEase(KStoryPresentation.BGM_FADE_EASE);
                await _bgmFadeTween.ToUniTask();
            }
        }
        
        /// <summary>
        /// BGMをフェードアウトする
        /// </summary>
        public async UniTask FadeOutBGM(float fadeDuration = -1f, bool stopAfterFade = true)
        {
            if (fadeDuration < 0)
            {
                // フェード時間が特に設定されておらず負の値の場合は、Constantで宣言している値を使用する
                fadeDuration = KStoryPresentation.BGM_FADE_DURATION;
            }
            
            _bgmFadeTween?.Kill();
            
            _bgmFadeTween = CurrentBGMSource.DOFade(0f, fadeDuration).SetEase(KStoryPresentation.BGM_FADE_EASE);
            await _bgmFadeTween.ToUniTask();
            
            if (stopAfterFade)
            {
                CurrentBGMSource.Stop();
                CurrentBGMSource.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// BGMをクロスフェードで切り替える
        /// </summary>
        public async UniTask CrossFadeBGM(string filePath, float fadeDuration = -1f)
        {
            if (fadeDuration < 0)
            {
                // フェード時間が特に設定されておらず負の値の場合は、Constantで宣言している値を使用する
                fadeDuration = KStoryPresentation.BGM_FADE_DURATION;
            }

            var clip = await LoadAudioClipAsync(filePath);
        
            _bgmFadeTween?.Kill();
        
            // クロスフェード処理
            NextBGMSource.gameObject.SetActive(true);
            NextBGMSource.clip = clip;
            NextBGMSource.volume = 0f;
            CurrentBGMSource.loop = true;
            NextBGMSource.Play();
        
            // フェードシーケンス
            var fadeSequence = DOTween.Sequence();
            fadeSequence.Append(CurrentBGMSource.DOFade(0f, fadeDuration).SetEase(KStoryPresentation.BGM_FADE_EASE));
            fadeSequence.Join(NextBGMSource.DOFade(1f, fadeDuration).SetEase(KStoryPresentation.BGM_FADE_EASE));
            fadeSequence.OnComplete(() =>
            {
                CurrentBGMSource.Stop();
                CurrentBGMSource.gameObject.SetActive(false);
                _isUsingSecondaryBGM = !_isUsingSecondaryBGM;
            });
        
            _bgmFadeTween = fadeSequence;
        }

        /// <summary>
        /// BGMの音量を直接設定する
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            CurrentBGMSource.volume = volume;
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        public async UniTask PlaySE(string filePath, float volume)
        {
            var clip = await LoadAudioClipAsync(filePath);

            if (clip != null)
            {
                AudioSource source = _seSourcePool.Get();
                source.volume = volume;
                source.PlayOneShot(clip);

                // クリップ再生完了を待たずに即座に返す。リソース管理は非同期で行う
                ReleaseAudioSourceAfterPlay(source, clip.length).Forget();
            }
        }

        /// <summary>
        /// SEのオブジェクトプールから引数で渡したAudioSourceを解除する
        /// </summary>
        public void SESourceRelease(AudioSource source) => _seSourcePool.Release(source);
        
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

                await UniTask.Delay(TimeSpan.FromSeconds(clip.length));

                _voiceSourcePool.Release(source);
            }
        }

        /// <summary>
        /// Voiceのオブジェクトプールから引数で渡したAudioSourceを解除する
        /// </summary>
        public void VoiceSourceRelease(AudioSource source) => _voiceSourcePool.Release(source);

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
        private IObjectPool<AudioSource> CreateAudioSourcePool(CryStar.Enums.AudioType type, int defaultCapacity, int maxSize)
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
        private AudioSource CreateAudioSource(CryStar.Enums.AudioType type, string objectName = null)
        {
            // 新規ゲームオブジェクトを生成
            GameObject obj = new GameObject(objectName ?? type.ToString());
            obj.transform.SetParent(transform);
            
            // AudioSourceコンポーネントを追加
            AudioSource source = obj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = _mixer.FindMatchingGroups(type.ToString())[0];
            source.loop = true;
            obj.SetActive(false);
            return source;
        }

        /// <summary>
        /// AudioClipを非同期で読み込む
        /// </summary>
        private async UniTask<AudioClip> LoadAudioClipAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            // キャッシュから取得を試行
            if (_audioClipCache.TryGetValue(filePath, out var cachedClip))
            {
                return cachedClip;
            }

            // 既に読み込み中の場合は待機
            if (_loadHandles.TryGetValue(filePath, out var existingHandle))
            {
                try
                {
                    return await existingHandle.ToUniTask();
                }
                catch (Exception e)
                {
                    LogUtility.Error($"AudioClipのロードに失敗しました: {filePath}, Error: {e.Message}", LogCategory.Audio);
                    _loadHandles.Remove(filePath);
                    return null;
                }
            }

            try
            {
                // 新しいアセットを読み込み
                var handle = Addressables.LoadAssetAsync<AudioClip>(filePath);
                _loadHandles[filePath] = handle;
        
                var clip = await handle.ToUniTask();
        
                // キャッシュに保存
                if (clip != null)
                {
                    _audioClipCache[filePath] = clip;
                }
        
                return clip;
            }
            catch (Exception e)
            {
                LogUtility.Error($"AudioClipのロードに失敗しました: {filePath}, Error: {e.Message}", LogCategory.Audio);
                _loadHandles.Remove(filePath);
                return null;
            }
        }
        
        /// <summary>
        /// 指定時間後にAudioSourceをプールに返却する
        /// </summary>
        private async UniTask ReleaseAudioSourceAfterPlay(AudioSource source, float duration)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            _seSourcePool.Release(source);
        }

        #endregion
    }
}