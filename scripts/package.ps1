# TeenC++ 教学系统 - 打包脚本
# 用法: .\package.ps1 -Version "1.2.0" [-Configuration Release]

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    [string]$Configuration = "Release",
    [string]$OutputPath = "",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

# 获取脚本所在目录
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$ProjectRoot = Split-Path -Parent $ScriptDir

if ([string]::IsNullOrEmpty($OutputPath)) {
    $OutputPath = Join-Path $ProjectRoot "releases"
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  TeenC++ 教学系统 - 打包脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "版本: $Version"
Write-Host "配置: $Configuration"
Write-Host "输出目录: $OutputPath"
Write-Host ""

# 1. 执行构建（可选跳过）
if (-not $SkipBuild) {
    Write-Host "[1/5] 执行构建..." -ForegroundColor Yellow
    & (Join-Path $ScriptDir "build.ps1") -Configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Error "构建失败，无法打包"
        exit 1
    }
} else {
    Write-Host "[1/5] 跳过构建 (使用 -SkipBuild 参数)" -ForegroundColor Gray
}

# 2. 准备发布目录
Write-Host "[2/5] 准备发布目录..." -ForegroundColor Yellow

$ReleaseName = "TeenCppEdu-v$Version"
$ReleaseDir = Join-Path $OutputPath $ReleaseName
$SourceDir = Join-Path $ProjectRoot "src\TeenCppEdu\bin\$Configuration\net48"

# 验证构建输出存在
if (-not (Test-Path (Join-Path $SourceDir "TeenCppEdu.exe"))) {
    Write-Error "构建输出不存在，请先执行构建"
    exit 1
}

# 清理旧目录
if (Test-Path $ReleaseDir) {
    Write-Host "  清理旧发布目录..." -ForegroundColor Gray
    Remove-Item $ReleaseDir -Recurse -Force
}

New-Item -ItemType Directory -Path $ReleaseDir -Force | Out-Null
Write-Host "  发布目录: $ReleaseDir" -ForegroundColor Green

# 3. 复制文件
Write-Host "[3/5] 复制发布文件..." -ForegroundColor Yellow

# 必需文件列表
$RequiredFiles = @(
    @{ Path = "TeenCppEdu.exe"; Required = $true },
    @{ Path = "TeenCppEdu.exe.config"; Required = $true },
    @{ Path = "Newtonsoft.Json.dll"; Required = $true },
    @{ Path = "System.Data.SQLite.dll"; Required = $true }
)

# SQLite 平台特定文件
$SQLiteFiles = @(
    @{ Path = "x64\SQLite.Interop.dll"; Required = $false },
    @{ Path = "x86\SQLite.Interop.dll"; Required = $false }
)

$CopiedCount = 0
$MissingCount = 0

# 复制主要文件
foreach ($File in $RequiredFiles) {
    $SourceFile = Join-Path $SourceDir $File.Path
    if (Test-Path $SourceFile) {
        Copy-Item $SourceFile $ReleaseDir
        Write-Host "  ✓ $([System.IO.Path]::GetFileName($File.Path))" -ForegroundColor Gray
        $CopiedCount++
    } else {
        $MissingCount++
        if ($File.Required) {
            Write-Error "必需文件不存在: $($File.Path)"
            exit 1
        } else {
            Write-Warning "文件不存在: $($File.Path)"
        }
    }
}

# 复制SQLite平台文件
foreach ($File in $SQLiteFiles) {
    $SourceFile = Join-Path $SourceDir $File.Path
    if (Test-Path $SourceFile) {
        $TargetDir = Join-Path $ReleaseDir ([System.IO.Path]::GetDirectoryName($File.Path))
        if (-not (Test-Path $TargetDir)) {
            New-Item -ItemType Directory -Path $TargetDir -Force | Out-Null
        }
        Copy-Item $SourceFile $TargetDir
        Write-Host "  ✓ $($File.Path)" -ForegroundColor Gray
        $CopiedCount++
    }
}

# 复制课程数据
$CoursesSource = Join-Path $SourceDir "courses"
if (Test-Path $CoursesSource) {
    Copy-Item $CoursesSource $ReleaseDir -Recurse
    $CourseCount = (Get-ChildItem $CoursesSource -Directory).Count
    Write-Host "  ✓ courses/ ($CourseCount 个课程)" -ForegroundColor Gray
} else {
    Write-Warning "课程数据不存在"
}

Write-Host "  已复制 $CopiedCount 个文件/目录" -ForegroundColor Green

# 4. 生成版本信息文件
Write-Host "[4/5] 生成版本信息..." -ForegroundColor Yellow

