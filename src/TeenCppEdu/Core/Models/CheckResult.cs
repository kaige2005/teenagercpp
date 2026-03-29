using System.Collections.Generic;

namespace TeenCppEdu.Core.Models
{
    /// <summary>
    /// 代码检查结果
    /// </summary>
    public class CheckResult
    {
        /// <summary>
        /// 是否通过所有检查
        /// </summary>
        public bool IsPassed { get; set; }

        /// <summary>
        /// 总分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 是否已被导师手动放行
        /// </summary>
        public bool IsManuallyApproved { get; set; }

        /// <summary>
        /// 手动放行导师名称
        /// </summary>
        public string ApprovedBy { get; set; }

        /// <summary>
        /// 放行备注
        /// </summary>
        public string ApprovalNote { get; set; }

        /// <summary>
        /// 各项检查详细结果
        /// </summary>
        public List<CheckItemResult> ItemResults { get; set; } = new List<CheckItemResult>();

        /// <summary>
        /// 汇总信息
        /// </summary>
        public string Summary { get; set; }
    }

    /// <summary>
    /// 单项检查结果
    /// </summary>
    public class CheckItemResult
    {
        /// <summary>
        /// 检查项名称
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// 检查类型
        /// </summary>
        public CheckType Type { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsPassed { get; set; }

        /// <summary>
        /// 该项满分
        /// </summary>
        public int MaxScore { get; set; }

        /// <summary>
        /// 该项得分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 反馈信息（给学生看的提示）
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        /// 详细诊断信息
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// 是否必需项
        /// </summary>
        public bool IsRequired { get; set; }
    }

    public enum CheckType
    {
        Syntax,         // 语法检查
        Keyword,        // 关键字检查
        Output,         // 输出结果检查
        Structure,      // 代码结构检查
        Style           // 代码风格检查
    }
}
