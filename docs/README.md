# 项目文档结构 (P1整改后)

**最后更新**: 2026-04-06

---

## 📁 统一目录结构

```
docs/
├── README.md                     # 本文档 - 统一入口
├── DOCUMENT_MANAGEMENT_GUIDE.md  # 文档管理规范
│
├── design/                       # 🎨 设计文档 (四阶段工作流产出物)
│   ├── README.md
│   ├── DESIGN_SYSTEM.md         # 设计系统规范
│   ├── DESIGN_WORKFLOW_GUIDE.md # 产品设计工作流 (四阶段方法)
│   │
│   ├── research/                # Phase 1: 调研分析
│   │   └── L01_L02_research.md
│   │
│   ├── concept/                 # Phase 2: 概念设计
│   │   ├── L01_L02_concept.md
│   │   ├── L01_L02_concept_architect.md
│   │   └── L01_L02_concept_storyteller.md
│   │
│   ├── prd/                     # Phase 3: 详细设计 (待创建)
│   └── review/                  # Phase 4: 评审验证 (待创建)
│
├── dev/                         # 💻 开发文档
│   ├── README.md
│   ├── BUILD.md                 # 构建指南
│   ├── QUICKSTART.md           # 快速开始
│   ├── VSCODE_GUIDE.md         # IDE配置
│   ├── CI_PIPELINE.md          # CI/CD配置
│   └── architecture/           # 架构文档 (待迁移)
│
├── process/                     # 🔄 流程规范
│   ├── README.md
│   ├── WORKFLOW.md             # 主工作流 (已合并优化版)
│   ├── DEFECT_WORKFLOW.md      # 缺陷处理流程
│   ├── QUALITY_ASSURANCE.md    # 质量管理体系
│   └── templates/              # 文档模板
│       ├── PRD_TEMPLATE.md
│       ├── ARCHITECTURE_TEMPLATE.md
│       ├── TASK_TEMPLATE.md
│       └── RELEASE_NOTES_TEMPLATE.md
│
├── release/                     # 🚀 发布管理
│   ├── README.md
│   ├── RELEASE_POLICY.md       # 发布政策
│   ├── RELEASE_PROCESS.md      # 发布流程
│   ├── RELEASE_CHECKLIST.md    # 发布检查清单
│   ├── VERSION_MANAGEMENT.md   # 版本管理规范
│   ├── GITHUB_RELEASE.md       # GitHub发布指南
│   └── history/                # 历史发布记录
│       └── RELEASE_v1.0.0.md
│
├── planning/                    # 📋 规划文档
│   ├── README.md
│   ├── PRD.md                  # 产品需求文档 (原始版)
│   ├── ROADMAP.md              # 产品路线图
│   ├── BACKLOG.md              # 需求待办列表
│   ├── PRODUCT_IDEAS_RAW.md    # 产品构思原稿
│   ├── Sprint-2026-04.md       # 当前Sprint
│   └── Sprint-2026-04-ProductPolish.md
│
├── test/                        # 🧪 测试文档
│   ├── README.md
│   ├── TEST_PLAN.md            # 测试计划
│   ├── TEST_MATRIX_v1.2.md     # v1.2测试矩阵
│   ├── TEST_CASES_v1.2.0.md    # v1.2测试用例
│   ├── L01_REFACTOR_CHECKLIST.md
│   ├── PHASE_GAP_ANALYSIS.md
│   ├── DEV_SELF_TEST_CHECKLIST.md
│   └── curriculum/
│       └── TEST_CHECKLIST_LESSON2.md
│
├── teacher/                     # 👨‍🏫 教师文档
│   ├── README.md
│   ├── TEACHER.md              # 教师指南
│   └── CURRICULUM_SKILLS.md    # 课程技能说明
│
├── state/                       # 📊 状态快照
│   ├── README.md
│   ├── WORK_STATE_20260406.md  # 当前状态
│   ├── WORK_STATE_20260404.md
│   └── PROGRESS_LOG_20260403.md
│
└── archive/                     # 📦 归档文档
    ├── curriculum_legacy/      # 旧课程设计 (2个文档)
    ├── releases/               # 历史测试报告
    │   ├── TEST_RESULTS_v1.0.md
    │   ├── TEST_REPORT_v1.1.0-beta.3.md
    │   ├── DEV_TEST_REPORT_v1.2.0-beta.2.md
    │   └── TEST_EXECUTION_v1.2.0-beta.7.md
    └── sprints/               # (待归档)
```

---

## 📊 文档统计

| 分类 | 文件数 | 主要文档 |
|------|--------|----------|
| design | 6 | 概念设计完成，PRD待创建 |
| dev | 5 | 开发规范文档集 |
| process | 4+4 | 流程规范 + 模板 |
| release | 5+1 | 发布管理 (历史归archive) |
| planning | 6 | 规划文档集合 |
| test | 8 | 测试文档集 (历史归archive) |
| teacher | 2 | 教师相关 |
| state | 3 | 状态快照 |
| docs根 | 2 | 管理规范 + 索引 |
| **总计** | **~41** | 结构清晰，便于维护 |

---

## 🔗 快速导航

### 最常用 (Top 5)
1. **[流程规范](process/WORKFLOW.md)** - 主工作流
2. **[设计系统](design/DESIGN_SYSTEM.md)** - 视觉规范
3. **[构建指南](dev/BUILD.md)** - 如何编译
4. **[当前概念设计](design/concept/L01_L02_concept.md)** - 最新设计
5. **[质量管理](process/QUALITY_ASSURANCE.md)** - QA体系

### 当前活跃工作
- [L01/L02重构计划](design/L01_L02_Refactor_Plan.md)
- [Sprint-2026-04](planning/Sprint-2026-04-ProductPolish.md)
- [今天的状态](state/WORK_STATE_20260406.md)

---

## ✅ P0+P1 整改完成

| 原问题 | 整改动作 | 当前状态 |
|--------|----------|----------|
| 工作流分裂 | 合并内容，删除WORKFLOW_OPTIMIZED | ✅ 已完成 |
| curriculum重复 | 归档旧文档，删除目录 | ✅ 已完成 |
| 状态文档散乱 | 创建state/，统一命名，删除重复 | ✅ 已完成 |
| 文档分类不清 | 创建dev/process/release等目录 | ✅ 已完成 |
| 根目录文档多 | 迁移到对应分类目录 | ✅ 已完成 |
| 历史文档杂 | 归档旧测试报告 | ✅ 已完成 |
| 根目录design/ | 迁移architecture.md到docs/dev/ | ✅ 已完成 |

### 额外完成

- ✅ 迁移根目录 `design/architecture.md` → `docs/dev/architecture/`
- ✅ 迁移根目录 `design/course_data_schema.json` → `docs/design/`
- ✅ 删除空的根目录 `design/`
- ✅ 更新 MEMORY.md 所有文档路径

---

**下一步建议**:
- **A**: 继续 Phase 3 详细设计 (L01/L02重构)
- **B**: 整理根目录 design/ (迁移 architecture.md)
- **C**: P2优化 (统一Frontmatter等细节)

*最后更新: 2026-04-06 - P1整改完成*
