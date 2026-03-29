using System.Collections.Generic;

namespace TeenCppEdu.Core.Models
{
    /// <summary>
    /// 代码检查规则配置
    /// </summary>
    public class CheckRule
    {
        /// <summary>
        /// 规则ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 规则名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 检查类型
        /// </summary>
        public CheckType Type { get; set; }

        /// <summary>
        /// 该项分值
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 是否必需（必需项失败则整体不通过）
        /// </summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>
        /// 检查参数
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 失败时提示信息
        /// </summary>
        public string FailMessage { get; set; }

        /// <summary>
        /// 成功时提示信息
        /// </summary>
        public string SuccessMessage { get; set; }
    }

    /// <summary>
    /// 关卡检查规则集
    /// </summary>
    public class LessonCheckRules
    {
        public string LessonId { get; set; }
        public List<CheckRule> Rules { get; set; } = new List<CheckRule>();

        /// <summary>
        /// 预期输出（用于输出对比检查）
        /// </summary>
        public string ExpectedOutput { get; set; }

        /// <summary>
        /// 是否允许近似匹配
        /// </summary>
        public bool AllowPartialMatch { get; set; } = true;
    }
}
