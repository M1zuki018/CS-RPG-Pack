using System.Collections.Generic;

namespace iCON.Battle
{
    /// <summary>
    /// キャラクターごとの現在のパラメーターを保存するクラス
    /// </summary>
    public class BattleUnitUserData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int CurrentHp { get; set; } = 300;
        public int CurrentWill { get; set; } = 100;
        public int CurrentStamina { get; set; } = 50;
        public int CurrentSp { get; set; } = 50;
        public List<int> IdeaIdList { get; set; } = new List<int>();
    }
}
