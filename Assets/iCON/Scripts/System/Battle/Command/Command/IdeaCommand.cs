using Cysharp.Threading.Tasks;
using UnityEngine;

namespace iCON.Battle
{
    /// <summary>
    /// アイデアコマンド
    /// </summary>
    public class IdeaCommand : IBattleCommand
    {
        // 優先順は特に設定なし
        public int Priority => 50;
        public string DisplayName => "アイデア";
        
        public bool CanExecute(BattleUnit executor)
        {
            // 使用者が生きていれば実行可能
            return executor.IsAlive;
        }
        
        public async UniTask<BattleCommandResult> ExecuteAsync(BattleUnit executor, BattleUnit[] targets)
        {
            if (targets.Length == 0)
            {
                // 敵がいない場合はコマンド失敗としてリザルトを作成
                return new BattleCommandResult(false, "対象が存在しません");
            }
            
            // 単体攻撃
            var target = targets[0];
            
            // ダメージ計算を行う
            int damage = CalculateDamage(executor, target);
            
            // クリティカル判定を行う
            bool isCritical = CheckCritical(executor.Data.CriticalLate);
            if (isCritical)
            {
                // 小数点以下は丸める
                damage = Mathf.RoundToInt(damage * executor.Data.CriticalDamage);
            }
            
            // ダメージ適用
            target.TakeDamage(damage);
            
            // エフェクトデータ作成
            var effects = new BattleEffectData[]
            {
                new BattleEffectData
                {
                    Target = target,
                    Damage = damage,
                    EffectName = "Idea",
                    IsCritical = isCritical
                }
            };
            
            // ログに表示するメッセージを作成
            string message = isCritical ? 
                $"{executor.Name}の攻撃！会心の一撃！{target.Name}に{damage}のダメージ！" :
                $"{executor.Name}の攻撃！{target.Name}に{damage}のダメージ！";
            
            return new BattleCommandResult(true, message, effects);
        }
        
        /// <summary>
        /// ダメージ計算を行う
        /// </summary>
        private int CalculateDamage(BattleUnit attacker, BattleUnit defender)
        {
            // 基本ダメージ計算式
            int baseDamage = attacker.PhysicalAttack;
            int defense = defender.PhysicalAttack;
            int damage = Mathf.Max(1, baseDamage - defense / 2);
            
            // ランダム要素を追加（±10%）
            float randomFactor = Random.Range(0.9f, 1.1f);
            damage = Mathf.RoundToInt(damage * randomFactor);
            
            return damage;
        }
        
        /// <summary>
        /// クリティカル攻撃か抽選を行う
        /// </summary>
        private bool CheckCritical(float criticalLate)
        {
            return Random.Range(0f, 1f) < criticalLate;
        }
    }
}