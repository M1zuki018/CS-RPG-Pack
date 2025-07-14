namespace iCON.Battle
{
    /// <summary>
    /// 実行待ちのバトルコマンドを表すエントリークラス
    /// </summary>
    public class BattleCommandEntry
    {
        /// <summary>
        /// コマンドを実行するバトルユニット（実行者）
        /// </summary>
        public BattleUnit Executor { get; set; }
        
        /// <summary>
        /// 実行されるバトルコマンドのインスタンス
        /// </summary>
        public IBattleCommand Command { get; set; }
        
        /// <summary>
        /// コマンドの実行対象となるバトルユニットの配列
        /// </summary>
        public BattleUnit[] Targets { get; set; }
        
        /// <summary>
        /// コマンドの実行優先度
        /// </summary>
        public int Priority => Command.Priority;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="executor">コマンドを実行するバトルユニット（実行者）</param>
        /// <param name="command">実行されるバトルコマンドのインスタンス</param>
        /// <param name="targets">コマンドの実行対象となるバトルユニットの配列</param>
        public BattleCommandEntry(BattleUnit executor, IBattleCommand command, BattleUnit[] targets)
        {
            Executor = executor;
            Command = command;
            Targets = targets;
        }
    }
}