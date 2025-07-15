using iCON.Enums;

namespace iCON.Battle
{
    /// <summary>
    /// Ideaのマスタデータ
    /// </summary>
    public class IdeaMasterData
    {
        /// <summary>
        /// 使用者のキャラクターID
        /// </summary>
        public int UserCharacterId { get; set; }
        
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// コマンド処理の優先順
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// アイデア名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// アイデアの説明文
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 消費スキルポイント
        /// </summary>
        public int Cost { get; set; }

        /// <summary>
        /// 効果の種類
        /// </summary>
        public IdeaEffectType EffectType { get; set; }

        /// <summary>
        /// 効果値の種類
        /// </summary>
        public EffectValueType ValueType { get; set; }

        /// <summary>
        /// 効果値
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// ターゲットの人数（全員の場合は-1）
        /// </summary>
        public int TargetCount { get; set; }

        /// <summary>
        /// ターゲットの指定方法
        /// </summary>
        public TargetSelectionType SelectionType { get; set; }
    }
}