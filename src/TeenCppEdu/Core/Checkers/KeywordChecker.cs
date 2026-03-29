using System;
using System.Linq;
using System.Text.RegularExpressions;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.Core.Checkers
{
    /// <summary>
    /// 关键字检查器 - 检查是否包含指定的关键字
    /// </summary>
    public class KeywordChecker : ICodeChecker
    {
        public string Name => "关键字检查";

        public CheckItemResult Check(string sourceCode, CheckRule rule)
        {
            var result = new CheckItemResult
            {
                RuleName = rule.Name,
                Type = CheckType.Keyword,
                MaxScore = rule.Score
            };

            if (!rule.Parameters.TryGetValue("keywords", out var keywordsObj) || keywordsObj == null)
            {
                result.IsPassed = false;
                result.Feedback = "检查规则配置错误：缺少关键字列表";
                result.Details = "请联系管理员检查课程配置";
                return result;
            }

            var keywords = keywordsObj.ToString().Split(',').Select(k => k.Trim()).Where(k => !string.IsNullOrEmpty(k)).ToList();
            var matchMode = rule.Parameters.TryGetValue("matchMode", out var mm) ? mm?.ToString() : "all";

            var missingKeywords = keywords.Where(k => !sourceCode.Contains(k)).ToList();
            var foundKeywords = keywords.Where(k => sourceCode.Contains(k)).ToList();

            if (matchMode == "all" && missingKeywords.Any())
            {
                result.IsPassed = false;
                result.Feedback = string.Format(rule.FailMessage ?? "你的代码还缺少必要内容哦！", string.Join(", ", missingKeywords));
                result.Details = $"需要包含的关键字：{string.Join(", ", keywords)}\n缺少：{string.Join(", ", missingKeywords)}";
            }
            else if (matchMode == "any" && !foundKeywords.Any())
            {
                result.IsPassed = false;
                result.Feedback = string.Format(rule.FailMessage ?? "你的代码需要包含 {0} 中的至少一个！", string.Join(", ", keywords));
                result.Details = $"需要至少包含以下之一：{string.Join(", ", keywords)}";
            }
            else
            {
                result.IsPassed = true;
                result.Score = rule.Score;
                result.Feedback = rule.SuccessMessage ?? "太棒了！所有必需的关键字都找到了！";
                result.Details = $"找到的关键字：{string.Join(", ", foundKeywords)}";
            }

            return result;
        }
    }
}
