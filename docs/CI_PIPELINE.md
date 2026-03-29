# TeenC++ 教学系统 - 自动化构建流程方案

## 概述

本文档说明 TeenC++ 教学系统的自动化构建流程设计，包括脚本功能、质量门禁、以及与 CI/CD 系统的集成方案。

---

## 流程总览

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   代码提交   │ → │   构建阶段   │ → │   测试阶段   │ → │   发布阶段   │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
       │                  │                  │                  │
       ▼                  ▼                  ▼                  ▼
  Git Commit        build.ps1          dotnet test       package.ps1
  触发自动构建        编译+检查           单元测试          打包+校验
       │                  │                  │                  │
       │             质量门禁检查       覆盖率检查           版本标记
       │                  │                  │                  │
       └──────────────────┴──────────────────┴──────────────────┘
                          │
                    失败 → 阻塞流程
                    成功 → 继续下一环节
```

---

## 脚本功能详解

### 1. build.ps1 - 构建脚本

**位置**: `scripts/build.ps1`

**功能**:
| 步骤 | 功能 | 质量门禁 |
|------|------|----------|
| 1. 清理 | 可选清理旧构建产物 | - |
| 2. 还原依赖 | dotnet restore | 失败则阻塞 |
| 3. 编译 | dotnet build | 错误 > 0 阻塞 |
| 4. 单元测试 | dotnet test | 失败则阻塞 |
| 5. 质量检查 | 统计警告/错误 | 错误 > 0 阻塞，警告 > 100 告警 |
| 6. 数据同步 | 复制课程数据 | 失败则告警 |

**参数**:
```powershell
.\build.ps1 -Configuration Release   # 构建配置 (Release/Debug)
.\build.ps1 -Clean                   # 清理后构建
.\build.ps1 -SkipTests               # 跳过单元测试
```

**输出**:
```powershell
# 成功返回构建信息到 $global:BuildResult
$BuildResult = @{
    Success = $true
    Configuration = "Release"
    OutputPath = "...\bin\Release\net48"
    ExePath = "...\TeenCppEdu.exe"
    WarningCount = 0
    ErrorCount = 0
    CourseCount = 2
}
```

---

### 2. package.ps1 - 打包脚本

**位置**: `scripts/package.ps1`

**功能**:
| 步骤 | 功能 | 说明 |
|------|------|------|
| 1. 执行构建 | 调用 build.ps1 | 可选跳过 -SkipBuild |
| 2. 准备目录 | 创建发布目录 | 清理旧版本 |
| 3. 复制文件 | 复制exe、dll、课程数据 | 验证必需文件 |
| 4. 生成版本信息 | 记录版本、Git提交、构建时间 | JSON格式 |
| 5. 创建压缩包 | zip + SHA256校验和 | 便于分发 |

**参数**:
```powershell
.\package.ps1 -Version "1.2.0"                    # 必需：版本号
.\package.ps1 -Version "1.2.0-beta.1"            # 支持预发布版本
.\package.ps1 -Configuration Debug               # 构建配置
.\package.ps1 -OutputPath "C:\Releases"          # 自定义输出路径
.\package.ps1 -SkipBuild                         # 跳过构建（使用已构建的）
```

**输出**:
- `TeenCppEdu-v{版本}.zip` - 发布压缩包
- `TeenCppEdu-v{版本}.zip.sha256` - 校验和文件
- `TeenCppEdu-v{版本}/` - 解压后的发布目录
- `version.json` - 版本信息
- `TEST_REPORT.md` - 测试报告
- `*-RELEASE_NOTES-TEMPLATE.md` - 发布说明模板

---

## 质量门禁 (Quality Gates)

### 编译阶段门禁

| 检查项 | 阈值 | 失败动作 |
|--------|------|----------|
| 编译错误 | = 0 | ❌ 阻塞发布 |
| 编译警告 | ≤ 100 | ⚠️ > 100 告警（不阻塞） |
| 输出文件 | 存在 | ❌ 阻塞发布 |

### 测试阶段门禁

| 检查项 | 阈值 | 失败动作 |
|--------|------|----------|
| 单元测试通过率 | = 100% | ❌ 阻塞发布 |
| 测试失败数 | = 0 | ❌ 阻塞发布 |

### 打包阶段门禁

| 检查项 | 阈值 | 失败动作 |
|--------|------|----------|
| 必需文件 | 全部存在 | ❌ 阻塞发布 |
| 课程数据 | 完整 | ❌ 阻塞发布 |
| 版本信息 | 有效 | ❌ 阻塞发布 |

---

## 与 CI/CD 系统集成

### 方案 A：GitHub Actions (推荐)

**文件**: `.github/workflows/build.yml`

```yaml
name: Build and Test

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Build
        run: |
          cd scripts
          ./build.ps1 -Configuration Release

      - name: Upload Artifacts
        uses: actions/upload-artifact@v4
        with:
          name: build-output
          path: src/TeenCppEdu/bin/Release/net48/
