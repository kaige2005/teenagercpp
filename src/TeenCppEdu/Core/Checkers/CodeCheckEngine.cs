using System.Collections.Generic;
using System.Linq;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.Core.Checkers
{
    /// <summary>
    /// 代码检查引擎 - 协调各检查器完成全面检查
    /// </summary>
    public class CodeCheckEngine
    {
        private readonly Dictionary<CheckType, ICodeChecker> _checkers;

        public CodeCheckEngine()
        {
            _checkers = new Dictionary<CheckType, ICodeChecker>
            {
                { CheckType.Syntax, new SyntaxChecker() },
                { CheckType.Keyword, new KeywordChecker() },
                { CheckType.Output, new OutputChecker() },
                { CheckType.Structure, new StructureChecker() }
            };
        }

        /// <summary>
        /// 执行完整的代码检查
        /// </summary>
        public CheckResult CheckCode(string sourceCode, LessonCheckRules rules)
        {
            var result = new CheckResult();

            // 防御性检查
            if (rules?.Rules == null)
            {
                result.Summary = "❌ 检查规则未加载，无法进行检查";
                result.IsPassed = false;
                return result;
            }

            int totalScore = 0;
            int maxScore = 0;
            bool requiredPassed = true;

            foreach (var rule in rules.Rules)
            {
                if (!_checkers.TryGetValue(rule.Type, out var checker))
                    continue;

                var itemResult = checker.Check(sourceCode, rule);
                itemResult.IsRequired = rule.IsRequired;
                result.ItemResults.Add(itemResult);

                totalScore += itemResult.Score;
                maxScore += itemResult.MaxScore;

                if (rule.IsRequired && !itemResult.IsPassed)
                {
                    requiredPassed = false;
                }
            }

            result.Score = totalScore;
            result.IsPassed = requiredPassed && totalScore >= maxScore * 0.6; // 60%通过线
            result.Summary = BuildSummary(result);

            return result;
        }

        /// <summary>
        /// 人工放行处理
        /// </summary>
        public void ManualApprove(CheckResult result, string approvedBy, string note)
        {
            result.IsManuallyApproved = true;
            result.ApprovedBy = approvedBy;
            result.ApprovalNote = note;
            result.IsPassed = true;
        }

        private string BuildSummary(CheckResult result)
        {
            var passed = result.ItemResults.Count(r => r.IsPassed);
            var total = result.ItemResults.Count;
            var requiredFailed = result.ItemResults.Where(r => !r.IsPassed && r.IsRequired).Select(r => r.RuleName);

            if (result.IsPassed)
            {
                return $"🎉 太棒了！通过了所有检查！得分：{result.Score}/{total * 100}";
            }
            else if (requiredFailed.Any())
            {
                return $"⚠️ 还有必要项没有通过：{string.Join(", ", requiredFailed)}，继续努力！";
            }
            else
            {
                return $"💪 已完成 {passed}/{total} 项检查，得分：{result.Score}，再完善一下！";
            }
        }
    }
}
