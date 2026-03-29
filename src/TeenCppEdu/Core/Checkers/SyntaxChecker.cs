using System;
using System.Linq;
using System.Text.RegularExpressions;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.Core.Checkers
{
    /// <summary>
    /// 语法检查器 - 基础语法检查
    /// </summary>
    public class SyntaxChecker : ICodeChecker
    {
        public string Name => "语法检查";

        public CheckItemResult Check(string sourceCode, CheckRule rule)
        {
            var result = new CheckItemResult
            {
                RuleName = rule.Name,
                Type = CheckType.Syntax,
                MaxScore = rule.Score
            };

            var errors = new System.Collections.Generic.List<string>();

            // 检查1: main函数是否存在
            if (!Regex.IsMatch(sourceCode, @"\bint\s+main\s*\("))
            {
                errors.Add("缺少 main 函数，C++程序必须从 main 函数开始");
            }

            // 检查2: 大括号匹配
            int openBraces = sourceCode.Count(c => c == '{');
            int closeBraces = sourceCode.Count(c => c == '}');
            if (openBraces != closeBraces)
            {
                errors.Add($"大括号不匹配：有 {openBraces} 个'{{'，{closeBraces} 个'}}'");
            }

            // 检查3: 括号匹配
            int openParens = sourceCode.Count(c => c == '(');
            int closeParens = sourceCode.Count(c => c == ')');
            if (openParens != closeParens)
            {
                errors.Add("小括号不匹配，检查函数调用或表达式");
            }

            // 检查4: 分号检查（简单检查，不处理字符串内的情况）
            var lines = sourceCode.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                // 跳过空行、注释行、预处理指令、花括号行、for循环
                if (string.IsNullOrWhiteSpace(line) ||
                    line.StartsWith("//") ||
                    line.StartsWith("#") ||
                    line.StartsWith("/*") ||
                    line == "{" || line == "}" ||
                    line.StartsWith("for(") || line.StartsWith("for "))
                    continue;

                // 简单检查：某些语句应该以分号结尾
                if ((line.Contains("cout") || line.Contains("cin") || line.Contains("return")) && !line.EndsWith(";"))
                {
                    errors.Add($"第 {i + 1} 行可能缺少分号");
                }
            }

            if (errors.Any())
            {
                result.IsPassed = false;
                result.Feedback = rule.FailMessage ?? "发现一些语法问题，请仔细检查一下！";
                result.Details = string.Join("\n", errors);
            }
            else
            {
                result.IsPassed = true;
                result.Score = rule.Score;
                result.Feedback = rule.SuccessMessage ?? "语法检查通过，代码看起来很规范！";
                result.Details = "未发现明显语法错误";
            }

            return result;
        }
    }
}
