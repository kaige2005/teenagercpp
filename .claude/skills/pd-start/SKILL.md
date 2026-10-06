# pd-start - 启动产品设计工作流

快速启动产品设计阶段的多 Agent 协作流程。

## 用法

```
/pd-start <主题> [选项]

选项:
  --phase=all    完整流程 (默认)
  --phase=research 仅调研
  --phase=concept  仅概念
  --phase=detail   仅详细设计
  --phase=review   仅评审

  --parallel       并行执行 (默认)
  --sequential     串行执行

  --template=<name> 使用特定模板
```

## 快速开始

### 场景1: 完整设计流程

```
/pd-start L03新关卡设计
```

执行:
1. 派遣 Researcher 调研用户需求
2. 派遣 Architect + Storyteller 概念设计
3. 派遣 UX-Designer + Content-Designer 详细设计
4. 派遣 Reviewer 评审
5. 生成完整PRD

### 场景2: 仅评审现有设计

```
/pd-start L01重构方案 --phase=review
```

执行:
- 派遣 Reviewer 评估现有设计
- 输出评审报告
- 给出优化建议

### 场景3: 仅调研

```
/pd-start 青少年编程竞品分析 --phase=research
```

## 工作流详情

### Phase 顺序

```
/pd-start 触发:

if phase == all 或未指定:
  顺序执行:
    [1] Research  → docs/design/research/
    [2] Concept   → docs/design/concept/  (需确认后继续)
    [3] Detail    → docs/design/prd/      (需确认后继续)
    [4] Review    → docs/design/review/   (需确认后继续)
    
  每个阶段完成时提示:
  "Phase X 完成。查看产出物: <path>
   进入下一阶段? [是/修改/结束]"

if phase == specific:
  仅执行该阶段
```

### Agent 分配

| Phase | Agent | 输出 |
|-------|-------|------|
| Research | Researcher | 调研报告 |
| Concept | Architect + Storyteller | 概念文档 |
| Detail | UX-Designer + Content-Designer | PRD |
| Review | Reviewer + Validator | 评审报告 |

### 并行策略

**Phase内部并行**:
```
Concept Phase:
  Architect    ──┐
                 ├──→ 合并为概念文档
  Storyteller ──┘

Detail Phase:
  UX-Designer    ──┐
                   ├──→ 合并为PRD
  Content-Designer ─┘
```

## 交互示例

```
用户: /pd-start L04循环课程设计

Claude: 🚀 启动产品设计: L04循环课程设计

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Phase 1: 调研分析
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

派遣 Researcher 进行以下调研:
- 用户研究: 12-16岁学员对循环概念的理解难点
- 竞品分析: CodeCombat/Scratch循环教学
- 需求确认: L04在整体课程中的定位

执行中...

✅ 调研报告完成
📄 查看: docs/design/research/L04-research.md

关键发现:
- 痛点1: 循环条件容易混淆 (P0)
- 痛点2: 需要可视化理解循环过程 (P0)
- 设计建议: 采用动画+分步演示

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
进入 Phase 2: 概念设计? [是/修改/结束]
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

用户: 是

Claude: 
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Phase 2: 概念设计
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

并行执行:
┌─ Architect: 信息架构 ─────────┐
│ L04: 循环奥义                  │
│ ├── 知识: for/while概念        │
│ ├── 实践: 幸运摇奖机           │
│ └── 挑战: 无限循环Bug          │
└──────────────────────────────┘
┌─ Storyteller: 故事线 ────────┐
│ 场景: 刷怪峡谷                 │
│ 任务: 重复攻击直到清怪         │
│ 角色: 小极小灵学会循环魔法     │
└──────────────────────────────┘

合并为概念文档...

✅ 概念文档完成
📄 查看: docs/design/concept/L04-concept.md

进入 Phase 3: 详细设计? [是/修改/结束]
...
```

## 与现有Skill整合

```
/pd-start 内部调用链:

/pd-start
  ↓
调用 superpowers:writing-plans (规划)
  ↓
调用 superpowers:verification-before-completion (检查)
  ↓
调用 frontend-design (UI设计)
  ↓
调用 code-review (内容审核)
  ↓
产出最终PRD
```

## 产出物规范

### 目录结构

```
docs/design/
├── research/
│   └── <主题>-research.md
├── concept/
│   └── <主题>-concept.md
├── prd/
│   └── <主题>-prd.md
├── review/
│   └── <主题>-review.md
└── archive/
    └── (已冻结的设计)
```

### 冻结机制

评审通过后:
```
设计冻结 → docs/design/archive/<主题>-YYYYMMDD/
                                  ├── prd.md (锁定)
                                  ├── review.md (通过证明)
                                  └── handoff.md (开发交接)
```

## 质量保证

### 每个阶段的检查点

```
Research → Concept 过渡:
□ 用户画像明确?
□ 核心痛点已排序?
□ 竞品分析有价值?

Concept → Detail 过渡:
□ 信息架构合理?
□ 故事大纲完整?
□ 技术路线可行?

Detail → Review 过渡:
□ 所有功能点可测试?
□ 内容准确性验证?
□ 设计系统遵循?

Review → Freeze 过渡:
□ P0问题=0?
□ 评审平均分≥4?
□ 风险可控?
```

## 快捷命令

| 命令 | 用途 |
|------|------|
| `/pd-start` | 启动完整流程 |
| `/pd-research` | 仅调研 |
| `/pd-concept` | 仅概念 |
| `/pd-detail` | 仅详细设计 |
| `/pd-review` | 仅评审 |
| `/pd-status` | 查看当前设计进度 |

---

*整合: Superpowers + frontend-design + code-review*
