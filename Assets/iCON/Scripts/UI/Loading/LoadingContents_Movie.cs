using UnityEngine;
using UnityEngine.Video;
using Random = System.Random;

namespace iCON.UI
{
    /// <summary>
    /// ロード中に表示するムービーを管理するクラス
    /// </summary>
    public class LoadingContents_Movie : MonoBehaviour
    {
        /// <summary>
        /// 乱数生成器
        /// </summary>
        private static Random _random = new Random();
        
        /// <summary>
        /// ビデオプレイヤー
        /// </summary>
        [SerializeField]
        private VideoPlayer _videoPlayer;
        
        /// <summary>
        /// ビデオクリップ
        /// TODO: 後ほどAddressableに対応させる
        /// </summary>
        [SerializeField]
        private VideoClip[] _videoClip;

        private void Awake()
        {
            if (_videoPlayer != null)
            {
                // ランダムで抽選を行い、再生するクリップを決定する
                _videoPlayer.clip = _videoClip[_random.Next(_videoClip.Length)];
            }
        }
    }

}
