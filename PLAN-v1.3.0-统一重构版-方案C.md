# v1.3.0 统一重构版 - 可执行计划 (方案C)

**版本目标**: L01/L02重构 + L03增强 + 8课世界观统一  
**方案**: C (扩展版) - L03增加完整测验填空内容  
**工期**: 5.5周 (2026-04-15 至 2026-05-25)  
**发布日期**: 2026-05-25  

---

## 一、方案C核心变化

### 变化对比

| 方面 | 基础版 | 方案C扩展版 | 变化 |
|------|--------|------------|------|
| L03内容 | 世界观同步 | **完整三阶段** | +测验+填空+故事增强 |
| 测验题 | 仅L01-L02有 | **L01/L02/L03都有4题** | L03新增4题 |
| 代码填空 | 仅L01-L02有 | **L01/L02/L03都有4题** | L03新增4题 |
| 场景故事 | L01(5页)/L02(5页) | **L01/L02/L03都有5页** | L03故事从1页→5页 |
| **工期** | 5周 | **5.5周** | +3天 |
| **总课程** | 2.5课完整 | **3课完整** | v1.3.0覆盖更广 |

### 为什么选择方案C

✅ **用户价值**: v1.3.0发布时，用户可完整体验3课 (L01-L03)  
✅ **质量提升**: L03从"标杆"变为"标准"，体验一致  
✅ **测试充分**: 3课完整，可验证8课扩展可行性  
✅ **发布节奏**: 5.5周仍在Q2内，不影响后续版本  

---

## 二、Superpowers 全能力调用指南

### 2.1 Commands (命令层)

```bash
# Step 1: 头脑风暴 (启动前)
/brainstorm "v1.3.0关键风险与破局点"
  → 输出: 风险清单 + 应对策略

# Step 2: 写入计划 (本文档已完成)
/write-plan "v1.3.0统一重构版"
  → 输出: 结构化计划文档

# Step 3: 执行计划 (启动执行)
/execute-plan "v1.3.0统一重构版"
  → 自动调度Agents并行/串行执行
```

### 2.2 Agents (智能体层)

| Agent | 来源 | 职责 | 调用方式 |
|-------|------|------|----------|
| **Content-Designer** | `product-design-framework` | 内容编写 | `/task-v` + `--agent=Content-Designer` |
| **UX-Designer** | `product-design-framework` | UI设计 | `/task-v` + `--agent=UX-Designer` |
| **Reviewer** | `product-design-framework` | 质量评审 | 自动触发 (Milestone检查点) |
| **code-reviewer** | `code-review` plugin | 代码审查 | PR时自动触发 |
| **TDD-Agent** | `tdd` skill | 测试驱动 | `/tdd` 命令 |

**Agent调用示例**:
```bash
# 创建任务，指定Agent
/task-v "L01场景故事编写" \
  --agent=Content-Designer \
  --priority=P0 \
  --validation=content-review

# 调度执行
/execute-plan "Phase 1"
  → 系统自动: Content-Designer × 2 并行执行
```

### 2.3 Skills (技能层)

| Skill | 来源 | 用途 | 调用命令 |
|-------|------|------|---------|
| **product-design-framework** | `.claude/skills/` | 4-Phase课程设计 | `/pd-start` |
| **pd-review** | `.claude/skills/` | 设计评审 | `/pd-review` |
| **frontend-design** | `claude-plugins-official` | UI/UX设计 | `/frontend-design` |
| **tdd** | `.claude/skills/` | 测试驱动开发 | `/tdd` |
| **task-with-validation** | `.claude/skills/` | 带验证的任务 | `/task-v` |
| **precheck** | `.claude/skills/` | 预提交检查 | `/precheck` |
| **release** | `.claude/skills/` | 版本发布 | `/release` |
| **superpowers** | `claude-plugins-official` | 计划执行 | `/execute-plan` |

**Skill调用链示例**:
```bash
# L04课程设计 (v1.5预览)
/pd-start "L04循环秘境" --phase=all
  → 自动调用:
     1. Researcher (调研)
     2. Architect + Storyteller (概念)
     3. UX-Designer + Content-Designer (详细)
     4. Reviewer + Validator (评审)
  → 输出: 完整PRD

# 开发任务
/task-v "L04 lesson.json实现"
  → 自动创建:
     1. 实现任务
     2. 编译验证
     3. 运行时测试
     4. 集成验证
```

---

## 三、分Phase执行计划 (含Agent/Skill调用)

### Phase 1: 内容重构 (Week 1-2.5)

