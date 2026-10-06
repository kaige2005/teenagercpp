# TeenC++ 项目文档管理规范

**版本**: 2.0  
**日期**: 2026-04-06  
**适用范围**: TeenC++ 教学系统全项目

---

## 1. 目录结构规范 (简化版)

```
TeenagerCPlusPlusEduSystem/
├── README.md                    # 项目入口文档
├── CHANGELOG.md                 # 版本变更日志
├── README.md                    # 首页项目说明
│
├── docs/                        # 📚 文档中心
│   ├── README.md               # 文档导航页
│   │
│   ├── design/                 # 🎨 设计文档 (四阶段工作流产出)
│   │   ├── README.md          # 设计文档导航
│   │   ├── DESIGN_SYSTEM.md   # 设计系统规范
│   │   ├── WORKFLOW_GUIDE.md  # 产品设计工作流 (原名DESIGN_WORKFLOW_GUIDE)
│   │   ├── research/          # Phase 1: 调研
│   │   ├── concept/           # Phase 2: 概念
│   │   ├── prd/               # Phase 3: 详细设计
│   │   └── review/            # Phase 4: 评审
│   │
│   ├── dev/                    # 💻 开发文档
│   │   ├── BUILD.md           # 构建指南
│   │   ├── QUICKSTART.md      # 快速开始
│   │   ├── VSCODE_GUIDE.md    # IDE配置
│   │   └── architecture/      # 架构文档(从design/迁移)
│   │
│   ├── process/                # 🔄 流程规范
│   │   ├── WORKFLOW.md        # 主工作流(合并OPTIMIZED版本)
│   │   ├── QUALITY_ASSURANCE.md
│   │   ├── DEFECT_WORKFLOW.md
│   │   └── templates/         # 模板文件
│   │
│   ├── release/                # 🚀 发布管理
│   │   ├── RELEASE_PROCESS.md
│   │   ├── RELEASE_POLICY.md
│   │   ├── VERSION_MANAGEMENT.md
│   │   └── history/           # 历史发布记录(从docs根目录移入)
│   │
│   ├── planning/               # 📋 规划文档
│   │   ├── ROADMAP.md         # 产品路线图
│   │   ├── BACKLOG.md         # 需求待办
│   │   └── Sprint-*.md        # Sprint状态
│   │
│   └── archive/                # 📦 归档文档(不再活跃使用)
│       ├── old_workflow/      # 旧版本工作流
│       └── ideas/             # 历史构思文档
│
├── courses/                    # 📖 课程内容
│   ├── l01/
│   ├── l02/
│   └── ...
│
├── src/                        # 💾 源代码
├── tests/                      # 🧪 测试
└── tools/                      # 🔧 工具脚本
```

---

## 2. 文档分类规则

### 2.1 按生命周期分类

| 类别 | 说明 | 位置 | 保留策略 |
|------|------|------|----------|
| **活跃** (Active) | 当前正在使用的文档 | 对应分类目录 | 持续更新 |
| **归档** (Archive) | 历史版本，仍有参考价值 | `docs/archive/` | 不修改，可查看 |
| **废弃** (Obsolete) | 完全过时，无参考价值 | 删除或移到 archive/obsolete/ | 不维护 |

### 2.2 按内容类型分类

| 类型 | 命名规则 | 示例 |
|------|----------|------|
| 设计文档 | `{主题}_{类型}.md` | `L01_L02_concept.md` |
| 状态文档 | `{类型}_{日期}.md` | `WORK_STATE_20260406.md` |
| 检查清单 | `{主题}_CHECKLIST.md` | `L01_REFACTOR_CHECKLIST.md` |
| 测试报告 | `TEST_{类型}_v{版本}.md` | `TEST_MATRIX_v1.2.md` |
| Sprint计划 | `Sprint-{年份}-{月份}-{主题}.md` | `Sprint-2026-04-ProductPolish.md` |

---

## 3. 命名规范

### 3.1 禁止使用

```
❌ xxx_OLD.md
❌ xxx_NEW.md
❌ xxx_FINAL.md
❌ xxx_v1.md, xxx_v2.md (使用Git版本控制)
❌ xxx_OPTIMIZED.md (合并到主版本)
```

