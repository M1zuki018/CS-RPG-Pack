namespace iCON.Battle
{
    /// <summary>
    /// バトルエフェクトのデータ
    /// </summary>
    public class BattleEffectData
    {
        /// <summary>
        /// コマンドを実行する対象
        /// </summary>
        public BattleUnit Target { get; set; }
        
        /// <summary>
        /// 与えるダメージ量（回復の場合は負の数にする）
        /// </summary>
        public int Damage { get; set; }
        
        /// <summary>
        /// 効果名
        /// </summary>
        public string EffectName { get; set; }
        
        /// <summary>
        /// クリティカル判定かどうか
        /// </summary>
        public bool IsCritical { get; set; }
    }
}
