using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// ストーリーを実行するために必要なデータを設定するためのスクリプタブルオブジェクト
    /// </summary>
    [CreateAssetMenu(fileName = "StoryExecuteDataSO", menuName = "Scriptable Objects/StoryExecuteDataSO")]
    public class StoryExecuteDataSO : ScriptableObject
    {
        [Header("再生するストーリーの設定")]
        [SerializeField]
        private StoryLine _storyLine;

        [SerializeField, Comment("キャラクターの立ち絵の拡大率")]
        private float _characterScale = 1;

        [Comment("キャラクターの立ち絵の位置補正量")]
        public Vector3 _characterPositionOffset = Vector3.zero;
        
        /// <summary>
        /// 再生するストーリーの設定
        /// </summary>
        public StoryLine StoryLine => _storyLine;
        
        /// <summary>
        /// キャラクターの立ち絵の拡大率
        /// </summary>
        public float CharacterScale => _characterScale;
        
        /// <summary>
        /// キャラクターの立ち絵の位置補正量
        /// </summary>
        public Vector3 CharacterPositionOffset => _characterPositionOffset;
    }
}