**目标**: L01/L02完整内容 + L03增强

```
Week 1 (并行最大化):
┌─────────────────────────────────────────────────────────────────┐
│ Agent调度: Content-Designer × 3 (并行)                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐              │
│ │Designer A   │ │Designer B   │ │Designer C   │              │
│ │             │ │             │ │             │              │
│ │L01场景故事  │ │L02场景故事  │ │L03场景增强  │              │
│ │(5页)        │ │(5页)        │ │(+4页)       │              │
│ │             │ │             │ │             │              │
│ │Day 1-2     │ │Day 1-2     │ │Day 1-2     │              │
│ └─────────────┘ └─────────────┘ └─────────────┘              │
│                                                                 │
│ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐              │
│ │后续任务     │ │后续任务     │ │后续任务     │              │
│ │             │ │             │ │             │              │
│ │Day 3-5     │ │Day 3-5     │ │Day 3-5     │              │
│ │概念卡片     │ │概念卡片     │ │概念卡片     │              │
│ │测验(4题)   │ │测验(4题)   │ │测验(4题)   │              │
│ │填空(4题)   │ │填空(4题)   │ │填空(4题)   │              │
│ └─────────────┘ └─────────────┘ └─────────────┘              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

**具体任务调用**:

```bash
# ========== Week 1 Day 1: 启动内容开发 ==========

# 任务1: L01场景故事
/task-v "L01场景故事编写-5页" \
  --agent=Content-Designer \
  --priority=P0 \
  --output="docs/content/l01/scene.md" \
  --validation=story-review

# 任务2: L02场景故事  
/task-v "L02场景故事编写-5页" \
  --agent=Content-Designer \
  --priority=P0 \
  --output="docs/content/l02/scene.md" \
  --validation=story-review

# 任务3: L03场景增强
/task-v "L03场景增强-4页" \
  --agent=Content-Designer \
  --priority=P0 \
  --output="docs/content/l03/scene_enhanced.md" \
  --validation=story-review

# 并行执行3个任务
/execute-plan "Week 1 Content - Phase 1"

# ========== Week 1 Day 2: 概念卡片 ==========

/task-v "L01概念卡片-5张" --agent=Content-Designer
/task-v "L02概念卡片-4张" --agent=Content-Designer  
/task-v "L03概念卡片-4张" --agent=Content-Designer

/execute-plan "Week 1 Content - Phase 2"

# ========== Week 1 Day 3-4: 测验与填空 ==========

/task-v "L01测验题目-4题" --agent=Content-Designer
/task-v "L02测验题目-4题" --agent=Content-Designer
/task-v "L03测验题目-4题" --agent=Content-Designer
/task-v "L01代码填空-4题" --agent=Content-Designer
/task-v "L02代码填空-4题" --agent=Content-Designer
/task-v "L03代码填空-4题" --agent=Content-Designer

/execute-plan "Week 1 Content - Phase 3"
```

---

### Week 2: 实践阶段 + Bug猎手

**Agent调度**: Content-Designer × 2 (并行)

```bash
# ========== Week 2 Day 1-2: 实践模板 + 检查规则 ==========

# L01
/task-v "L01实践模板与检查规则" \
  --agent=Content-Designer \
  --subtasks="template.cpp,rules.json" \
  --validation=compile-test

# L02
/task-v "L02实践模板与检查规则" \
  --agent=Content-Designer \
  --subtasks="template.cpp,rules.json" \
  --validation=compile-test

# L03 (增强)
/task-v "L03实践模板优化-增加TODO引导" \
  --agent=Content-Designer \
  --validation=compile-test

/execute-plan "Week 2 Practice Phase"

# ========== Week 2 Day 3-4: Bug猎手 ==========

/task-v "L01 Bug猎手-5个语法错误" --agent=Content-Designer
/task-v "L02 Bug猎手-==vs=陷阱" --agent=Content-Designer
/task-v "L03 Bug猎手-数组越界" --agent=Content-Designer

/execute-plan "Week 2 Challenge Phase"

# ========== Week 2 Day 5: lesson.json组装 ==========

/task-v "L01 lesson.json组装" --agent=Content-Designer --validation=json-schema
/task-v "L02 lesson.json组装" --agent=Content-Designer --validation=json-schema
/task-v "L03 lesson.json更新" --agent=Content-Designer --validation=json-schema

/execute-plan "Week 2 Integration"

# Milestone 1 检查点
/pd-review "L01 L02 L03内容" --dimensions=content
  → Reviewer Agent自动评审
  → 输出: 问题清单 + 修改建议
