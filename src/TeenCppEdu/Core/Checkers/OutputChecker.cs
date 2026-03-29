using System;
using System.Linq;
using System.Text.RegularExpressions;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.Core.Checkers
{
    /// <summary>
    /// 输出检查器 - 检查代码中的输出内容
    /// </summary>
    public class OutputChecker : ICodeChecker
    {
        public string Name => "输出检查";

        public CheckItemResult Check(string sourceCode, CheckRule rule)
        {
            var result = new CheckItemResult
            {
                RuleName = rule.Name,
                Type = CheckType.Output,
                MaxScore = rule.Score
            };

            // 提取所有 cout 语句中的字符串内容
            var coutPattern = @"cout\s*<<\s*""([^""]*)""";
            var matches = Regex.Matches(sourceCode, coutPattern);
            var outputs = matches.OfType<System.Text.RegularExpressions.Match>().Select(m => m.Groups[1].Value).ToList();

            if (!rule.Parameters.TryGetValue("expectedOutput", out var expectedObj) || expectedObj == null)
            {
                result.IsPassed = true;
                result.Score = rule.Score;
                result.Feedback = "输出检查通过";
                result.Details = $"找到输出内容：{string.Join(", ", outputs)}";
                return result;
            }

            var expectedOutput = expectedObj.ToString();
            var matchMode = rule.Parameters.TryGetValue("matchMode", out var mm) ? mm?.ToString() ?? "contains" : "contains";
            var allowPartial = rule.Parameters.TryGetValue("allowPartial", out var ap) && bool.TryParse(ap?.ToString(), out var apv) ? apv : true;

            bool isMatch = false;
            string actualOutput = string.Join("", outputs);

            switch (matchMode)
            {
                case "exact":
                    isMatch = actualOutput == expectedOutput;
                    break;
                case "contains":
                    isMatch = actualOutput.Contains(expectedOutput);
                    break;
                case "any":
                    isMatch = outputs.Any(o => o.Contains(expectedOutput));
                    break;
            }

            if (isMatch)
            {
                result.IsPassed = true;
                result.Score = rule.Score;
                result.Feedback = rule.SuccessMessage ?? "输出内容完全正确！";
                result.Details = $"预期输出包含：{expectedOutput}\n实际输出：{actualOutput}";
            }
            else
            {
                result.IsPassed = allowPartial && actualOutput.Length > 0;
                result.Score = result.IsPassed ? rule.Score / 2 : 0;
                result.Feedback = rule.FailMessage ?? $"输出内容还需要调整哦！需要输出：{expectedOutput}";
                result.Details = $"预期输出：{expectedOutput}\n实际找到的输出：{actualOutput}";
            }

            return result;
        }
    }
}
