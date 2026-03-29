using System.Linq;
using System.Text.RegularExpressions;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.Core.Checkers
{
    /// <summary>
    /// 代码风格检查器 - 检查命名规范、缩进等风格问题
    /// </summary>
    public class StyleChecker : ICodeChecker
    {
        public string Name => "代码风格检查";

        public CheckItemResult Check(string sourceCode, CheckRule rule)
        {
            var result = new CheckItemResult
            {
                RuleName = rule.Name,
                Type = CheckType.Style,
                MaxScore = rule.Score
            };

            var warnings = new System.Collections.Generic.List<string>();
            var checkItems = new System.Collections.Generic.List<string>();

            // 检查1: 代码是否有注释
            if (rule.Parameters.TryGetValue("requireComments", out var rc) && rc != null && rc.ToString().ToLower() == "true")
            {
                bool hasComment = sourceCode.Contains("//") || sourceCode.Contains("/*");
                checkItems.Add(hasComment ? "包含注释" : "建议添加注释");
                if (!hasComment)
                {
                    warnings.Add("建议：给代码添加一些注释，方便自己和他人理解");
                }
            }

            // 检查2: 缩进检查（简单检查 - 混用空格和tab）
            var lines = sourceCode.Split('\n');
            bool hasMixedIndent = false;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith(" ") && line.Contains("\t"))
                {
                    hasMixedIndent = true;
                    break;
                }
            }
            checkItems.Add(hasMixedIndent ? "缩进混用提示" : "缩进一致");
            if (hasMixedIndent)
            {
                warnings.Add("提示：建议保持缩进一致，不要混用空格和Tab");
            }

            // 检查3: 行尾空格
            bool hasTrailingSpaces = lines.Any(l => l.EndsWith(" ") || l.EndsWith("\t"));
            checkItems.Add(hasTrailingSpaces ? "有行尾空格" : "无行尾空格");

            // 检查4: 空行（代码分段）
            bool hasBlankLines = sourceCode.Contains("\n\n");
            checkItems.Add(hasBlankLines ? "代码分段" : "建议分段");

            if (warnings.Any())
            {
                // 风格问题非致命，可以给部分分
                result.IsPassed = true;
                result.Score = rule.Score / 2;
                result.Feedback = rule.FailMessage ?? "代码还有一些小建议可以参考";
                result.Details = string.Join("\n", warnings) + "\n\n检查项：\n" + string.Join("\n", checkItems);
            }
            else
            {
                result.IsPassed = true;
                result.Score = rule.Score;
                result.Feedback = rule.SuccessMessage ?? "代码风格很棒！";
                result.Details = string.Join("\n", checkItems);
            }

            return result;
        }
    }
}
