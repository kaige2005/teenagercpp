# TeenC++ 教学系统 - Claude Code 工作规范

> **归档说明（2026-10-06）**：本文件原位于工作区根目录 `claude-workspace/CLAUDE.md`，
> 项目终止时随开发环境一并移入本仓库。文中文件路径按"工作区根 + `TeenagerCPlusPlusEduSystem/`"
> 编写；若直接在本仓库内工作，路径需相应去掉 `TeenagerCPlusPlusEduSystem/` 前缀。
> 项目状态与归档资料见 [README](README.md) 与 [docs/dev-environment/](docs/dev-environment/README.md)。

## 场景化自动触发

您不需要记住命令。Claude会在以下场景**自动建议**合适的工具：

| 您的表达 | 自动建议 |
|---------|-----------|
| "实现...功能" / "修复bug" | 💡 建议 `/tdd` + `/task-v` |
| "写完了" / "完成了" | ⏸️ 验证清单检查 |
| "发布" / "打包版本" | 🚀 建议 `/release` 流程 |
| "生成C#代码" / "写代码" | ⚠️ .NET 4.8兼容性提醒 |
| "出错了/崩溃" | 🔧 建议 `systematic-debugging` |
| "设计界面" | 🎨 建议 `frontend-design` |
| **"设计课程" / "产品规划"** | 🎯 **建议 `/pd-start` 设计流程** |

**控制方式**: 见建议后直接说"好"、"执行"或忽略即可。

---

## 产品设计工作流 (新增)

### 快速启动

```
/pd-start <主题>              # 启动完整设计流程
/pd-start <主题> --phase=all  # 同上
/pd-start <主题> --phase=research  # 仅调研
/pd-start <主题> --phase=review    # 仅评审现有设计
/pd-review <文档路径>          # 设计评审验证
```

### 4-Phase 设计流程

```
┌─────────────────────────────────────────────────────────────────┐
│                    产品设计阶段 Workflow                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Phase 1: 调研分析 (Research)          [Researcher Agent]        │
│  ├── 用户研究: 画像/需求/痛点                                     │
│  ├── 竞品分析: 功能/差异化机会                                    │
│  └── 产出: 调研报告 → docs/design/research/                       │
│                          ↓ [确认后继续]                           │
│  Phase 2: 概念设计 (Concept)           [Architect + Storyteller] │
│  ├── 信息架构: 功能模块/数据模型                                  │
│  ├── 故事线: 世界观/角色/剧情                                     │
│  └── 产出: 概念文档 → docs/design/concept/                        │
│                          ↓ [确认后继续]                           │
│  Phase 3: 详细设计 (Detail)            [UX-Designer + Content]   │
│  ├── 交互设计: 流程/布局/UI (调用frontend-design)                 │
│  ├── 内容设计: 文案/测验/代码                                     │
│  └── 产出: PRD → docs/design/prd/                                │
│                          ↓ [确认后继续]                           │
│  Phase 4: 评审验证 (Review)            [Reviewer + Validator]    │
│  ├── 设计质量: 教育/游戏化/UX/技术/一致性                         │
│  ├── 问题清单: P0/P1/P2分级                                       │
│  └── 产出: 评审报告 → docs/design/review/                         │
│                          ↓                                       │
│                    ✅ 设计冻结 → 进入开发                         │
└─────────────────────────────────────────────────────────────────┘
```

### Agent 协作网络

```
用户需求
   ↓
/pd-start
   ↓
┌──────────┬──────────┬──────────┬──────────┐
│Researcher│Architect │UX-Design │Reviewer  │
│  调研    │  架构    │  UI设计  │  评审    │
│          │          │          │          │
│用户画像  │信息架构  │界面布局  │质量检查  │
│竞品分析  │数据模型  │交互原型  │问题清单  │
│需求排序  │技术路线  │微动效    │通过决议  │
└──────────┴──────────┴────┬─────┴──────────┘
                           │
                    Storyteller + Content
                    故事线 + 课程内容
                           │
                    frontend-design
                    视觉设计优化
                           │
                    superpowers:review
                    质量验证
                           ↓
                      设计冻结
```

### 使用示例

**场景1: 新课程设计**
```
用户: 设计第4课"循环奥义"

Claude: 🎯 检测到产品设计需求，建议启动 /pd-start

执行 /pd-start L04循环奥义:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Phase 1: 调研分析
派遣 Researcher 调研:
- 12-16岁学员对循环的理解难点
- 竞品循环教学设计(CodeCombat/Scratch)
- while vs for 的教学顺序

产出: docs/design/research/L04-research.md

进入 Phase 2? [是/调整/结束]
用户: 是
...
```

**场景2: 现有设计评审**
```
用户: 评审L01/L02重构设计

Claude: 执行 /pd-review:

【评分总览】
| 维度 | 得分 |
|------|------|
| 教育价值 | 5/5 |
| 游戏化 | 5/5 |
| 用户体验 | 4/5 |
| 技术可行 | 5/5 |
| 一致性 | 4/5 |
| 总分 | 4.65/5 |

【问题清单】
P1: L01挑战XP偏低(20→30)
P2: 可增加阶段切换引导动画

结论: 有条件通过

【立即修改?】[是/稍后]
```

---

## 快速命令

| 命令 | 用途 |
|------|------|
| `/precheck` | 预提交依赖和编译检查 |
| `/precheck release` | 发布前完整检查 |
| `/task-v <标题>` | 创建带验证子任务的任务 |
| `/release <版本>` | 标准化发布流程 |
| `/tdd <功能>` | 测试驱动开发流程 |
| **NEW** /pd-start <主题> | **产品设计工作流程** |
| **NEW** /pd-review <文档> | **设计评审验证** |

