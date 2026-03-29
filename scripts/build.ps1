# TeenC++ 教学系统 - 构建脚本
# 用法: .\build.ps1 [-Configuration Release|Debug] [-Clean] [-SkipTests]

param(
    [string]$Configuration = "Release",
    [switch]$Clean,
    [switch]$SkipTests
)

$ErrorActionPreference = "Stop"

# 获取脚本所在目录，确保在任何位置执行都能正确找到项目
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$ProjectRoot = Split-Path -Parent $ScriptDir
$SolutionPath = Join-Path $ProjectRoot "src\TeenCppEdu.sln"
$TestProjectPath = Join-Path $ProjectRoot "src\TeenCppEdu.Tests\TeenCppEdu.Tests.csproj"
$MainProjectPath = Join-Path $ProjectRoot "src\TeenCppEdu\TeenCppEdu.csproj"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  TeenC++ 教学系统 - 构建脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "配置: $Configuration"
Write-Host "项目根目录: $ProjectRoot"
Write-Host ""

# 验证路径
if (-not (Test-Path $SolutionPath)) {
    Write-Error "解决方案文件不存在: $SolutionPath"
    exit 1
}

# 1. 清理
if ($Clean) {
    Write-Host "[1/6] 清理解决方案..." -ForegroundColor Yellow
    dotnet clean "$SolutionPath" -c $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Error "清理失败"
        exit 1
    }
    Write-Host "  清理完成" -ForegroundColor Green
} else {
    Write-Host "[1/6] 跳过清理 (使用 -Clean 参数启用)" -ForegroundColor Gray
}

# 2. 还原依赖
Write-Host "[2/6] 还原 NuGet 依赖..." -ForegroundColor Yellow
dotnet restore "$SolutionPath" --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Error "依赖还原失败"
    exit 1
}
Write-Host "  依赖还原完成" -ForegroundColor Green

# 3. 编译
Write-Host "[3/6] 编译解决方案..." -ForegroundColor Yellow
$BuildOutput = dotnet build "$SolutionPath" -c $Configuration --no-restore --verbosity normal 2>&1
$BuildExitCode = $LASTEXITCODE

# 统计错误和警告
$ErrorCount = ($BuildOutput | Select-String ": error").Count
$WarningCount = ($BuildOutput | Select-String ": warning").Count

if ($BuildExitCode -ne 0 -or $ErrorCount -gt 0) {
    Write-Error "编译失败 ($ErrorCount 个错误, $WarningCount 个警告)"
    $BuildOutput | Where-Object { $_ -match ": error" } | ForEach-Object {
        Write-Host "  ERROR: $_" -ForegroundColor Red
    }
    exit 1
}
Write-Host "  编译完成 (0错误, $WarningCount 警告)" -ForegroundColor Green

# 4. 运行测试
if (-not $SkipTests) {
    Write-Host "[4/6] 运行单元测试..." -ForegroundColor Yellow
    if (Test-Path $TestProjectPath) {
        dotnet test "$TestProjectPath" -c $Configuration --no-build --verbosity minimal
        if ($LASTEXITCODE -ne 0) {
            Write-Error "单元测试失败"
            exit 1
        }
        Write-Host "  单元测试通过" -ForegroundColor Green
    } else {
        Write-Host "  测试项目不存在，跳过" -ForegroundColor Yellow
    }
} else {
    Write-Host "[4/6] 跳过测试 (使用 -SkipTests 参数)" -ForegroundColor Gray
}

# 5. 质量门禁检查
Write-Host "[5/6] 质量门禁检查..." -ForegroundColor Yellow

$OutputDir = Join-Path $ProjectRoot "src\TeenCppEdu\bin\$Configuration\net48"
$ExePath = Join-Path $OutputDir "TeenCppEdu.exe"

# 检查输出文件存在
if (-not (Test-Path $ExePath)) {
    Write-Error "输出文件不存在: $ExePath"
    exit 1
}

# 获取文件信息
$FileInfo = Get-Item $ExePath
Write-Host "  - 输出文件: $($FileInfo.Name)" -ForegroundColor Green
Write-Host "  - 文件大小: $([math]::Round($FileInfo.Length / 1KB, 2)) KB" -ForegroundColor Green
Write-Host "  - 修改时间: $($FileInfo.LastWriteTime)" -ForegroundColor Green

# 质量门禁 - 编译错误
if ($ErrorCount -gt 0) {
    Write-Error "[质量门禁] 编译错误数 > 0"
    exit 1
}

# 质量门禁 - 警告数（黄色警告，不阻塞）
if ($WarningCount -gt 100) {
    Write-Warning "[质量门禁] 警告数过多 ($WarningCount)，建议清理"
} elseif ($WarningCount -gt 0) {
    Write-Host "  [质量门禁] 警告数: $WarningCount (建议后续清理)" -ForegroundColor Yellow
} else {
    Write-Host "  [质量门禁] 无编译警告 ✓" -ForegroundColor Green
}

# 6. 复制课程数据
Write-Host "[6/6] 同步课程数据..." -ForegroundColor Yellow
$CoursesSource = Join-Path $ProjectRoot "courses"
$CoursesTarget = Join-Path $OutputDir "courses"

if (Test-Path $CoursesSource) {
    if (-not (Test-Path $CoursesTarget)) {
        New-Item -ItemType Directory -Path $CoursesTarget -Force | Out-Null
    }

    # 使用 robocopy 进行增量复制
    $CourseCount = (Get-ChildItem $CoursesSource -Directory).Count
    robocopy "$CoursesSource" "$CoursesTarget" /E /NFL /NDL /NJH /NJS
    Write-Host "  课程数据同步完成 ($CourseCount 个课程)" -ForegroundColor Green
} else {
    Write-Warning "课程数据源目录不存在: $CoursesSource"
}

# 7. 验证课程数据完整性
Write-Host ""
Write-Host "[验证] 课程数据完整性检查..." -ForegroundColor Yellow
$CourseDirs = Get-ChildItem $CoursesTarget -Directory -ErrorAction SilentlyContinue
$ValidCourses = 0
foreach ($Dir in $CourseDirs) {
    $LessonJson = Join-Path $Dir.FullName "lesson.json"
    $TemplateDir = Join-Path $Dir.FullName "templates"
    $CheckDir = Join-Path $Dir.FullName "checks"

    $HasLesson = Test-Path $LessonJson
    $HasTemplate = Test-Path $TemplateDir
    $HasCheck = Test-Path $CheckDir

    if ($HasLesson -and $HasTemplate -and $HasCheck) {
        Write-Host "  ✓ $($Dir.Name)" -ForegroundColor Green
        $ValidCourses++
    } else {
        Write-Host "  ✗ $($Dir.Name) (缺失文件)" -ForegroundColor Red
    }
}
Write-Host "  有效课程: $ValidCourses" -ForegroundColor Green

# 完成
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  构建成功!" -ForegroundColor Green
Write-Host "  输出路径: $OutputDir" -ForegroundColor Green
Write-Host "  可执行文件: TeenCppEdu.exe" -ForegroundColor Green
Write-Host "  版本: $((Get-Item $ExePath).VersionInfo.ProductVersion)" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

# 返回构建信息（可用于自动化）
$global:BuildResult = @{
    Success = $true
    Configuration = $Configuration
    OutputPath = $OutputDir
    ExePath = $ExePath
    WarningCount = $WarningCount
    ErrorCount = $ErrorCount
    CourseCount = $ValidCourses
}
