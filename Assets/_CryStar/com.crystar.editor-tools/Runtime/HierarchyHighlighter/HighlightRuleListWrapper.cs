using System;
using System.Collections.Generic;

namespace CryStar.Editor
{
    /// <summary>
    /// Hierarchy Highlighterで使用するJsonUtility用のラッパークラス
    /// </summary>
    [Serializable]
    public class HighlightRuleListWrapper
    {
        public List<HighlightRule> Rules;
    }
}