### 3.2 推荐使用

```
✅ README.md              # 目录入口文档
✅ WORKFLOW.md            # 主工作流(版本在Git中管理)
✅ WORKFLOW_LEGACY.md     # 明确标注为遗留版本
✅ {主题}_deprecated.md   # 明确标注为废弃
```

### 3.3 日期格式

```
文件名: {前缀}_{YYYYMMDD}.md
示例: WORK_STATE_20260406.md
      SPRINT_20260401_20260415.md
```

---

## 4. 状态追踪统一方案

### 4.1 主状态源

| 状态类型 | 来源 | 更新时机 |
|----------|------|----------|
| 会话级状态 | `~/.claude/.../memory/MEMORY.md` | 每次会话结束 |
| 日状态快照 | `docs/state/WORK_STATE_{YYYYMMDD}.md` | 每天结束 |
| Sprint状态 | `docs/planning/Sprint-{日期}.md` | Sprint周期 |

### 4.2 禁止重复创建

```
❌ docs/WORK_STATE_20260404.md
❌ docs/WORK_STATE_20260404_END.md
✅ docs/state/WORK_STATE_20260404.md (统一位置)
```

### 4.3 状态文档模板

```markdown
# 工作状态 - 2026-04-06

## 完成项
- [x] 任务A

## 产出物清单
| 文件 | 路径 | 描述 |
|------|------|------|
| 文档X | docs/design/X.md | ... |

## 阻塞项
- 问题Y: 需要...

## 下一步
- [ ] 任务B (优先级: P0)

---
更新人: {名字}
更新时间: {时间}
```

---

## 5. 文档维护职责

| 文档类型 | 维护者 | 更新频率 | 审核者 |
|----------|--------|----------|--------|
| DESIGN_SYSTEM.md | 设计负责人 | 需求驱动 | Architect |
| WORKFLOW_GUIDE.md | 流程管理员 | 流程变更后 | 项目负责人 |
| PRD | 产品经理 | 设计完成后 | Reviewer |
| Architecture | 技术负责人 | 架构变更后 | 技术评审 |
| RELEASE_PROCESS | 发布经理 | 流程优化后 | 项目负责人 |

---

## 6. 当前整改清单

### 6.1 立即执行

- [ ] 合并 `WORKFLOW.md` + `WORKFLOW_OPTIMIZED.md`
- [ ] 移动 `design/architecture.md` → `docs/dev/architecture/`
- [ ] 清理 `docs/curriculum/` 重复内容
- [ ] 创建 `docs/state/` 状态目录
- [ ] 创建 `docs/archive/` 归档目录

### 6.2 逐步优化

- [ ] 将版本相关的报告移入 `docs/release/history/`
- [ ] 归档旧Sprint文档到 `docs/planning/archive/`
- [ ] 统一所有文档的Frontmatter格式

---

## 7. 快速导航

### 7.1 常用文档速查

| 内容 | 实际路径 |
|------|----------|
| 设计系统 | `docs/design/DESIGN_SYSTEM.md` |
| 产品设计工作流 | `docs/design/WORKFLOW_GUIDE.md` |
| 主开发流程 | `docs/process/WORKFLOW.md` |
| 构建指南 | `docs/dev/BUILD.md` |
| 当前Sprint | `docs/planning/Sprint-2026-04-ProductPolish.md` |
| L01/L02设计 | `docs/design/concept/L01_L02_concept.md` |

### 7.2 新增课程设计流程

```
1. 启动: 阅读 docs/design/WORKFLOW_GUIDE.md
2. 调研: 产出 docs/design/research/{代号}_research.md
3. 概念: 产出 docs/design/concept/{代号}_concept*.md
4. 详细: 产出 docs/design/prd/{代号}_prd.md
5. 评审: 产出 docs/design/review/{代号}_review.md
6. 开发: 产出 courses/{lXX}/lesson.json
7. 归档: 更新 docs/state/WORK_STATE_{日期}.md
```

---

*文档管理规范版本: 2.0*  
*最后更新: 2026-04-06*  
*责任人: 项目文档管理员*
