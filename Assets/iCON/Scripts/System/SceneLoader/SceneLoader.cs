using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using iCON.Constants;
using iCON.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace iCON.System
{
    /// <summary>
    /// SceneLoader
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        /// <summary>
        /// タイムアウトとするまでの時間
        /// </summary>
        [SerializeField]
        private float _loadingTimeout = 30f;
        
        /// <summary>
        /// 遷移状態
        /// </summary>
        private LoadingStateType _loadingState = LoadingStateType.None;
        public LoadingStateType LoadingState => _loadingState;
        
        /// <summary>
        /// 現在のシーン
        /// </summary>
        private SceneType _currentScene;
        public SceneType CurrentScene => _currentScene;
        
        /// <summary>
        /// ロード操作をキャンセルするためのトークン
        /// </summary>
        private CancellationTokenSource _cts;
        
        /// <summary>
        /// 初期化済みか
        /// </summary>
        private bool _initialized = false;
        
        /// <summary>
        /// ロード中か
        /// </summary>
        public bool IsLoading => _loadingState == LoadingStateType.Loading;

        private void Awake()
        {
            if (!_initialized)
            {
                // 初期化が済んでいなければグローバルサービスとしてサービスロケーターに登録
                ServiceLocator.Resister(this);
                _initialized = true;
            }
            
            _currentScene = GetCurrentSceneType();
        }

        /// <summary>
        /// 現在のロード操作をキャンセル
        /// </summary>
        public void CancelCurrentLoad()
        {
            _cts?.Cancel();
            _loadingState = LoadingStateType.None;
        }
        
        /// <summary>
        /// 画面遷移を行う
        /// </summary>
        public async UniTask<bool> LoadSceneAsync(SceneTransitionData data)
        {
            // シーン読み込み中か確認
            if (IsLoading)
            {
                LogUtility.Error("現在シーン読み込み中です。リクエストを実行しません", LogCategory.System);
                return false;
            }
            
            // 念のため前回のロード操作をキャンセル
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            try
            {
                // 状態をロード中に変更
                _loadingState = LoadingStateType.Loading;

                // 遷移処理を実行
                bool success = await ExecuteSceneTransition(data, _cts.Token);

                if (success)
                {
                    // 遷移成功時は現在のシーン情報とロード状態を更新する
                    _currentScene = data.TargetScene;
                    _loadingState = LoadingStateType.Completed;
                    LogUtility.Info($"シーン遷移が完了しました: {data.TargetScene}", LogCategory.System);
                }
                else
                {
                    // 遷移失敗時はロード状態を失敗に変更する
                    _loadingState = LoadingStateType.Failed;
                    LogUtility.Error($"シーン遷移に失敗しました: {data.TargetScene}", LogCategory.System);
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                LogUtility.Info($"シーン遷移がキャンセルされました: {data.TargetScene}");
                _loadingState = LoadingStateType.None;
                return false;
            }
            catch (Exception e)
            {
                LogUtility.Error($"シーン遷移エラー: {e.Message}", LogCategory.System);
                _loadingState = LoadingStateType.Failed;
                return false;
            }
        }
        
        /// <summary>
        /// シーン遷移の実行
        /// </summary>
        private async UniTask<bool> ExecuteSceneTransition(SceneTransitionData data, CancellationToken token)
        {
            try
            {
                // タイムアウト設定
                var timeoutTask = UniTask.Delay(delayTimeSpan:TimeSpan.FromSeconds(_loadingTimeout), cancellationToken: token);
                
                Scene loadingScene = default;
                Scene currentScene = SceneManager.GetActiveScene();
                
                // ローディングスクリーンの表示
                if (data.UseLoadingScreen)
                {
                    await SceneManager.LoadSceneAsync(SceneType.Load.ToString(), LoadSceneMode.Additive);
                    loadingScene = SceneManager.GetSceneByName(SceneType.Load.ToString());
                    
                    // カメラを有効にするためにローディングシーンをアクティブに設定
                    SceneManager.SetActiveScene(loadingScene);
                    
                    // 少し待機してローディング画面が確実に表示されるようにする
                    await UniTask.Delay(100, cancellationToken: token);
                }

                // アセットのプリロードを行うか確認
                if (data.PreloadAssets)
                {
                    await UniTask.Delay(SceneConstants.PRELOAD_TIME, cancellationToken: token);
                }

                // シーンロード
                var loadTask = LoadSceneInternal(data.TargetScene, token);
                
                // ロードが早く終わるかタイムアウトが早いか判定
                var completedTask = await UniTask.WhenAny(loadTask, timeoutTask);
                if (completedTask == 1)
                {
                    // タイムアウトした場合
                    throw new TimeoutException($"シーン遷移 タイムアウト: {data.TargetScene}");
                }
                
                // 新しいシーンを取得してアクティブに設定
                Scene newScene = SceneManager.GetSceneByName(data.TargetScene.ToString());
                SceneManager.SetActiveScene(newScene);
                
                // ローディングスクリーンと古いシーンの削除
                if (data.UseLoadingScreen && loadingScene.IsValid())
                {
                    await SceneManager.UnloadSceneAsync(loadingScene);
                }
                
                // 古いメインシーンをアンロード
                if (currentScene.IsValid() && _currentScene.ToString() != data.TargetScene.ToString())
                {
                    await SceneManager.UnloadSceneAsync(currentScene);
                }
                
                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogUtility.Error($"シーン遷移に失敗しました: {ex.Message}", LogCategory.System);
                return false;
            }
        }

        /// <summary>
        /// 内部的なロード処理
        /// </summary>
        private async UniTask LoadSceneInternal(SceneType targetScene, CancellationToken token)
        {
            await SceneManager.LoadSceneAsync(targetScene.ToString(), LoadSceneMode.Additive);
            token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// 現在のシーンタイプを取得
        /// </summary>
        private SceneType GetCurrentSceneType()
        {
            var currentSceneName = SceneManager.GetActiveScene().name;
            if (Enum.TryParse<SceneType>(currentSceneName, out var sceneType))
            {
                return sceneType;
            }
            return SceneType.None;
        }
    }
}