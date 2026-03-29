using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeenCppEdu.Core.Checkers;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.Tests.Checkers
{
    /// <summary>
    /// 关键字检查器单元测试
    /// </summary>
    [TestClass]
    public class KeywordCheckerTests
    {
        private KeywordChecker _checker;
        private CheckRule _rule;

        [TestInitialize]
        public void Setup()
        {
            _checker = new KeywordChecker();
            _rule = new CheckRule
            {
                Type = CheckType.Keyword,
                Name = "关键字检查",
                Score = 25,
                FailMessage = "缺少必要的关键字",
                SuccessMessage = "关键字检查通过"
            };
            _rule.Parameters["keywords"] = "cout, endl";
        }

        [TestMethod]
        public void Check_HasAllKeywords_ShouldPass()
        {
            // Arrange
            var code = @"#include <iostream>
using namespace std;
int main() {
    cout << ""Hello"" << endl;
    return 0;
}";

            // Act
            var result = _checker.Check(code, _rule);

            // Assert
            Assert.IsTrue(result.IsPassed);
        }

        [TestMethod]
        public void Check_MissingCout_ShouldFail()
        {
            // Arrange
            var code = @"#include <iostream>
using namespace std;
int main() {
    return 0;
}";

            // Act
            var result = _checker.Check(code, _rule);

            // Assert
            Assert.IsFalse(result.IsPassed);
        }

        [TestMethod]
        public void Check_MissingEndl_ShouldFail()
        {
            // Arrange
            var code = @"#include <iostream>
using namespace std;
int main() {
    cout << ""Hello"";
    return 0;
}";

            // Act
            var result = _checker.Check(code, _rule);

            // Assert
            Assert.IsFalse(result.IsPassed);
        }

        [TestMethod]
        public void Check_CaseInsensitive_ShouldPass()
        {
            // Arrange - C++关键字是区分大小写的，但检查器应该能检测到
            var code = @"#include <iostream>
using namespace std;
int main() {
    COUT << ""Hello"" << ENDL;
    return 0;
}";

            // Act
            var result = _checker.Check(code, _rule);

            // Assert - 实际上C++是大小写敏感的，所以这应该失败
            Assert.IsFalse(result.IsPassed);
        }
    }
}