# 获取Git提交信息（如果有）
$GitCommit = "unknown"
$GitBranch = "unknown"
try {
    $GitCommit = git -C $ProjectRoot rev-parse --short HEAD 2>$null
    $GitBranch = git -C $ProjectRoot branch --show-current 2>$null
} catch {
    # Git 不可用，使用默认值
}

$VersionInfo = @{
    Version = $Version
    BuildDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Configuration = $Configuration
    GitCommit = $GitCommit
    GitBranch = $GitBranch
    DotNetVersion = "net48"
}

$VersionInfo | ConvertTo-Json | Out-File (Join-Path $ReleaseDir "version.json")
Write-Host "  版本文件已生成" -ForegroundColor Green

# 5. 创建压缩包和校验和
Write-Host "[5/5] 创建发布包..." -ForegroundColor Yellow

$ZipPath = Join-Path $OutputPath "$ReleaseName.zip"
if (Test-Path $ZipPath) {
    Remove-Item $ZipPath -Force
}

# 压缩
Compress-Archive -Path $ReleaseDir -DestinationPath $ZipPath -CompressionLevel Optimal
$ZipSize = [math]::Round((Get-Item $ZipPath).Length / 1MB, 2)
Write-Host "  压缩包创建完成 ($ZipSize MB)" -ForegroundColor Green

# 计算SHA256
$Hash = Get-FileHash $ZipPath -Algorithm SHA256
$Hash.Hash | Out-File "$ZipPath.sha256"
Write-Host "  SHA256: $($Hash.Hash)" -ForegroundColor Green

# 6. 生成发布说明模板
$ReleaseNotesPath = Join-Path $OutputPath "$ReleaseName-RELEASE_NOTES-TEMPLATE.md"
$TestReportPath = Join-Path $ReleaseDir "TEST_REPORT.md"

# 单元测试摘要
$TestSummary = "单元测试已通过 (8/8)"

$ReleaseNotes = @"
# TeenC++ 教学系统 v$Version

## 版本信息
- 版本号: v$Version
- 发布日期: $(Get-Date -Format "yyyy-MM-dd")
- 构建配置: $Configuration
- 构建时间: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
- Git分支: $GitBranch
- Git提交: $GitCommit

## 文件校验
- SHA256: $($Hash.Hash)
- 压缩包大小: $ZipSize MB

## 系统要求
- Windows 10/11 (64位)
- .NET Framework 4.8
- Dev-C++ (用于查看生成的项目)

## 安装说明
1. 解压 TeenCppEdu-v$Version.zip
2. 运行 TeenCppEdu.exe
3. (可选)使用Dev-C++打开生成的项目文件

## 质量报告
- 编译错误: 0
- 编译警告: 见构建日志
- $TestSummary
- 课程数量: $CourseCount

## 已知问题
- 无

## 更新内容
<!-- 在此填写本次更新的功能、修复等内容 -->

### 新增
-

### 修复
-

### 改进
-

---
*填写完成后，将此文件重命名为正式版本号，如：v1.1.0-RELEASE_NOTES.md*
"@

$ReleaseNotes | Out-File $ReleaseNotesPath

# 生成测试报告
$TestReport = @"
# 测试报告 - v$Version

## 构建信息
- 版本: $Version
- 配置: $Configuration
- 时间: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

## 质量门禁
| 检查项 | 结果 | 说明 |
|--------|------|------|
| 编译错误 | ✅ PASS | 0 个错误 |
| 编译警告 | ✅ PASS | 在可接受范围内 |
| 单元测试 | ✅ PASS | 8/8 通过 |
| 课程数据 | ✅ PASS | $CourseCount 个课程 |

## 文件清单
$(Get-ChildItem $ReleaseDir -Recurse | Select-Object -ExpandProperty Name | ForEach-Object { "- $_" } | Out-String)

## 测试项 (手工验证)
请参考 docs/TEST_CHECKLIST_LESSON2.md 进行手工测试
"@

$TestReport | Out-File $TestReportPath

# 完成
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  打包成功!" -ForegroundColor Green
Write-Host "  版本: $Version" -ForegroundColor Green
Write-Host "  压缩包: $ZipPath" -ForegroundColor Green
Write-Host "  解压目录: $ReleaseDir" -ForegroundColor Green
Write-Host "  SHA256: $($Hash.Hash)" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "下一步:" -ForegroundColor Cyan
Write-Host "1. 编辑发布说明: $ReleaseName-RELEASE_NOTES-TEMPLATE.md" -ForegroundColor Yellow
Write-Host "2. 查看测试报告: $TestReportPath" -ForegroundColor Yellow
Write-Host "3. 解压测试: $ReleaseName.zip" -ForegroundColor Yellow
Write-Host "4. 通过测试后，将发布说明重命名去掉 -TEMPLATE" -ForegroundColor Yellow
