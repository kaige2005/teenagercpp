using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeenCppEdu.Core.Checkers;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.Tests.Checkers
{
    /// <summary>
    /// 语法检查器单元测试
    /// </summary>
    [TestClass]
    public class SyntaxCheckerTests
    {
        private SyntaxChecker _checker;
        private CheckRule _rule;

        [TestInitialize]
        public void Setup()
        {
            _checker = new SyntaxChecker();
            _rule = new CheckRule
            {
                Type = CheckType.Syntax,
                Name = "语法基础检查",
                Score = 25,
                FailMessage = "语法错误",
                SuccessMessage = "语法正确"
            };
        }

        [TestMethod]
        public void Check_ValidCode_ShouldPass()
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
            Assert.IsTrue(result.Score > 0);
        }

        [TestMethod]
        public void Check_MissingSemicolon_ShouldFail()
        {
            // Arrange
            var code = @"#include <iostream>
using namespace std
int main() {
    return 0
}";

            // Act
            var result = _checker.Check(code, _rule);

            // Assert
            Assert.IsFalse(result.IsPassed);
        }

        [TestMethod]
        public void Check_EmptyCode_ShouldFail()
        {
            // Arrange
            var code = "";

            // Act
            var result = _checker.Check(code, _rule);

            // Assert
            Assert.IsFalse(result.IsPassed);
        }

        [TestMethod]
        public void Check_NoMainFunction_ShouldFail()
        {
            // Arrange
            var code = @"#include <iostream>
using namespace std;";

            // Act
            var result = _checker.Check(code, _rule);

            // Assert
            Assert.IsFalse(result.IsPassed);
        }
    }
}
