using System.Collections.Generic;
using iCON.Utility;
using UnityEngine;

namespace iCON.Battle
{
    /// <summary>
    /// バトルに使用するデータ
    /// </summary>
    public class BattleData
    {
        /// <summary>
        /// 戦闘に参加しているキャラクターのデータ
        /// </summary>
        public List<BattleUnit> UnitData { get; private set; }
        
        /// <summary>
        /// 戦闘に参加している敵のデータ
        /// </summary>
        public List<BattleUnit> EnemyData {get; private set;}
        
        /// <summary>
        /// 戦闘に参加しているキャラクターの数
        /// </summary>
        public int UnitCount => UnitData.Count;

        /// <summary>
        /// 戦闘に参加している敵の数
        /// </summary>
        public int EnemyCount => EnemyData.Count;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattleData(IReadOnlyList<int> units, IReadOnlyList<int> enemies, 
            List<BattleUnitMaster> unitDataList, List<BattleUnitMaster> enemyDataList)
        {
            // ユニットのデータリストを作成
            UnitData = new List<BattleUnit>(units.Count);
            for (int i = 0; i < units.Count; i++)
            {
                // キャラクターIDを渡してバトルデータを生成
                var unitData = new BattleUnit(units[i], unitDataList[i]);
                UnitData.Add(unitData);
                LogUtility.Verbose($"生成されたUnitData {UnitCount}", LogCategory.Gameplay);
            }
            
            // 敵のデータリストを作成
            EnemyData = new List<BattleUnit>(enemies.Count);
            for (int i = 0; i < enemies.Count; i++)
            {
                // キャラクターIDを渡してバトルデータを生成
                var enemyData = new BattleUnit(enemies[i], enemyDataList[i]);
                EnemyData.Add(enemyData);
                LogUtility.Verbose($"生成されたEnemyData {EnemyCount}", LogCategory.Gameplay);
            }
        }
    }
}
