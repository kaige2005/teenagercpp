using System.Linq;
using System.Text.RegularExpressions;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.Core.Checkers
{
    /// <summary>
    /// 代码结构检查器 - 检查代码结构是否符合教学要求
    /// </summary>
    public class StructureChecker : ICodeChecker
    {
        public string Name => "结构检查";

        public CheckItemResult Check(string sourceCode, CheckRule rule)
        {
            var result = new CheckItemResult
            {
                RuleName = rule.Name,
                Type = CheckType.Structure,
                MaxScore = rule.Score
            };

            var errors = new System.Collections.Generic.List<string>();
            var checks = new System.Collections.Generic.List<string>();

            // 检查 requiredElements
            if (rule.Parameters.TryGetValue("requiredElements", out var elementsObj) && elementsObj != null)
            {
                var elements = elementsObj.ToString().Split(',').Select(e => e.Trim()).Where(e => !string.IsNullOrEmpty(e));
                foreach (var element in elements)
                {
                    switch (element.ToLower())
                    {
                        case "include":
                            // 检查是否包含 #include <...> 或 #include "..."
                            var hasInclude = sourceCode.Contains("#include") && (sourceCode.Contains("<") || sourceCode.Contains("\""));
                            checks.Add(hasInclude ? "包含头文件" : "缺少头文件包含");
                            if (!hasInclude) errors.Add("需要使用 #include 包含必要的头文件");
                            break;

                        case "namespace":
                            var hasNamespace = Regex.IsMatch(sourceCode, @"using\s+namespace\s+std");
                            checks.Add(hasNamespace ? "使用命名空间" : "未使用命名空间");
                            break;

                        case "main":
                            var hasMain = Regex.IsMatch(sourceCode, @"\bint\s+main\s*\(");
                            checks.Add(hasMain ? "包含main函数" : "缺少main函数");
                            if (!hasMain) errors.Add("必须包含 int main() 函数");
                            break;

                        case "return":
                            var hasReturn = Regex.IsMatch(sourceCode, @"\breturn\s+0\s*;");
                            checks.Add(hasReturn ? "正确返回0" : "未返回0");
                            break;
                    }
                }
            }

            // 检查 prohibitedElements（禁止元素）
            if (rule.Parameters.TryGetValue("prohibitedElements", out var prohibitedObj) && prohibitedObj != null)
            {
                var prohibited = prohibitedObj.ToString().Split(',').Select(e => e.Trim()).Where(e => !string.IsNullOrEmpty(e));
                foreach (var element in prohibited)
                {
                    var found = sourceCode.Contains(element);
                    if (found)
                    {
                        errors.Add($"不应使用：{element}");
                    }
                }
            }

            if (errors.Any())
            {
                result.IsPassed = false;
                result.Feedback = rule.FailMessage ?? "代码结构还需要调整一下！";
                result.Details = string.Join("\n", errors);
            }
            else
            {
                result.IsPassed = true;
                result.Score = rule.Score;
                result.Feedback = rule.SuccessMessage ?? "代码结构很棒！";
                result.Details = string.Join("\n", checks);
            }

            return result;
        }
    }
}
