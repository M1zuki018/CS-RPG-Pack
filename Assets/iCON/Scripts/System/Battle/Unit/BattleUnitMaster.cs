using UnityEngine;

namespace iCON.Battle
{
    /// <summary>
    /// バトルのキャラクターのパラメーター
    /// </summary>
    [CreateAssetMenu(fileName = "BattleUnitMaster", menuName = "Scriptable Objects/Battle/BattleUnitMaster")]
    public class BattleUnitMaster : ScriptableObject
    {
        [SerializeField] private int _characterId;
        [SerializeField] private string _name;
        [SerializeField] private int _hp;
        [SerializeField] private int _will;
        [SerializeField] private int _stamina;
        [SerializeField] private int _sp;
        [SerializeField] private int _physicalAttack;
        [SerializeField] private int _skillAttack;
        [SerializeField] private int _intelligence;
        [SerializeField] private int _physicalDefense;
        [SerializeField] private int _skillDefense;
        [SerializeField] private int _speed;
        [SerializeField] private int _dodgeSpeed;
        [SerializeField] private int _armorPenetration;
        [SerializeField] private float _criticalLate; // TODO: 値の登録方法については検討中
        [SerializeField] private float _criticalDamage; // TODO: 値の登録方法については検討中
        [SerializeField] private string _iconPath;

        /// <summary>
        /// キャラクターID
        /// </summary>
        public int Id => _characterId;
        
        /// <summary>
        /// キャラクター名
        /// </summary>
        public string Name => _name;
        
        /// <summary>
        /// HP
        /// </summary>
        public int Hp => _hp;

        /// <summary>
        /// 意思力
        /// </summary>
        public int Will => _will;

        /// <summary>
        /// スタミナ
        /// </summary>
        public int Stamina => _stamina;

        /// <summary>
        /// スキルポイント
        /// </summary>
        public int Sp => _sp;

        /// <summary>
        /// 物理攻撃力
        /// </summary>
        public int PhysicalAttack => _physicalAttack;

        /// <summary>
        /// スキル攻撃力
        /// </summary>
        public int SkillAttack => _skillAttack;

        /// <summary>
        /// 知力
        /// </summary>
        public int Intelligence => _intelligence;

        /// <summary>
        /// 物理防御力
        /// </summary>
        public int PhysicalDefense => _physicalDefense;

        /// <summary>
        /// スキル防御力
        /// </summary>
        public int SkillDefense => _skillDefense;

        /// <summary>
        /// 攻撃速度
        /// </summary>
        public int Speed => _speed;

        /// <summary>
        /// 回避速度
        /// </summary>
        public int DodgeSpeed => _dodgeSpeed;

        /// <summary>
        /// 防御無視
        /// </summary>
        public int ArmorPenetration => _armorPenetration;
        
        /// <summary>
        /// クリティカル率
        /// </summary>
        public float CriticalLate => _criticalLate;
        
        /// <summary>
        /// クリティカルダメージ
        /// </summary>
        public float CriticalDamage => _criticalDamage;
        
        /// <summary>
        /// キャラクターアイコンのパス
        /// </summary>
        public string IconPath => _iconPath;
    }
}