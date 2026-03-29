using TeenCppEdu.Core.Models;

namespace TeenCppEdu.Core.Checkers
{
    /// <summary>
    /// 代码检查器接口
    /// </summary>
    public interface ICodeChecker
    {
        /// <summary>
        /// 检查器名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 执行检查
        /// </summary>
        /// <param name="sourceCode">学生源代码</param>
        /// <param name="rule">检查规则</param>
        /// <returns>检查结果</returns>
        CheckItemResult Check(string sourceCode, CheckRule rule);
    }
}
