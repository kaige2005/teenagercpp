# TeenC++ 发布脚本 v1.0
# 强制验证发布包完整性

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,

    [Parameter(Mandatory=$false)]
    [switch]$SkipVerification = $false
)

$ErrorActionPreference = "Stop"
$SourceDir = "..\src\TeenCppEdu\bin\Release\net48"
$ReleaseDir = "..\releases\$Version"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TeenC++ 发布脚本 v1.0" -ForegroundColor Cyan
Write-Host "版本: $Version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# ========== Step 1: 编译检查 ==========
Write-Host "`n[Step 1/5] 编译检查..." -ForegroundColor Yellow

try {
    Push-Location "..\src"
    $buildOutput = dotnet build TeenCppEdu\TeenCppEdu.csproj -c Release 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Error "编译失败！"
        exit 1
    }
    Write-Host "✓ 编译通过 (0 error)" -ForegroundColor Green
    Pop-Location
} catch {
    Write-Error "编译异常: $_"
    exit 1
}

# ========== Step 2: 单元测试 ==========
Write-Host "`n[Step 2/5] 单元测试..." -ForegroundColor Yellow

try {
    Push-Location "..\src"
    $testOutput = dotnet test TeenCppEdu.Tests\TeenCppEdu.Tests.csproj --verbosity minimal 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Error "单元测试失败！"
        exit 1
    }
    Write-Host "✓ 单元测试通过" -ForegroundColor Green
    Pop-Location
} catch {
    Write-Error "测试异常: $_"
    exit 1
}

# ========== Step 3: 发布包完整性检查 ==========
Write-Host "`n[Step 3/5] 发布包完整性检查..." -ForegroundColor Yellow

$RequiredFiles = @(
    "TeenCppEdu.exe",
    "TeenCppEdu.exe.config",
    "Newtonsoft.Json.dll",
    "System.Data.SQLite.dll",
    "x64\SQLite.Interop.dll",
    "x86\SQLite.Interop.dll"
)

$RequiredCourses = @(
    "courses\l01\lesson.json",
    "courses\l01\templates\main.cpp",
    "courses\l01\checks\rules.json",
    "courses\l02\lesson.json",
    "courses\l02\templates\main.cpp",
    "courses\l02\checks\rules.json",
    "courses\l03\lesson.json",
    "courses\l03\templates\main.cpp",
    "courses\l03\checks\rules.json"
)

# 检查源目录文件是否存在
$MissingFiles = @()
foreach ($file in $RequiredFiles) {
    $fullPath = Join-Path $SourceDir $file
    if (-not (Test-Path $fullPath)) {
        $MissingFiles += $file
    }
}

if ($MissingFiles.Count -gt 0) {
    Write-Error "编译输出缺少以下文件:`n$($MissingFiles -join "`n")"
    exit 1
}

Write-Host "✓ 编译输出完整" -ForegroundColor Green

# 检查课程数据
foreach ($course in $RequiredCourses) {
    $fullPath = Join-Path "..\courses" $course
    if (-not (Test-Path $fullPath)) {
        Write-Error "课程数据缺失: $course"
        exit 1
    }
}

Write-Host "✓ 课程数据完整" -ForegroundColor Green

# ========== Step 4: 复制发布文件 ==========
Write-Host "`n[Step 4/5] 创建发布包..." -ForegroundColor Yellow

if (Test-Path $ReleaseDir) {
    Remove-Item -Recurse -Force $ReleaseDir
}

New-Item -ItemType Directory -Path $ReleaseDir | Out-Null

# 复制核心文件
Copy-Item "$SourceDir\TeenCppEdu.exe" $ReleaseDir
Copy-Item "$SourceDir\TeenCppEdu.exe.config" $ReleaseDir
Copy-Item "$SourceDir\Newtonsoft.Json.dll" $ReleaseDir
Copy-Item "$SourceDir\System.Data.SQLite.dll" $ReleaseDir
Copy-Item "$SourceDir\*.pdb" $ReleaseDir -ErrorAction SilentlyContinue

# 复制架构文件夹
Copy-Item "$SourceDir\x64" $ReleaseDir -Recurse
Copy-Item "$SourceDir\x86" $ReleaseDir -Recurse

# 复制课程数据
Copy-Item "..\courses" $ReleaseDir -Recurse

# 复制文档
if (Test-Path "..\docs\README.md") {
    Copy-Item "..\docs\README.md" $ReleaseDir
}

Write-Host "✓ 发布包创建完成: $ReleaseDir" -ForegroundColor Green

# ========== Step 5: 最终验证 ==========
Write-Host "`n[Step 5/5] 最终验证..." -ForegroundColor Yellow

$VerificationErrors = @()

foreach ($file in $RequiredFiles) {
    $fullPath = Join-Path $ReleaseDir $file
    if (-not (Test-Path $fullPath)) {
        $VerificationErrors += "发布包缺少: $file"
    }
}

foreach ($course in $RequiredCourses) {
    $fullPath = Join-Path $ReleaseDir $course
    if (-not (Test-Path $fullPath)) {
        $VerificationErrors += "发布包缺少课程: $course"
    }
}

# 验证EXE可运行（检查PE头）
$exePath = Join-Path $ReleaseDir "TeenCppEdu.exe"
if (Test-Path $exePath) {
    $bytes = [System.IO.File]::ReadAllBytes($exePath)
    $peHeader = [System.BitConverter]::ToUInt16($bytes, 60)
    $machine = [System.BitConverter]::ToUInt16($bytes, $peHeader + 4)
    if ($machine -eq 0x14C -or $machine -eq 0x8664) {
        Write-Host "✓ EXE文件PE头验证通过" -ForegroundColor Green
    } else {
        $VerificationErrors += "EXE文件PE头异常"
    }
}

if ($VerificationErrors.Count -gt 0) {
    Write-Error "验证失败:`n$($VerificationErrors -join "`n")"
    exit 1
}

# ========== 生成发布报告 ==========
$Report = @"
# TeenC++ 发布报告

## 版本信息
- 版本: $Version
- 构建时间: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
- 构建配置: Release

## 验证结果
- [x] 编译通过 (0 error)
- [x] 单元测试通过
- [x] 文件完整性检查
- [x] 课程数据完整性
- [x] 发布包创建成功

## 文件清单
$(Get-ChildItem $ReleaseDir -Recurse | Select-Object -ExpandProperty FullName | ForEach-Object { "- $_" } | Out-String)

## 发布包大小
$(Get-ChildItem $ReleaseDir -Recurse | Measure-Object -Property Length -Sum | Select-Object -ExpandProperty Sum | ForEach-Object { "{0:N2} MB" -f ($_ / 1MB) })

---
Generated by publish.ps1 v1.0
"@

$ReportPath = Join-Path $ReleaseDir "RELEASE_REPORT.md"
$Report | Out-File -FilePath $ReportPath -Encoding UTF8

# ========== 完成 ==========
Write-Host "`n========================================" -ForegroundColor Green
Write-Host "🎉 发布成功！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "版本: $Version" -ForegroundColor White
Write-Host "位置: $ReleaseDir" -ForegroundColor White
Write-Host "报告: $ReportPath" -ForegroundColor White
Write-Host "`n发布前检查清单:" -ForegroundColor Yellow
Write-Host "□ 在干净环境测试运行" -ForegroundColor Gray
Write-Host "□ 验证L01/L02/L03课程加载" -ForegroundColor Gray
Write-Host "□ 执行用户测试" -ForegroundColor Gray

exit 0
