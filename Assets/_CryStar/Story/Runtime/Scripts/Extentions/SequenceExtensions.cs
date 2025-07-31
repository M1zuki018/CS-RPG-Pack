using CryStar.Story.Enums;
using DG.Tweening;

namespace CryStar.Utility
{
    /// <summary>
    /// シーケンスの拡張メソッド
    /// </summary>
    public static class SequenceExtensions
    {
        public static Sequence AddTween(this Sequence sequence, SequenceType sequenceType, Tween tween)
        {
            return sequenceType switch
            {
                SequenceType.Append => sequence.Append(tween),
                SequenceType.Join => sequence.Join(tween),
                _ => sequence.Insert(0, tween)
            };
        }
    }
}