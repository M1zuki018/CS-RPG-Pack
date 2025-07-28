using System.Linq;
using CryStar.Attribute;
using UnityEngine;

namespace iCON.System
{
    /// <summary>
    /// ストーリーを実行するために必要なデータを設定するためのスクリプタブルオブジェクト
    /// </summary>
    [CreateAssetMenu(fileName = "SceneDataSO", menuName = "Scriptable Objects/SceneDataSO")]
    public class SceneDataSO : ScriptableObject
    {
        [Header("再生するストーリーの設定")]
        [SerializeField]
        private StoryLine _storyLine;
        
        [SerializeField, Comment("キャラクターの立ち絵の拡大率")]
        private float _characterScale = 1;

        [SerializeField, Comment("キャラクターの立ち絵の位置補正量")]
        private Vector3 _characterPositionOffset = Vector3.zero;
        
        [SerializeField, Comment("前提ストーリー")]
        private StorySaveData[] _premiseStoryArray;
        
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

        /// <summary>
        /// 前提ストーリーを全てクリアしているか
        /// </summary>
        public bool IsPremiseAllStoryClear()
        {
            return _premiseStoryArray.All(StoryUserData.IsPremiseStoryClear);
        }
    }
}