```

**优势**:
- 免费 Windows 运行环境
- 与 GitHub 代码库深度集成
- 支持 PR 构建检查

---

### 方案 B：Azure DevOps Pipeline

**文件**: `azure-pipelines.yml`

```yaml
trigger:
  branches:
    include:
      - main
      - release/*

pool:
  vmImage: 'windows-latest'

variables:
  BuildConfiguration: 'Release'

steps:
  - task: PowerShell@2
    displayName: 'Build Solution'
    inputs:
      filePath: 'scripts/build.ps1'
      arguments: '-Configuration $(BuildConfiguration)'

  - task: PublishTestResults@2
    displayName: 'Publish Test Results'
    inputs:
      testResultsFormat: 'VSTest'
      testResultsFiles: '**/TestResults/*.trx'

  - task: PowerShell@2
    displayName: 'Package Release'
    inputs:
      filePath: 'scripts/package.ps1'
      arguments: '-Version $(Build.BuildNumber) -SkipBuild'

  - task: PublishBuildArtifacts@1
    displayName: 'Publish Artifacts'
    inputs:
      pathToPublish: 'releases/'
      artifactName: 'release'
```

**优势**:
- 与 Visual Studio 深度集成
- 支持 Azure Artifacts 存储
- 灵活的发布管理

---

### 方案 C：本地手动执行（当前模式）

**适用场景**:
- 个人开发
- 无互联网环境
- 快速测试

**执行流程**:
```powershell
# 1. 完整构建
cd scripts
.\build.ps1 -Configuration Release

# 2. 功能测试（手动）
..\src\TeenCppEdu\bin\Release\net48\TeenCppEdu.exe

# 3. 打包发布
.\package.ps1 -Version "1.1.0"

# 4. 分发
# 复制 releases/TeenCppEdu-v1.1.0.zip
```

---

## 版本管理流程

### 版本号格式

```
主版本.次版本.修订号[-预发布标签]

示例:
1.0.0         # 正式版
1.1.0-beta.1  # 第2课测试版
1.2.0-alpha   # 第3课开发版
2.0.0-rc.1    # 发布候选版
```

### 版本升级规则

| 场景 | 版本变更 | 示例 |
|------|----------|------|
| 新增功能（第3课） | 次版本 + 1 | 1.1.0 → 1.2.0 |
| Bug 修复 | 修订号 + 1 | 1.1.0 → 1.1.1 |
| 重大架构变更 | 主版本 + 1 | 1.x.x → 2.0.0 |
| 测试版本 | 添加标签 | 1.1.0-beta.1 |

---

## 快速参考

### 常用命令

```powershell
# 开发调试构建
.\scripts\build.ps1 -Configuration Debug

# 正式发布构建
.\scripts\build.ps1 -Configuration Release

# 清理并重新构建
.\scripts\build.ps1 -Configuration Release -Clean

# 仅打包（使用已构建的）
.\scripts\package.ps1 -Version "1.1.0" -SkipBuild

# 完整打包流程
.\scripts\package.ps1 -Version "1.1.0-beta.1"
```

### 目录结构

```
TeenCppEdu/
├── scripts/
│   ├── build.ps1           # 构建脚本
│   └── package.ps1         # 打包脚本
├── src/
│   ├── TeenCppEdu/         # 主项目
│   └── TeenCppEdu.Tests/   # 测试项目
├── courses/                # 课程数据
├── releases/               # 发布包（自动生成）
│   ├── TeenCppEdu-v1.1.0/
│   ├── TeenCppEdu-v1.1.0.zip
│   └── TeenCppEdu-v1.1.0-RELEASE_NOTES.md
├── docs/
│   ├── WORKFLOW.md         # 工作流程
│   └── CI_PIPELINE.md      # 本文档
└── README.md
```

---

## 故障排查

### 常见问题

| 问题 | 原因 | 解决方案 |
|------|------|----------|
| "解决方案文件不存在" | 执行目录不对 | 在 scripts 目录执行 |
| "编译错误" | 代码语法问题 | 查看详细错误日志 |
| "单元测试失败" | 逻辑变更影响测试 | 更新测试用例 |
| "课程数据缺失" | courses 目录未复制 | 检查课程文件位置 |
| "权限被拒绝" | 文件占用 | 关闭运行的程序后重试 |

### 调试构建

```powershell
# 详细输出
$ErrorActionPreference = "Continue"
.\scripts\build.ps1 -Verbose

# 检查构建结果
$global:BuildResult

# 手动执行步骤
cd src
dotnet restore
dotnet build -c Release
dotnet test --verbosity diagnostic
```

---

## 扩展建议

### 短期优化

1. **添加代码覆盖率检查**
   - 使用 coverlet 生成覆盖率报告
   - 设置门禁阈值 (≥ 70%)

2. **添加静态代码分析**
   - 使用 Roslyn analyzers
   - 集成 CodeQL 安全扫描

3. **自动化版本号管理**
   - 根据 Git 标签自动获取版本
   - 自动生成 CHANGELOG

### 长期优化

1. **CI/CD 自动化部署**
   - 每次 PR 自动运行测试
   - 合并到 main 自动打包
   - 发布 Release 自动上传

2. **多环境发布**
   - 开发环境 (dev)
   - 测试环境 (staging)
   - 生产环境 (production)

3. **制品库集成**
   - Azure Artifacts
   - GitHub Packages
   - NuGet 包管理

---

*最后更新: 2026-03-22*
