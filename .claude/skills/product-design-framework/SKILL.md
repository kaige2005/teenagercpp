# Product Design Framework - 产品设计阶段工作流

产品设计阶段的标准化工作流程，使用多 Agent 协作提高设计质量。

## 用法

```
/pd-start <设计主题> [--phase=<phase>]

阶段选项:
- research: 仅调研阶段
- concept: 仅概念设计
- detail: 仅详细设计
- review: 仅评审验证
- all: 完整流程 (默认)
```

##  Workflow 概览

```
┌─────────────────────────────────────────────────────────────────┐
│                    产品设计阶段 Workflow                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Phase 1: 调研分析 (Research)                                    │
│  ├── Agent: Researcher (市场/用户/竞品调研)                       │
│  ├── 产出: 调研报告 (用户画像/竞品分析/痛点清单)                   │
│  └── 检查点: 需求明确度确认                                       │
│                          ↓                                      │
│  Phase 2: 概念设计 (Concept)                                     │
│  ├── Agent: Architect (架构规划)                                 │
│  ├── Agent: Storyteller (故事线/角色设计)                        │
│  ├── 产出: 概念文档 (信息架构/用户流程/故事大纲)                   │
│  └── 检查点: 概念可行性评审                                       │
│                          ↓                                      │
│  Phase 3: 详细设计 (Detail)                                      │
│  ├── Agent: UX-Designer (交互设计)                               │
│  ├── Agent: Content-Designer (内容设计)                          │
│  ├── 产出: PRD (功能规格/内容细节/UI原型)                        │
│  └── 检查点: 设计完整性检查                                       │
│                          ↓                                      │
│  Phase 4: 评审验证 (Review)                                      │
│  ├── Agent: Reviewer (设计评审)                                  │
│  ├── Agent: Validator (可行性验证)                               │
│  ├── 产出: 评审报告 (问题清单/优化建议/通过与否)                   │
│  └── 检查点: 设计质量门禁                                         │
│                          ↓                                      │
│                    ✅ 设计冻结，进入开发                           │
└─────────────────────────────────────────────────────────────────┘
```

## 多 Agent 角色定义

### 1. Researcher (调研专家)

**职责**: 
- 用户研究 (用户画像、学习行为分析)
- 竞品分析 (功能对比、差异化机会)
- 需求挖掘 (痛点梳理、功能优先级)

**触发时机**: Phase 1

**输出模板**:
```markdown
# 调研报告: <主题>

## 用户画像
- 目标用户: 
- 核心需求:
- 使用场景:

## 竞品分析
| 产品 | 优势 | 劣势 | 机会点 |
|------|------|------|--------|

## 痛点清单
- P0 (必须解决):
- P1 (应该解决):
- P2 (可以做):

## 设计约束
- 技术约束:
- 时间约束:
- 资源约束:
```

### 2. Architect (架构师)

**职责**:
- 信息架构设计
- 功能模块划分
- 数据模型设计
- 技术路线规划

**触发时机**: Phase 2

### 3. Storyteller (叙事设计师)

**职责**:
- 世界观构建
- 角色设定
- 故事线设计
- 情感曲线规划

**触发时机**: Phase 2-3

**特殊技能**: 
- 连续课程的故事衔接
- 角色一致性维护
- 教育内容的游戏化包装

### 4. UX-Designer (交互设计师)

**职责**:
- 用户流程设计
- 界面布局规划
- 交互原型
- 微交互动效

**触发时机**: Phase 3

**工具整合**:
- frontend-design (视觉设计)
- DESIGN_SYSTEM (遵循设计系统)

### 5. Content-Designer (内容设计师)

**职责**:
- 课程内容编写
- 测验/题库设计
- 示例代码编写
- 错误提示文案

**触发时机**: Phase 3

**输出标准**:
- 内容准确性 (CS专家审核)
- 难度梯度 (教育心理学)
- 趣味性 (叙事包装)