```

---

### Phase 2: 技术实现 (Week 3)

**Agent调度**: UX-Designer × 1 + TDD-Agent × 1 (并行)

```bash
# ========== Week 3: UI布局优化 ==========

# 启动UI设计 (调用 frontend-design skill)
/frontend-design "v1.3.0 UI布局优化" \
  --requirements="responsive,1280-1920,DPI-aware" \
  --deliverables="layout-spec.md,ui-mockups"

# 创建UI开发任务
/task-v "UI响应式布局实现" \
  --agent=UX-Designer \
  --priority=P0 \
  --subtasks="left-panel,main-content,dpi-scaling" \
  --validation=ui-test

# ========== Week 3: 技术债务清理 ==========

# TDD方式清理编译警告
/tdd "编译警告清理-前50个" \
  --scope="high-priority-warnings" \
  --target="50/168" \
  --approach="gradual-fix"

# 并行执行
/execute-plan "Week 3 Technical"

# Milestone 2 检查点
/precheck
  → 编译检查
  → 警告数量统计
  → 布局验证
```

---

### Phase 3: 测试验证 (Week 4)

**Agent调度**: TDD-Agent × 1 + QA自动化

```bash
# ========== Week 4: 全面测试 ==========

# 主流程UI自动化测试
/task-v "UI自动化测试-主流程" \
  --agent=TDD-Agent \
  --scenarios="l01-complete,l02-complete,l03-complete,lesson-transition" \
  --validation=auto

# 性能测试
/task-v "性能测试" \
  --metrics="startup-time,lesson-load-time,memory-usage" \
  --targets="<3s,<2s,<200MB"

# 手动验收测试 (人工)
# - L01完整通关
# - L02完整通关  
# - L03完整通关
# - 徽章获得验证

/execute-plan "Week 4 Testing"

# 代码审查 (自动触发 code-reviewer Agent)
# - PR创建时自动触发
# - 输出: 代码质量报告

# Milestone 3 检查点
/precheck release
  → 全量检查
  → 测试报告
  → 质量门禁
```

---

### Phase 4: 发布准备 (Week 5-5.5)

```bash
# ========== Week 5: Bug修复与文档 ==========

# Bug修复任务 (根据测试结果)
/task-v "P0 Bug修复" --priority=P0
/task-v "P1 Bug修复" --priority=P1

# 文档更新
/task-v "README更新-v1.3.0" --agent=Content-Designer
/task-v "CHANGELOG更新" --agent=Content-Designer

/execute-plan "Week 5 Bugfix"

# ========== Week 5.5 Day 1-2: 发布 ==========

# 最终检查
/precheck release

# 版本发布 (调用 release skill)
/release v1.3.0 \
  --type=feature \
  --notes="L01/L02/L03统一重构完成" \
  --artifacts="installer,documentation"

# 发布后验证
# - 下载安装测试
# - 完整流程验证
```

---

## 四、任务清单总表

### 内容任务 (Content-Designer)

| 任务ID | 任务名 | 工期 | Agent | 验证方式 | 输出 |
|--------|--------|------|-------|----------|------|
| C01-01 | L01场景故事5页 | 2d | Designer A | story-review | scene.md |
| C01-02 | L01概念卡片5张 | 1d | Designer A | content-check | concepts.md |
| C01-03 | L01测验4题 | 1d | Designer A | quiz-valid | quiz.json |
| C01-04 | L01填空4题 | 1d | Designer A | code-check | fillblank.json |
| C01-05 | L01模板+规则 | 2d | Designer A | compile-test | template.cpp,rules.json |
| C01-06 | L01 Bug猎手 | 1d | Designer A | playable | buggy.cpp |
| C01-07 | L01 lesson.json | 0.5d | Designer A | json-schema | lesson.json |
| **C01小计** | | **8.5d** | | | |
| | | | | | |
| C02-01~07 | L02同上结构 | 8.5d | Designer B | 同上 | 同上 |
| **C02小计** | | **8.5d** | | | |
| | | | | | |
| C03-01~07 | L03增强内容 | 8.5d | Designer C | 同上 | 同上 |
| **C03小计** | | **8.5d** | | | |
| **内容总计** | | **25.5人天** | | | |

### 技术任务

| 任务ID | 任务名 | 工期 | Agent/Skill | 验证方式 |
|--------|--------|------|-------------|----------|
| T01 | UI布局优化 | 3d | UX-Designer | ui-test |
| T02 | DPI适配 | 1d | UX-Designer | multi-dpi-test |
| T03 | 编译警告清理 | 3d | TDD-Agent | build-clean |
| T04 | UI自动化测试 | 2d | TDD-Agent | auto-test |
| T05 | 性能测试 | 1d | TDD-Agent | benchmark |
| **技术总计** | | **10人天** | | |

### 发布任务

| 任务ID | 任务名 | 工期 | Agent | 验证方式 |
|--------|--------|------|-------|----------|
| R01 | Bug修复 | 2d | Dev | regression-test |
| R02 | 文档更新 | 1d | Content | review |
| R03 | 版本发布 | 1d | release skill | install-test |
| **发布总计** | | **4人天** | | |

---

## 五、执行入口

### 快速启动 (复制即用)

```bash
# ====== 启动 v1.3.0 统一重构版 (方案C) ======

