#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CryStar.Editor
{
    /// <summary>
    /// Summary コメントを取得するクラス
    /// </summary>
    public static class ScriptSummaryExtractor
    {
        /// <summary>
        /// スクリプトからクラス直前の summary コメントを取得
        /// NOTE: クラス名の上に summary コメントをつけていない場合はメソッドのものを取得してしまう場合があるので
        /// 現在一律して取得しないようにしている
        /// </summary>
        public static string GetFormattedSummary(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                return string.Empty;
            }

            string scriptContent = File.ReadAllText(scriptPath);

            // クラス宣言の直前にある <summary> コメントを抽出
            // パターン: summary -> 属性(オプション) -> class宣言
            string pattern =
                @"<summary>\s*([\s\S]*?)\s*</summary>\s*(?:\[[\s\S]*?\]\s*)*(?:public\s+|private\s+|internal\s+|protected\s+)*(?:static\s+|abstract\s+|sealed\s+)*class\s+\w+";

            Match match = Regex.Match(scriptContent, pattern, RegexOptions.Singleline);

            if (match.Success)
            {
                string rawSummary = match.Groups[1].Value;

                // 各行から /// を削除し、前後の空白をトリム
                string[] lines = rawSummary.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var cleanedLines = new List<string>();

                foreach (string line in lines)
                {
                    string cleanedLine = line.Replace("///", "").Trim();
                    if (!string.IsNullOrEmpty(cleanedLine))
                    {
                        cleanedLines.Add(cleanedLine);
                    }
                }

                // 空白1つで結合
                return string.Join(" ", cleanedLines);
            }

            return string.Empty;
        }
    }
}
#endif