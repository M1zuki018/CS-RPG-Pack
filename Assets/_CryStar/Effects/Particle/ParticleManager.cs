using System.Collections.Generic;
using CryStar.Core;
using CryStar.Core.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CryStar.Story.Execution
{
    /// <summary>
    /// ParticleSystemを管理するManager
    /// </summary>
    public class ParticleManager : CustomBehaviour
    {
        /// <summary>
        /// ParticleSystemのデータ配列
        /// </summary>
        [Header("Particleの設定")]
        [SerializeField] 
        private ParticleData[] _particleDataArray;
        
        /// <summary>
        /// Particleのキャッシュ
        /// </summary>
        private Dictionary<int, ParticleData> _particleCache = new Dictionary<int, ParticleData>();

        #region Life cycle

        /// <summary>
        /// Awake
        /// </summary>
        public override async UniTask OnAwake()
        {
            ServiceLocator.Register(this, ServiceType.Local);
            await base.OnAwake();
        }

        /// <summary>
        /// Destroy
        /// </summary>
        private void OnDestroy()
        {
            CleanupAllParticles();
        }

        #endregion
        
        /// <summary>
        /// Play
        /// </summary>
        public void PlayParticle(int index)
        {
            // キャッシュが存在するか検索
            if (!_particleCache.ContainsKey(index))
            {
                if (_particleDataArray == null || _particleDataArray[index] == null)
                {
                    // 配列がnull、データが存在しない場合はreturn
                    return;
                }
                
                // Particleが生成済みでない場合、インスタンスを生成して辞書に登録
                var particleData = _particleDataArray[index].Instantiate(gameObject.transform);
                _particleCache[index] = particleData;
            }
            
            // 再生
            _particleCache[index].SetActive(true);
            _particleCache[index].Play();
        }

        /// <summary>
        /// Stop
        /// </summary>
        public void StopParticle(int index)
        {
            if (!_particleCache.ContainsKey(index))
            {
                return;
            }
            
            // 停止
            _particleCache[index].Stop();
            _particleCache[index].SetActive(false);
        }

        #region Private Methods

        /// <summary>
        /// 全パーティクルのクリーンアップ
        /// </summary>
        private void CleanupAllParticles()
        {
            foreach (var particleData in _particleCache.Values)
            {
                // リソースのクリーンアップ
                particleData?.Dispose();
            }
            
            _particleCache.Clear();
        }

        #endregion
    }
}
