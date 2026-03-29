# .NET 8 SDK 安装脚本
# 适用于 Windows

param(
    [string]$InstallDir = "$env:LOCALAPPDATA\Microsoft\dotnet",
    [switch]$Force
)

$ErrorActionPreference = "Stop"

Write-Host "=== .NET 8 SDK 安装脚本 ===" -ForegroundColor Cyan
Write-Host ""

# 检查是否已安装
dotnet --version 2>$null
if ($LASTEXITCODE -eq 0 -and -not $Force) {
    $existingVersion = dotnet --version
    Write-Host ".NET SDK 已安装: $existingVersion" -ForegroundColor Green
    Write-Host "使用 -Force 参数强制重新安装" -ForegroundColor Yellow
    exit 0
}

# 下载链接
$DownloadUrl = "https://download.visualstudio.microsoft.com/download/pr/" +
               "7a0d1cd9-5e0a-4f0e-9f1e-7c3c8c8c8c8c/" +
               "dotnet-sdk-8.0.100-win-x64.exe"

# 官方安装脚本（更可靠）
$InstallScriptUrl = "https://dot.net/v1/dotnet-install.ps1"

Write-Host "正在下载 .NET 安装脚本..." -ForegroundColor Cyan
Write-Host "来源: $InstallScriptUrl"

try {
    $ScriptPath = "$env:TEMP\dotnet-install.ps1"

    # 使用 Invoke-WebRequest 下载（带进度）
    if ($PSVersionTable.PSVersion.Major -ge 5) {
        $ProgressPreference = 'Continue'
        Invoke-WebRequest -Uri $InstallScriptUrl -OutFile $ScriptPath -UseBasicParsing
    } else {
        # 旧版 PowerShell 回退
        (New-Object System.Net.WebClient).DownloadFile($InstallScriptUrl, $ScriptPath)
    }

    Write-Host "安装脚本下载成功！" -ForegroundColor Green
    Write-Host ""

    # 执行安装
    Write-Host "正在安装 .NET 8 SDK..." -ForegroundColor Cyan
    Write-Host "安装路径: $InstallDir" -ForegroundColor Gray
    Write-Host "这可能需要几分钟，请耐心等待..." -ForegroundColor Yellow
    Write-Host ""

    & $ScriptPath -Channel 8.0 -InstallDir $InstallDir -NoPath

    if ($LASTEXITCODE -ne 0) {
        throw "安装脚本返回错误代码: $LASTEXITCODE"
    }

    Write-Host ""
    Write-Host "=== 安装成功！===" -ForegroundColor Green
    Write-Host ""

    # 添加到 PATH
    $UserPath = [Environment]::GetEnvironmentVariable("PATH", "User")
    if ($UserPath -notlike "*$InstallDir*") {
        Write-Host "添加 $InstallDir 到用户 PATH..." -ForegroundColor Cyan
        [Environment]::SetEnvironmentVariable(
            "PATH",
            "$UserPath;$InstallDir",
            "User"
        )
        Write-Host "PATH 已更新！请重新打开终端使更改生效。" -ForegroundColor Yellow
    }

    # 验证安装
    Write-Host ""
    Write-Host "验证安装..." -ForegroundColor Cyan

    $env:PATH = "$env:PATH;$InstallDir"
    $Version = & "$InstallDir\dotnet.exe" --version 2>&1

    if ($LASTEXITCODE -eq 0) {
        Write-Host ".NET SDK 版本: $Version" -ForegroundColor Green
        Write-Host ""
        Write-Host "可用命令:" -ForegroundColor Cyan
        Write-Host "  dotnet --version    - 查看版本"
        Write-Host "  dotnet --list-sdks  - 列出所有 SDK"
        Write-Host "  dotnet build        - 编译项目"
        Write-Host ""
    } else {
        Write-Host "安装完成，但验证失败。请手动运行 'dotnet --version'" -ForegroundColor Yellow
    }

} catch {
    Write-Host ""
    Write-Host "=== 安装失败 ===" -ForegroundColor Red
    Write-Host "错误: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "替代方案：手动下载安装" -ForegroundColor Cyan
    Write-Host "1. 访问: https://dotnet.microsoft.com/download/dotnet/8.0"
    Write-Host "2. 下载 .NET 8.0 SDK (x64)"
    Write-Host "3. 运行安装程序"
    exit 1
}
