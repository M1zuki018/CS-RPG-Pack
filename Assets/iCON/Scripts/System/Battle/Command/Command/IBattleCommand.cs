using Cysharp.Threading.Tasks;
using iCON.Battle;
using UnityEngine;

namespace iCON.Battle
{
    /// <summary>
    /// バトルコマンドのインターフェース
    /// </summary>
    public interface IBattleCommand
    {
        /// <summary>
        /// コマンドを実行する
        /// </summary>
        /// <param name="executor">実行者</param>
        /// <param name="targets">対象</param>
        /// <returns>実行結果</returns>
        public UniTask<BattleCommandResult> ExecuteAsync(BattleUnit executor, BattleUnit[] targets);
        
        /// <summary>
        /// コマンドが実行可能かチェック
        /// </summary>
        public bool CanExecute(BattleUnit executor);
        
        /// <summary>
        /// コマンドの優先度
        /// NOTE: 基本は速度パラメーターを元に計算されるが特別なコマンドの場合はここの値を変更する
        /// </summary>
        public int Priority { get; }
        
        /// <summary>
        /// コマンドの表示名
        /// </summary>
        public string DisplayName { get; }
    }   
}