### 6. Reviewer (设计评审员)

**职责**:
- 设计质量检查
- 一致性审查
- 用户体验评估
- 问题清单输出

**触发时机**: Phase 4

**评审维度**:
```
□ 教育价值 (学习目标是否明确)
□ 游戏化 (趣味性是否足够)
□ 用户体验 (流程是否顺畅)
□ 技术可行 (实现难度评估)
□ 一致性 (与现有设计对齐)
□ 完整性 (无遗漏功能)
```

### 7. Validator (可行性验证员)

**职责**:
- 技术可行性验证
- 时间/资源评估
- 风险识别
- ROI分析

**触发时机**: Phase 4

## 产出物标准

### Phase 1: 调研报告

| 产出物 | 必需内容 | 质量标准 |
|--------|----------|----------|
| 用户画像 | 年龄段/认知水平/动机/痛点 | 定量+定性数据 |
| 竞品矩阵 | 3-5个竞品, 功能对比 | 客观对比, 来源标注 |
| 需求清单 | 优先级排序的痛点列表 | MoSCoW方法 |
| 设计约束 | 技术/时间/资源限制 | 明确可验证 |

**评审通过标准**:
- [ ] 目标用户清晰可描述
- [ ] 核心痛点 ≤ 5个且已排序
- [ ] 竞品分析覆盖主流产品

### Phase 2: 概念文档

| 产出物 | 必需内容 | 质量标准 |
|--------|----------|----------|
| 信息架构 | 功能模块图/信息层级 | 用户视角 |
| 用户流程 | 核心流程图/状态转换 | 覆盖主路径+异常 |
| 故事大纲 | 世界观/角色/章节概要 | 情感曲线可见 |
| 数据模型 | 核心实体/关系 | 满足功能需求 |

**评审通过标准**:
- [ ] 架构图可被非技术人员理解
- [ ] 故事大纲有完整起承转合
- [ ] 所有P0需求被覆盖

### Phase 3: 详细设计 (PRD)

| 产出物 | 必需内容 | 质量标准 |
|--------|----------|----------|
| 功能规格 | 每个功能的输入/输出/规则 | 可测试 |
| 内容细节 | 文案/示例/测验完整 | CS准确性验证 |
| UI原型 | 低保真/高保真 | 遵循设计系统 |
| API定义 | 接口/数据结构 | 前后端一致 |

**评审通过标准**:
- [ ] 任意功能可被工程师直接实现
- [ ] 内容经CS专家审核
- [ ] UI与现有课程风格一致

### Phase 4: 评审报告

| 产出物 | 必需内容 | 质量标准 |
|--------|----------|----------|
| 问题清单 | 分类/优先级/修改建议 | 可执行 |
| 风险评估 | 技术风险/进度风险/体验风险 | 有应对方案 |
| 通过决议 | Go/NoGo/有条件通过 | 明确标准 |

**通过标准**:
- P0问题: 0个
- P1问题: ≤ 3个且有解决方案
- 风险评估: 可接受

## 工作流执行

### 启动设计流程

```
用户: /pd-start L01-L02三阶段重构

Claude: 🚀 启动产品设计流程: L01-L02三阶段重构

Phase 1: 调研分析
├── 派遣 Researcher 进行用户研究
├── 派遣 Researcher 进行竞品分析
└── 产出: 调研报告

[执行中...]

调研报告完成。
请查看: docs/design/research/L01-L02-research.md

进入 Phase 2? [是/调整/结束]
```

### 阶段切换检查

每个阶段结束时:

```
Phase X 完成检查清单:
□ 所有必需产出物已生成
□ 产出物符合质量标准
□ 关键决策已记录
□ 干系人已确认

[通过] → 进入下一阶段
[未通过] → 在当前阶段迭代
```

### 并行工作流

当无依赖时，多 Agent 并行:

```
Phase 2 概念设计
├── Architect (信息架构) ──┐
├── Storyteller (故事线) ──┼→ 同步到概念文档
└── Architect (数据模型) ──┘

三者并行执行，自动合并输出
```

## 与现有工具整合

### Superpowers 整合

| Superpowers Skill | 对应 Agent | 使用场景 |
|------------------|-----------|----------|
| brainstorming | Researcher | 需求发散 |
| writing-plans | Architect | 架构规划 |
| executing-plans | UX-Designer | 设计执行 |
| verification-before-completion | Reviewer | 质量检查 |

### frontend-design 整合

**UX-Designer Agent** 自动调用:
```
当设计UI布局时:
  调用 frontend-design 生成高保真原型
  
当需要视觉优化时:
  调用 frontend-design 提供设计方案
```

### Skill 调用链

```
/pd-start 主题
  ↓
触发 /pd-research (Researcher)
  ↓
触发 /pd-concept (Architect + Storyteller)
  ↓
触发 /pd-detail (UX-Designer + Content-Designer)
  ↓
触发 /pd-review (Reviewer + Validator)
  ↓
产出: 完整PRD + 评审报告
```

## 示例工作流

### 场景: L01 重构设计

```
Step 1: 启动
用户: /pd-start L01三阶段重构

Step 2: 调研 (Researcher Agent)
调研问题:
- 12-16岁学员对现有L01的反馈?
- 竞品(CodeCombat/Codecademy)入门课设计?
- 三阶段 vs 传统关卡的学习效果?

产出: research/L01-research.md

Step 3: 概念 (Architect + Storyteller)
Architect:
- L01三阶段结构定义
- 与L02/L03的一致性规划
- 数据模型调整

Storyteller:
- 魔法学院世界观
- 小极小灵角色设定
- L01故事大纲

产出: concept/L01-concept.md

Step 4: 详细设计 (UX-Designer + Content-Designer)
UX-Designer:
- 三阶段导航UI
- 场景/测验/Bug猎手界面
- 调用 frontend-design 视觉优化

Content-Designer:
- 场景故事文案
- 3道测验题目
- 3道代码填空
- Bug猎手代码

产出: prd/L01-prd.md

Step 5: 评审 (Reviewer + Validator)
Reviewer 检查:
- 教育价值: ⭐⭐⭐⭐⭐
- 游戏化: ⭐⭐⭐⭐⭐
- 一致性: ⭐⭐⭐⭐⚪ (L01挑战XP偏低)
- 完整性: ⭐⭐⭐⭐⭐

Validator 验证:
- 技术可行: 可用现有框架
- 时间评估: 3天开发
- 风险: UI适配需测试

产出: review/L01-review.md
评审结论: 有条件通过 (需调整XP)

Step 6: 设计冻结
所有产出物归档到 docs/design/L01/
进入开发阶段
```

## 配置与定制

### 项目配置

`.claude/settings.json`:
```json
{
  "productDesign": {
    "defaultAgents": ["Researcher", "Architect", "Storyteller", 
                      "UX-Designer", "Content-Designer", "Reviewer"],
    "reviewThreshold": {
      "p0Max": 0,
      "p1Max": 3,
      "minScore": 4.0
    },
    "autoAdvance": false,
    "parallelAgents": true
  }
}
```

### 产出物模板路径

```
.claude/templates/product-design/
├── research-template.md
├── concept-template.md
├── prd-template.md
└── review-template.md
```

## 质量门禁

设计阶段结束必须通过:

```
质量门禁检查:
□ 调研报告: 用户画像清晰, 需求已排序
□ 概念文档: 架构可行, 故事完整
□ 详细设计: 功能可测试, 内容准确
□ 评审通过: P0问题=0, 平均分≥4
□ 设计冻结: 所有文档已归档

失败处理:
- 退回当前阶段修改
- 或降级功能 (移至下一版本)
```

---

*版本: 1.0*
*整合: Superpowers + frontend-design*
