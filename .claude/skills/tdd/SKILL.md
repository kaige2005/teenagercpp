# TDD Skill - 测试驱动开发流程

为TeenC++项目建立标准化的测试驱动开发流程，减少生成代码的runtime错误。

## 理念

**红-绿-重构循环**
1. 🔴 **红**: 编写失败的测试
2. 🟢 **绿**: 编写最小代码使测试通过
3. 🔵 **重构**: 优化代码，保持测试通过

## 用法

```
/tdd <功能描述>
```

## 流程

### Step 1: 理解需求 (5分钟)
- 分析功能规格
- 确定边界条件
- 识别依赖项

### Step 2: 编写测试 (10分钟)
根据功能类型选择测试策略:

**业务逻辑类**
```csharp
[Test]
public void CalculateScore_ValidCode_ReturnsExpectedScore()
{
    // Arrange
    var validator = new CodeValidator();
    var code = "int main() { return 0; }";

    // Act
    var result = validator.CalculateScore(code);

    // Assert
    Assert.AreEqual(100, result);
}

[Test]
public void CalculateScore_NullCode_ThrowsArgumentException()
{
    // Arrange
    var validator = new CodeValidator();

    // Act & Assert
    Assert.Throws<ArgumentException>(() => validator.CalculateScore(null));
}
```

**UI交互类**
```csharp
[Test]
public void CheckCodeButton_Click_ValidInput_ShowsSuccessMessage()
{
    // Arrange
    var form = new LessonForm();
    form.CodeInput.Text = "valid code";

    // Act
    form.CheckCodeButton.PerformClick();

    // Assert
    Assert.IsTrue(form.SuccessLabel.Visible);
}
```

### Step 3: 运行测试确认失败
```bash
dotnet test --filter "FullyQualifiedName~MyTest"
```

### Step 4: 实现最小代码
编写刚好使测试通过的代码，不添加额外功能。

### Step 5: 运行测试确认通过
```bash
dotnet test
```

### Step 6: 重构优化
- 消除重复代码
- 改进命名
- 优化结构

### Step 7: 重新运行测试
确保重构后测试仍然通过。

## .NET Framework 4.8 兼容提示

### ❌ 避免使用
```csharp
// .NET 4.8 不支持
value.GetValueOrDefault()  // 对可空类型
bool.Parse("true")  // 不安全，可能异常
```

### ✅ 使用替代
```csharp
// 安全写法
value ?? defaultValue
value.HasValue ? value.Value : defaultValue

if (bool.TryParse("true", out bool result))
    // use result
```

## 测试分类

### 单元测试 (Unit Tests)
- 不依赖外部资源
- 使用Mock/Stub
- 快速执行

### 集成测试 (Integration Tests)
- 测试数据库操作
- 测试文件IO
- 测试API调用

### UI测试 (UI Tests)
- 测试WinForms控件
- 测试事件处理
- 使用TestStack.White等框架

## 示例工作流程

**场景: 实现"代码填空"功能**

```
/tdd 实现代码填空功能
```

执行流程:
1. **分析需求**
   - 用户输入代码填空答案
   - 系统验证答案正确性
   - 显示正确/错误反馈

2. **编写测试**
   - FillAnswer_CorrectAnswer_ReturnsTrue
   - FillAnswer_WrongAnswer_ReturnsFalse
   - FillAnswer_NullInput_HandlesGracefully

3. **运行测试** → 全部失败 ✅

4. **实现代码**
   ```csharp
   public bool CheckAnswer(string userAnswer, string correctAnswer)
   {
       if (string.IsNullOrEmpty(userAnswer))
           return false;
       return userAnswer.Trim() == correctAnswer.Trim();
   }
   ```

5. **运行测试** → 全部通过 ✅

6. **重构**
   - 提取空格处理逻辑
   - 添加大小写不敏感选项

7. **回归测试** → 通过 ✅

## 常见错误预防

### 空引用异常预防
```csharp
// 总是检查null
if (control == null) throw new ArgumentNullException(nameof(control));

// 使用?. 操作符
var text = label?.Text ?? string.Empty;
```

### UI线程问题
```csharp
// WinForms中使用Invoke
if (this.InvokeRequired)
{
    this.Invoke(new Action(UpdateUI));
    return;
}
```

### 资源释放
```csharp
// 使用using语句
using (var connection = new SQLiteConnection(connectionString))
{
    // use connection
}
```

## 质量门禁

- [ ] 每个新功能至少2个测试用例
- [ ] 边界条件必须测试
- [ ] Null输入必须处理
- [ ] 异常路径必须覆盖
- [ ] 代码覆盖率 > 80% (核心业务)

## 工具

- **NUnit/xUnit**: 测试框架
- **Moq**: Mock框架
- **FluentAssertions**: 断言库
- **Coverlet**: 覆盖率分析
