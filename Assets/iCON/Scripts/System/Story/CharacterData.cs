using UnityEngine;

namespace iCON.System
{   
    /// <summary>
    /// キャラクターデータ
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterData", menuName = "iCON/Story/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _fullName;
        [SerializeField] private string _displayName;
        [SerializeField] private string _defaultAssetPath;
        [SerializeField] private Color _characterColor = Color.white;
        [SerializeField] private float _textSpeed;

        /// <summary>キャラクターの管理ID</summary>
        public string ID => _id;
        
        /// <summary>フルネーム</summary>
        public string FullName => _fullName;
        
        /// <summary>表示名</summary>
        public string DisplayName => _displayName;
        
        /// <summary>ベースとなる素材のパス</summary>
        public string DefaultAssetPath => _defaultAssetPath;
        
        /// <summary>キャラクターカラー</summary>
        public Color CharacterColor => _characterColor;
        
        /// <summary>テキストの表示速度</summary>
        public float TextSpeed => _textSpeed;
    }
}