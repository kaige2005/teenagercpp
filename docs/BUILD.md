# TeenC++ 教学系统 - 编译指南

## 环境要求

### 必需组件
- **.NET Framework 4.8** 或更高版本（Win10/11 已内置）
- **Visual Studio 2019/2022** 或 **Visual Studio Code** + C# 扩展
- **Dev-C++ 5.11**（学生端编译运行代码用）

### 依赖包
项目使用 NuGet 包管理，会自动恢复：
- `Newtonsoft.Json` (13.0.3) - JSON 序列化
- `System.Data.SQLite.Core` (1.0.118) - SQLite 数据库

---

## 编译步骤

### 方式一：Visual Studio（推荐）

1. **打开解决方案**
   ```
   双击 src\TeenCppEdu.sln
   ```

2. **还原 NuGet 包**
   - 右键解决方案 → "还原 NuGet 包"
   - 或点击菜单：工具 → NuGet 包管理器 → 管理解决方案的 NuGet 包

3. **编译项目**
   - 按 `Ctrl+Shift+B` 或点击菜单：生成 → 生成解决方案
   - 输出路径：`src\TeenCppEdu\bin\Debug\`

4. **复制课程数据**
   编译后需要手动复制课程数据到输出目录：
   ```bash
   xcopy /E /I courses src\TeenCppEdu\bin\Debug\courses
   ```

### 方式二：命令行编译

```bash
# 进入 src 目录
cd TeenCppEdu\src

# 还原包
dotnet restore TeenCppEdu.sln

# 编译
dotnet build TeenCppEdu.sln --configuration Release

# 复制数据
xcopy /E /I courses TeenCppEdu\bin\Release\net48\courses
```

---

## 运行程序

### 开发环境运行
```bash
src\TeenCppEdu\bin\Debug\TeenCppEdu.exe
```

### 打包分发
将以下内容打包给学生：
```
TeenCppEdu/
├── TeenCppEdu.exe      # 主程序
├── courses/            # 课程数据
├── output/             # 生成的项目目录（运行时自动创建）
├── teen_cpp_student.db # 学生数据库（运行时自动创建）
├── Newtonsoft.Json.dll # 依赖库
└── System.Data.SQLite.dll
```

---

## 常见问题

### 问题1：找不到 courses 目录
**症状**：程序启动后提示课程加载失败
**解决**：确保 `courses` 文件夹与 `.exe` 在同目录，包含 `lesson01/lesson.json`

### 问题2：SQLite 相关错误
**症状**：数据库初始化失败
**解决**：安装 [SQLite ODBC Driver](https://sqlite.org/download.html)

### 问题3：字体显示不正常
**症状**：界面文字显示为方框
**解决**：确认系统安装了 "Microsoft YaHei"（微软雅黑）字体

---

## 项目结构

```
src/
├── TeenCppEdu/
│   ├── Core/
│   │   ├── Models/           # 数据模型
│   │   └── Checkers/         # 代码检查引擎
│   ├── Services/
│   │   ├── Database/         # SQLite 服务
│   │   └── ProjectGenerator/ # Dev-C++ 项目生成
│   ├── UI/
│   │   ├── Forms/            # 窗体
│   │   └── Controls/         # 自定义控件
│   ├── Program.cs            # 程序入口
│   └── TeenCppEdu.csproj     # 项目文件
courses/
└── lesson01/                 # 第一课数据
    ├── lesson.json           # 课程配置
    ├── templates/
    │   └── main.cpp          # 代码模板
    └── checks/
        └── rules.json        # 检查规则
```
