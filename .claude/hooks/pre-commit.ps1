# TeenC++ 预提交检查 - 关键依赖完整性
# 目的: 防止 SQLite.Interop.dll 等依赖缺失的产物进入版本
#
# 检查项 (路径已于 2026-09-25 修正到 v1.3 结构):
#   - src\TeenCppEdu\bin\Release\net48\           输出目录
#   - System.Data.SQLite.dll / Newtonsoft.Json.dll
#   - x86\SQLite.Interop.dll, x64\SQLite.Interop.dll
#   - courses\ 课程资源目录
#
# 注意: 旧版本检查的是 TeenagerCppLearning\bin\Release 与
#       WeifenLuo.WinFormsUI.Docking.dll — 两者在当前代码库中都已不存在。
#
# 输出契约: 全部通过时静默退出 0; 发现问题时输出 {"systemMessage": "..."} 并仍退出 0
#          (非阻断, 只提示)

$ErrorActionPreference = 'Continue'

$root = 'C:\Users\Carl\claude-workspace\TeenagerCPlusPlusEduSystem'
$out = Join-Path $root 'src\TeenCppEdu\bin\Release\net48'

$problems = @()

if (-not (Test-Path $out)) {
    $problems += "Release 输出目录不存在: $out  (先运行 scripts\build.ps1)"
}
else {
    foreach ($dll in @('System.Data.SQLite.dll', 'Newtonsoft.Json.dll', 'TeenCppEdu.exe')) {
        if (-not (Test-Path (Join-Path $out $dll))) {
            $problems += "缺少: $dll"
        }
    }
    foreach ($arch in @('x86', 'x64')) {
        if (-not (Test-Path (Join-Path $out "$arch\SQLite.Interop.dll"))) {
            $problems += "SQLite.Interop.dll 缺失 ($arch) - 该架构下程序无法启动"
        }
    }
    if (-not (Test-Path (Join-Path $out 'courses'))) {
        $problems += 'courses 资源目录缺失 - 课程无法加载'
    }
}

if ($problems.Count -gt 0) {
    $msg = "预提交检查未通过:`n" + (($problems | ForEach-Object { "  - $_" }) -join "`n")
    [pscustomobject]@{ systemMessage = $msg } | ConvertTo-Json -Compress
}

exit 0
