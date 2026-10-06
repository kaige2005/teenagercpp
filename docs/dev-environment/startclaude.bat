@echo off
REM ============================================================
REM  Claude Code 启动器 (2026-09-25 重写)
REM
REM  端点 / 模型 / 密钥 统一由 %USERPROFILE%\.claude\settings.json
REM  的 env 段管理，本脚本不再设置任何凭据。
REM
REM  旧版本在这里 set 了已失效的 GLM 端点与密钥，会覆盖正确配置；
REM  且导出的 CLAUDE_API_PROVIDER=openai 不是合法变量，会残留污染环境。
REM  另外旧版最后执行的是 `claude chat`，不是合法命令。
REM ============================================================

cd /d "%USERPROFILE%\claude-workspace"

echo ========================================
echo             Claude Code 启动器
echo ========================================
echo [信息] 工作目录: %CD%
echo.

claude

pause
