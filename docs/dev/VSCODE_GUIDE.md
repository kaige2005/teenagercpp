# VS Code 开发调试指南

## 🎯 快速开始

### 1. 安装必要插件

打开 VS Code，按 `Ctrl+Shift+X`，安装以下插件：

| 插件名称 | 作用 |
|---------|------|
| **C# Dev Kit** | 官方 C# 开发套件（必需） |
| **C#** | IntelliSense、调试支持（必需） |
| **.NET Install Tool** | 自动安装 .NET SDK |

> 打开项目文件夹时 VS Code 也会提示安装推荐插件

### 2. 确保 .NET SDK 已安装

```bash
# 打开终端 (Ctrl+`) 运行
dotnet --version
```

如果没安装，下载 [.NET Framework 4.8 开发者包](https://dotnet.microsoft.com/download/dotnet-framework/net48)

---

## 🚀 编译运行

### 方式一：使用快捷键

| 操作 | 快捷键 |
|------|--------|
| 生成项目 | `Ctrl+Shift+B` |
| 开始调试 | `F5` |
| 不调试运行 | `Ctrl+F5` |
| 停止调试 | `Shift+F5` |

### 方式二：使用命令面板

按 `Ctrl+Shift+P`，输入：
- `Tasks: Run Task` → 选择 `build` 编译
- `Debug: Start Debugging` → 开始调试

### 方式三：使用侧边栏

1. **编译**：点击左侧活动栏"终端"图标 → 运行任务 → build
2. **调试**：点击左侧活动栏"调试图标" → 选择"启动 WinForms 应用" → 点击绿箭头

---

## 🐛 调试技巧

### 1. 设置断点

在代码行号左侧点击，设置红色断点：

```csharp
// 在 LessonForm.cs 的常见位置设置断点
private void BtnCheck_Click(object sender, EventArgs e)
{
    // 👈 在这里设断点，查看检查流程
    var engine = new CodeCheckEngine();
    var result = engine.CheckCode(txtCode.Text, _checkRules);
    // ...
}
```

### 2. 调试时观察变量

调试时，鼠标悬停变量查看值，或在下方面板添加监视：

```csharp
// 可以在监视窗口添加这些表达式
result.IsPassed          // 查看是否通过
result.ItemResults.Count // 查看检查项数量
_progress.Experience     // 查看当前经验值
```

### 3. 常用的断点位置

| 文件 | 行 | 调试什么 |
|------|-----|---------|
| `LessonForm.cs` | `BtnCheck_Click` | 代码检查流程 |
| `CodeCheckEngine.cs` | `CheckCode` | 检查器执行过程 |
| `SyntaxChecker.cs` | `Check` | 语法检查结果 |
| `DatabaseService.cs` | `SaveSubmission` | 数据保存 |
| `DevCppProjectGenerator.cs` | `GenerateProject` | 项目生成过程 |

---

## 📝 输出面板

### 查看编译输出
```
视图 → 输出 (Ctrl+Shift+U) → 选择 "Tasks"
```

### 查看调试控制台
```
视图 → 调试控制台 (Ctrl+Shift+Y)
```

### 查看终端
```
视图 → 终端 (Ctrl+`)
```

---

## 🔧 常见问题

### 问题1: "无法启动程序"
**症状**：按 F5 提示找不到 .exe
**解决**：
```bash
# 手动运行一次编译
dotnet build

# 然后确保 launch.json 中的路径正确
```

### 问题2: 断点不生效
**症状**：断点显示为空心圆圈
**解决**：
1. 确保是 `Debug` 配置编译（不是 Release）
2. 检查 DebugType 是否为 `portable`
3. 项目文件已配置：`<DebugType>portable</DebugType>`

### 问题3: 找不到课程数据
**症状**：运行时报找不到 courses/lesson01
**解决**：
```bash
# 方案1：手动复制
xcopy /E /I courses src\TeenCppEdu\bin\Debug\net48\courses

# 方案2：使用 VS Code 任务自动复制（已配置在 tasks.json）
```

### 问题4: 缺少 NuGet 包
**症状**：编译错误 "找不到包"
**解决**：
```bash
# 终端执行
dotnet restore

# 或
dotnet restore TeenCppEdu.csproj
```

### 问题5: .NET Framework 4.8 不可用
**症状**：提示 TargetFramework 不支持
**解决**：
1. 安装 [.NET Framework 4.8 开发者包](https://dotnet.microsoft.com/download/dotnet-framework/net48)
2. 或使用 .NET 6+（需修改项目）：
   ```xml
   <TargetFramework>net6.0-windows</TargetFramework>
   ```

---

## 🎮 调试特定场景

### 场景1：测试代码检查

```bash
# 1. 在 LessonForm.Line~200 处打断点 (BtnCheck_Click)
# 2. 进入第1课
# 3. 修改代码（故意写错）
# 4. 点击"检查代码"
# 5. 单步执行(F10)查看检查过程
```

### 场景2：测试导师放行

```bash
# 1. 在 ManualApproveDialog.Line~100 处打断点 (BtnConfirm_Click)
# 2. 故意让代码检查失败
# 3. 点击"导师放行"
# 4. 输入导师信息
# 5. 单步执行查看数据库保存
```

### 场景3：测试项目生成

```bash
# 1. 在 DevCppProjectGenerator.Line~30 处打断点
# 2. 点击"生成项目"
# 3. 检查生成的文件路径和内容
```

---

## 📂 VS Code 配置说明

### tasks.json 任务
| 任务名 | 作用 |
|--------|------|
| `restore` | 还原 NuGet 包 |
| `build` | 编译（默认） |
| `build-release` | Release 编译 |
| `copy-courses` | 复制课程数据 |
| `clean` | 清理编译结果 |
| `run` | 编译并运行 |

### launch.json 配置
| 配置名 | 作用 |
|--------|------|
| `启动 WinForms 应用` | 正常调试运行 |
| `附加到进程` | 调试已运行的程序 |

---

## 💡 实用快捷键

| 快捷键 | 功能 |
|--------|------|
| `F12` | 跳转到定义 |
| `Shift+F12` | 查找所有引用 |
| `F2` | 重命名符号 |
| `Ctrl+.` | 快速修复 |
| `Ctrl+Shift+O` | 跳转到文件中的符号 |
| `Ctrl+T` | 全局符号查找 |
| `Ctrl+K Ctrl+C` | 注释选中行 |
| `Ctrl+K Ctrl+U` | 取消注释 |
| `Ctrl+K Ctrl+D` | 格式化文档 |
| `Ctrl+Shift+\` | 跳转到匹配的括号 |

---

## 📊 调试窗口布局建议

调整布局以便开发调试：

```
┌─────────────────────────────────────────┐
│  Explorer │ [代码编辑区]        │ 调试   │
│  文件树   │                      │ 变量   │
│           │                      │ 监视   │
│           │                      │ 调用栈 │
├───────────┼──────────────────────┼───────┤
│ 终端/输出/调试控制台                    │
└─────────────────────────────────────────┘
```

拖动右侧边栏调整宽度，点击右上角按钮切换面板。

---

现在可以直接在 VS Code 中按 `F5` 开始调试了！🚀