# Step 1: 加载计划
/read PLAN-v1.3.0-统一重构版-方案C.md

# Step 2: 头脑风暴 (可选)
/brainstorm "v1.3.0执行关键风险和破局策略"

# Step 3: 执行 Phase 1 (内容重构)
/execute-plan "v1.3.0-Phase 1"
  → 自动并行调度:
     - Content-Designer A → L01
     - Content-Designer B → L02
     - Content-Designer C → L03

# Step 4: 执行 Phase 2 (技术实现)
/execute-plan "v1.3.0-Phase 2"
  → 自动并行调度:
     - UX-Designer → UI布局
     - TDD-Agent → 债务清理

# Step 5: 执行 Phase 3 (测试)
/execute-plan "v1.3.0-Phase 3"
  → TDD-Agent → 全量测试

# Step 6: 执行 Phase 4 (发布)
/execute-plan "v1.3.0-Phase 4"
  → Bug修复 + 发布

# 或者直接执行完整计划
/execute-plan "v1.3.0统一重构版-方案C"
```

---

## 六、关键检查点与质量门禁

### 质量门禁 (Review Points)

| 检查点 | 触发条件 | Agent/Command | 通过标准 |
|--------|----------|---------------|---------|
| **RP1: 内容冻结** | Week 2.5结束 | `/pd-review` | 3课内容通过评审 |
| **RP2: 技术完成** | Week 3结束 | `/precheck` | 编译通过+布局正常 |
| **RP3: 测试通过** | Week 4结束 | `/precheck release` | 全量测试通过 |
| **RP4: 发布就绪** | Week 5.5 | `code-reviewer` +人工 | 0 P0/P1 Bug |

### Reviewer自动评审维度

```bash
# Milestone 1: 内容评审
/pd-review "L01 L02 L03内容" \
  --dimensions=all \
  --reviewers="Content-Reviewer,Story-Reviewer,CS-Expert"
  
评审维度:
- ✅ 教育价值: 学习目标明确性
- ✅ 故事连贯: 世界观一致性  
- ✅ CS准确性: 技术概念正确
- ✅ 用户体验: 难度梯度合理
- ✅ 完整性: 无遗漏内容

输出: 
- 评分 (1-5每维度)
- 问题清单 (P0/P1/P2)
- 修改建议
```

---

## 七、风险与Superpowers应对

| 风险 | 概率 | Superpowers应对策略 |
|------|------|-------------------|
| 内容量大延期 | 中 | `/execute-plan` 自动并行最大化 |
| 质量不达标 | 中 | `Reviewer` Agent自动门禁 + `/pd-review` |
| 编译警告难清理 | 低 | `TDD-Agent` 分批处理 + 可延后 |
| UI适配复杂 | 低 | `frontend-design` skill专业支持 |
| Agent过载 | 低 | 串行降级策略 (自动回退) |

---

## 八、最终状态确认

**当前可执行状态**:
- ✅ 计划文档完整 (本文件)
- ✅ Agent资源就绪 (Content×3, UX×1, TDD×1)
- ✅ Skills全部可用 (9个skill已就位)
- ✅ 依赖清晰 (关键路径图已定义)
- ✅ 质量门禁明确 (4个RP检查点)

**准备就绪**:
```
命令: /execute-plan "v1.3.0统一重构版-方案C"
状态: ✅ 可执行
预期: 5.5周后发布v1.3.0
```

---

*计划版本*: C-1.0 (可执行版)  
*创建日期*: 2026-04-08  
*执行入口*: `/execute-plan "v1.3.0统一重构版-方案C"`  
*发布目标*: 2026-05-25 v1.3.0
