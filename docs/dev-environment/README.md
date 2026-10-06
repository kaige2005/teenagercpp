# 开发环境归档 (Development Environment)

归档 TeenC++ 开发期间使用的 Claude Code 环境配置。

**来源**：工作区 `claude-workspace/` 根目录与 `.claude/`。**目的**：清理本地工作区后，将来继续开发时能还原环境。

---

## 一、已随仓库归档（克隆即生效）

位于**仓库根目录**：

| 路径 | 用途 |
|------|------|
| `CLAUDE.md` | 项目工作规范（原在工作区根）。**注意**：其中路径按"工作区根 + 子目录"写成 `TeenagerCPlusPlusEduSystem/src/...`；若直接在本仓库内工作，路径需相应调整 |
| `.claude/settings.json` | 项目级设置（启用插件清单）。预提交 hook **已注销** |
| `.claude/hooks/pre-commit.ps1` | 预提交检查脚本：验证关键 DLL / SQLite.Interop / courses 完整性 |
| `.claude/skills/` | 项目自定义 skills：`release`、`tdd`、`precheck`、`task-with-validation`、`pd-start`、`pd-review`、`product-design-framework`、`context-aware-assistant`、`ui-ux-pro-max` |
| `.claude/RESPONSE_PATTERNS.md` | 响应模式约定 |

## 二、本目录内的归档文件

| 文件 | 说明 |
|------|------|
| `startclaude.sh` / `startclaude.bat` | 工作区启动器（2026-09-25 重写版）。**不含任何凭据**，端点/模型/密钥统一由用户级 `~/.claude/settings.json` 的 `env` 段管理 |
| `settings.local.json.example` | 原工作区 `.claude/settings.local.json` 的副本。内容仅为**权限白名单**（允许的 Bash/Read/Skill 规则），无任何密钥 |
| `claude-user-settings.template.json` | 用户级配置的**键位模板**，值为占位符 |

---

## 三、⚠️ 未能归档：用户级 `~/.claude/settings.json`

该文件的 `env` 段保存实际生效的端点、模型与密钥。

**本次归档未能读取该文件**——读取操作被 Claude Code 的权限策略拦截，因此：

- 无法确认其内容是否含凭据，**故未归档**，以免把密钥推送进这个公开仓库
- `claude-user-settings.template.json` 只列出键名，值全部是占位符
- **将来恢复环境时，请从你自己的备份或服务商处取回真实端点与密钥填入**；本仓库不提供、也无法提供这些值

> 建议把该文件的备份放在**仓库之外**的私密位置（如密码管理器），不要放进任何 git 仓库。

---

## 四、将来如何还原环境

1. 将本仓库克隆到任意路径
2. 项目级配置已随仓库就位（`.claude/`、`CLAUDE.md`），无需额外操作
3. 在本机 `~/.claude/settings.json` 中按 `claude-user-settings.template.json` 填入真实端点与密钥
4. 如需恢复预提交 hook，在 `.claude/settings.json` 中重新加入 `hooks.PreToolUse` 段（`matcher: "Bash|PowerShell"`、`if: "Bash(git commit*)"`、命令 `powershell -NoProfile -File ".claude/hooks/pre-commit.ps1"`）
5. 可选：把 `startclaude.sh` / `startclaude.bat` 放回工作目录根，按需修改其中的 `WORK_DIR`
