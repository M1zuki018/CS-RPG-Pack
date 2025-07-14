using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using iCON.Enums;

namespace iCON.Battle
{
    /// <summary>
    /// バトルコマンドオブジェクトを生成・管理するファクトリークラス
    /// </summary>
    public static class BattleCommandFactory
    {
        /// <summary>
        /// 生成済みコマンドインスタンスのキャッシュ
        /// </summary>
        private static readonly Dictionary<CommandType, IBattleCommand> _commandCache = new();
        
        /// <summary>
        /// 指定されたコマンドタイプに対応するコマンドオブジェクトを取得する
        /// サポートされていないコマンドを要求した場合nullを返す
        /// </summary>
        [return: MaybeNull]
        public static IBattleCommand GetCommand(CommandType commandType)
        {
            if (_commandCache.TryGetValue(commandType, out var cachedCommand))
            {
                // 既に生成済みのコマンドがあればそれを再利用する
                return cachedCommand;
            }
            
            // キャッシュに存在しない場合は、引数に対応するコマンドを新規生成
            IBattleCommand command = commandType switch
            {
                CommandType.Attack => new AttackCommand(), 
                CommandType.Guard => new GuardCommand(),
                // CommandType.Idea => new IdeaCommand(),
                // CommandType.Item => new ItemCommand(),
                _ => null
            };
            
            if (command != null)
            {
                // 生成に成功した場合はキャッシュに保存
                _commandCache[commandType] = command;
            }
            
            return command;
        }
    }
}