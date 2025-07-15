using System;
using iCON.Utility;
using UnityEngine;

namespace iCON.Battle
{
    /// <summary>
    /// バトル時に使用するキャラクターデータ
    /// </summary>
    public class BattleUnit
    {
        public BattleUnitMaster Data { get; set; }
        public BattleUnitUserData UserData { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentWill { get; set; }
        public int CurrentStamina { get; set; }
        public int CurrentSp { get; set; }
        public int PhysicalAttack { get; set; }
        public int SkillAttack { get; set; }
        public int Intelligence { get; set; }
        public int PhysicalDefense { get; set; }
        public int SkillDefense { get; set; }
        public int Speed { get; set; }
        public int DodgeSpeed { get; set; }
        public int ArmorPenetration { get; set; }
        
        /// <summary>
        /// ガード中
        /// </summary>
        public bool IsGuarding { get; set; }
        
        public string Name => Data.name;
        
        /// <summary>
        /// 生存しているか
        /// </summary>
        public bool IsAlive => CurrentHp > 0;
        
        /// <summary>
        /// HP変動を通知するコールバック
        /// </summary>
        public event Action<int, int, int> OnHpChanged;
        
        /// <summary>
        /// スキルポイント変動を通知するコールバック
        /// </summary>
        public event Action<int, int> OnSpChanged;

        /// <summary>
        /// 死亡通知
        /// </summary>
        public event Action OnDeath;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">キャラクターID</param>
        public BattleUnit(int id, BattleUnitMaster data)
        {
            // TODO: idを元にデータを検索

            Data = data; // 仮
            UserData = new BattleUnitUserData();
            
            if (Data == null)
            {
                LogUtility.Error($"{id} のBattleUnitMasterがありません", LogCategory.Gameplay);
                return;
            }
            
            if (UserData == null)
            {
                LogUtility.Error($"{id} のBattleUnitUserDataがありません", LogCategory.Gameplay);
                return;
            }
            
            // ユーザーデータ参照
            CurrentHp = UserData.CurrentHp;
            CurrentWill = UserData.CurrentWill;
            CurrentStamina = UserData.CurrentStamina;
            CurrentSp = UserData.CurrentSp;
            
            // マスタデータ参照
            PhysicalAttack = Data.PhysicalAttack;
            SkillAttack = Data.SkillAttack;
            Intelligence = Data.Intelligence;
            PhysicalDefense = Data.PhysicalDefense;
            SkillDefense = Data.SkillDefense;
            Speed = Data.Speed;
            DodgeSpeed = Data.DodgeSpeed;
            ArmorPenetration = Data.ArmorPenetration;
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        public void TakeDamage(int damage)
        {
            // 最小値は0、最大値はMaxHPにおさまるように調整
            var value = Mathf.Max(0, CurrentHp - damage);
            CurrentHp = Mathf.Min(value, Data.Hp);
            OnHpChanged?.Invoke(CurrentHp, Data.Hp, damage);
        }
    }
}