---

## 发布前强制检查清单

在任何发布操作前，**必须**执行：

```bash
# 1. 验证GitHub CLI
gh --version
gh auth status

# 2. 验证依赖完整性
/precheck release

# 3. 检查关键DLL (v1.3 实际路径 — 旧的 TeenagerCppLearning/ 已不存在)
ls TeenagerCPlusPlusEduSystem/src/TeenCppEdu/bin/Release/net48/System.Data.SQLite.dll
ls TeenagerCPlusPlusEduSystem/src/TeenCppEdu/bin/Release/net48/x86/SQLite.Interop.dll
ls TeenagerCPlusPlusEduSystem/src/TeenCppEdu/bin/Release/net48/x64/SQLite.Interop.dll

# 4. 功能快速测试
# - L01: 代码编辑器显示 + 检查代码按钮
# - L03: 三阶段UI正常
```

---

## 代码生成约束 (.NET Framework 4.8)

生成C#代码时**必须避免**：

```csharp
// ❌ 不使用 (4.8不支持)
value.GetValueOrDefault()
bool.Parse(string)

// ✅ 使用替代
value ?? defaultValue
value.HasValue ? value.Value : defaultValue
bool.TryParse(string, out bool result)
```

---

## Task 创建规范

所有任务必须包含验证子任务：

```
/task-v 任务标题
→ 自动创建:
   1. 实现代码编写
   2. 编译验证
   3. 运行时测试
   4. 集成验证
   5. 文档更新
```

标记任务完成前，**必须确认**：
- [ ] 编译通过
- [ ] 运行时测试完成
- [ ] 至少一个边界条件已验证

---

## GitHub 通路

**首选 `gh` CLI**（已登录 `kaige2005`，scopes: gist / read:org / repo）。
仓库是 **`kaige2005/teenagercpp`**（PUBLIC），不是历史文档里写的 `Carl/TeenagerCPlusPlusEduSystem`（该仓库不存在）。

如果 `gh auth status` 失败：

1. 重新登录：`gh auth login`
2. 或手动 Web 上传：
   - 访问: https://github.com/kaige2005/teenagercpp/releases
   - 手动创建 Release 并上传资源

> ⚠️ 历史文档曾指向"已配置的 MCP GitHub 服务器"。该方案**从未生效**：
> Claude Code 不读 `.claude/mcp/` 这个路径（项目 MCP 必须是项目根的 `.mcp.json`），
> 且包名 `@anthropics/mcp-github-server` 在 npm 上不存在（404），`GITHUB_TOKEN` 也从未设置。
> 已于 2026-09-25 删除该死配置。需要 MCP 时请新建 `.mcp.json` 并先确认包名可用。

---

## 工具脚本位置

| 脚本 | 路径 | 用途 |
|------|------|------|
| 预提交检查 | `.claude/hooks/pre-commit.ps1` | 验证 DLL / SQLite.Interop / courses — **hook 已注销**（项目终止时移除），脚本保留可手动运行（见下） |
| ~~MCP配置~~ | ~~`.claude/mcp/github-server.json`~~ | 已删除（从未生效），改用 `gh` CLI |
| 发布Skill | `.claude/skills/release/SKILL.md` | /release命令 |
| TDD Skill | `.claude/skills/tdd/SKILL.md` | /tdd命令 |
| **产品设计框架** | `.claude/skills/product-design-framework/SKILL.md` | /pd-start |
| **设计评审** | `.claude/skills/pd-review/SKILL.md` | /pd-review |

> **预提交 hook 现状（2026-10-06 更新）**：脚本已修正到 v1.3 路径（旧版查的 `TeenagerCppLearning/` 和
> `WeifenLuo.WinFormsUI.Docking.dll` 都早已不存在）。
>
> 项目终止时该 hook **已从 `.claude/settings.json` 注销**——项目不再产生提交，留着只会在其他仓库误触发。
> 脚本本身保留在 `.claude/hooks/pre-commit.ps1`，需要时手动运行：
> `powershell -NoProfile -File .claude/hooks/pre-commit.ps1`（无输出 = 全部通过）。
>
> 如需恢复为自动 hook，在 `.claude/settings.json` 重新加入 `hooks.PreToolUse`：matcher `Bash|PowerShell`，
> 命中 `git commit*` 时执行本脚本，超时 30 秒。

---

## 故障排查速查

| 问题症状 | 解决方案 |
|---------|---------|
| SQLite.Interop.dll缺失 | 检查 `src/TeenCppEdu/bin/Release/net48/{x86,x64}/`，重装 System.Data.SQLite.Core |
| 课程界面不显示 | 走 `UI/Forms/ModernLessonForm.cs`（三阶段结构）；DockPanel/WeifenLuo 依赖**已移除**，不要再查它 |
| 检查代码空引用 | 确保 `_lblPhaseStatus` 初始化后再访问 |
| 课程加载失败 | 先验证 `courses/*/lesson.json` 是合法 JSON（L03 曾因字符串内未转义 `"` 而整课无法解析） |
| XP 进度条溢出 | L01/L02 实际发放 200 XP，而 UI 上限写死 190（见记忆 teencpp-xp-discrepancy） |
| GitHub CLI认证失效 | `gh auth login`（MCP 回退方案已删除，不再存在） |

---

*版本: 2.2 - 项目终止归档：修正 hook 状态（已注销）、补充归档路径说明*
