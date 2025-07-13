namespace iCON.Battle
{
    /// <summary>
    /// バトルに使用するデータ
    /// </summary>
    public class BattleData
    {
        /// <summary>
        /// 戦闘に参加しているキャラクターの数
        /// </summary>
        public int UnitCount { get; private set; } = 1;
        
        /// <summary>
        /// 戦闘に参加している敵の数
        /// </summary>
        public int EnemyCount {get; private set;} = 1;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattleData()
        {
            // TODO: 初期化時に変数も適切な値を代入するようにする
        }
    }
}
