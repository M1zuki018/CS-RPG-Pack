using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using iCON.Constants;
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
        private AsyncOperationHandle<AudioClip> _loadHandle;
        private bool _isLoading = false;

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
            _bgmSource = CreateAudioSource(Enums.AudioType.BGM, "BGM_Primary");
            _bgmSourceSecondary = CreateAudioSource(Enums.AudioType.BGM, "BGM_Secondary");
            
            // 環境音用のオブジェクト生成
            _ambienceSource = CreateAudioSource(Enums.AudioType.Ambience, "Ambience_Primary");
            _ambienceSourceSecondary = CreateAudioSource(Enums.AudioType.Ambience, "Ambience_Secondary");
            
            // SE用のオブジェクトプール初期化
            _seSourcePool = CreateAudioSourcePool(Enums.AudioType.SE, 3, 100);
            
            // Voice用のオブジェクトプール初期化
            _voiceSourcePool = CreateAudioSourcePool(Enums.AudioType.Voice, 3, 20);

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
        public async UniTask<Tween> CrossFadeBGM(string filePath, float fadeDuration = -1f)
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
                
                // 次のソースを準備
                NextBGMSource.gameObject.SetActive(true);
                NextBGMSource.clip = clip;
                NextBGMSource.volume = 0f;
                CurrentBGMSource.loop = true;
                NextBGMSource.Play();
                
                // クロスフェード実行
                var sequence = DOTween.Sequence();
                sequence.Append(CurrentBGMSource.DOFade(0f, fadeDuration).SetEase(KStoryPresentation.BGM_FADE_EASE));
                sequence.Join(NextBGMSource.DOFade(1f, fadeDuration).SetEase(KStoryPresentation.BGM_FADE_EASE));
                sequence.OnComplete(() => {
                    CurrentBGMSource.Stop();
                    CurrentBGMSource.gameObject.SetActive(false);
                    _isUsingSecondaryBGM = !_isUsingSecondaryBGM;
                });
                
                _bgmFadeTween = sequence;
                return sequence;
            }

            return null;
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
        public async UniTask PlaySE(string filePath)
        {
            var clip = await LoadAudioClipAsync(filePath);

            if (clip != null)
            {
                AudioSource source = _seSourcePool.Get();
                source.PlayOneShot(clip);

                await UniTask.Delay(TimeSpan.FromSeconds(clip.length));

                _seSourcePool.Release(source);
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
        private AudioSource CreateAudioSource(iCON.Enums.AudioType type, string objectName = null)
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