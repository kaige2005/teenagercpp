#!/bin/bash
# ====================== 配置区（仅需修改这里） ======================
WORK_DIR="/c/Users/Carl/claude-workspace"  # 工作目录
# ====================================================================
# 说明 (2026-09-25 重写):
#   端点 / 模型 / 密钥 统一由 ~/.claude/settings.json 的 env 段管理，本脚本
#   不再设置任何凭据。
#
#   旧版本会 export 已失效的 GLM 端点与密钥，有三个致命问题：
#     1. ANTHROPIC_API_KEY 被覆盖成已废弃的 key，导致启动后无法认证
#     2. ANTHROPIC_API_BASE_URL 变量名写错（正确是 ANTHROPIC_BASE_URL），从未生效
#     3. 导出 CLAUDE_API_PROVIDER=openai —— Claude Code 2.x 不支持该变量，
#        且它会残留在用户环境里，是"启动报错"的来源之一
#   另外旧版最后执行的是 `claude chat`，这不是合法命令。
# ====================================================================

echo "========================================"
echo "             Claude Code 启动器            "
echo "========================================"

if [ ! -d "$WORK_DIR" ]; then
    echo "[信息] 工作目录不存在，自动创建：$WORK_DIR"
    mkdir -p "$WORK_DIR"
fi

cd "$WORK_DIR" || {
    echo "[致命错误] 切换目录失败：$WORK_DIR"
    read -n 1 -s -r -p "按任意键退出..."
    exit 1
}
echo "[成功] 工作目录：$(pwd)"

echo "[信息] 当前生效的端点与模型（来自 ~/.claude/settings.json）："
grep -E '"(ANTHROPIC_BASE_URL|ANTHROPIC_MODEL)"' "$HOME/.claude/settings.json" 2>/dev/null \
    | sed -E 's/^[[:space:]]*/    /'
echo ""

echo "========================================"
echo "          正在启动 Claude Code...          "
echo "========================================"

claude

echo "========================================"
echo "          执行完成（窗口不关闭）           "
echo "========================================"
read -n 1 -s -r -p "按任意键退出..."
