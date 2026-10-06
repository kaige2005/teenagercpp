# L01/L02 概念设计 - 架构设计 (Architect Agent)

## 一、信息架构设计

### 1.1 三阶段结构标准

基于L03成功经验，定义三阶段标准结构：

```
Lesson (课程)
├── 基本信息
│   ├── id, sequence, title, subtitle
│   ├── prerequisite, requiredExp
│   └── totalXp, iconPath
│
└── phases[] (三阶段)
    ├── Phase 1: Knowledge (知识)
    │   ├── type: "knowledge"
    │   ├── title: 阶段标题
    │   ├── sections[]: 小节列表
    │   │   ├── Section 1: scene (场景故事)
    │   │   ├── Section 2: concept (概念卡片)
    │   │   ├── Section 3: quiz (知识测验)
    │   │   └── Section 4: fillblank (代码填空)
    │   └── totalXp: 50-60
    │
    ├── Phase 2: Practice (实践)
    │   ├── type: "practice"
    │   ├── title: 阶段标题
    │   ├── description: 任务描述
    │   ├── steps[]: 编程步骤引导
    │   ├── templatePath: 代码模板
    │   ├── checkRulesPath: 检查规则
    │   ├── rewardExp: 100
    │   └── rewardBadge: 徽章ID
    │
    └── Phase 3: Challenge (挑战)
        ├── type: "challenge"
        ├── title: 阶段标题
        ├── description: 挑战描述
        ├── mode: "bug_hunt"
        ├── difficulty: "easy/medium/hard"
        ├── buggyCode: 错误代码
        ├── hints[]: 提示列表
        ├── expectedFix: 预期修复
        ├── rewardExp: 20-30
        └── rewardBadge: 徽章ID
```

### 1.2 L01 信息架构

```
L01: 你好，C++！
├── subtitle: "程序员的第一个魔法咒语"
├── totalXp: 190 (60+100+30)
│
├── Phase 1: Knowledge (60XP)
│   ├── Section 1: scene
│   │   ├── title: "魔法学院的入门咒语"
│   │   └── content: 小极小灵入学场景
│   ├── Section 2: concept
│   │   ├── title: "C++程序的魔法公式"
│   │   └── keyPoints: 5步结构
│   ├── Section 3: quiz (4题, 40XP)
│   │   ├── Q1: main函数入口
│   │   ├── Q2: cout用途
│   │   └── Q3: 分号作用
│   └── Section 4: fillblank (4空, 40XP)
│       ├── A: #include
│       ├── B: cout
│       └── C: endl
│
├── Phase 2: Practice (100XP)
│   ├── title: "编写个性化问候程序"
│   ├── steps: 3个TODO
│   ├── template: main.cpp (TODO标记)
│   ├── checkRules: 5项规则
│   └── badge: hello_master
│
└── Phase 3: Challenge (30XP)
    ├── title: "找出语法小偷"
    ├── buggyCode: 5个Bug
    ├── hints: 渐进提示
    └── badge: hello_bug_hunter
```

### 1.3 L02 信息架构

```
L02: 分支魔法
├── subtitle: "猜数字的智慧之门"
├── totalXp: 190 (60+100+30)
│
├── Phase 1: Knowledge (60XP)
│   ├── Section 1: scene
│   │   ├── title: "智慧之门的谜题"
│   │   └── content: 猜数字情景
│   ├── Section 2: concept
│   │   ├── title: "分支魔法的奥秘"
│   │   └── keyPoints: if/if-else/比较运算符
│   ├── Section 3: quiz (3题→4题, 40XP)
│   │   ├── Q1: == vs =
│   │   ├── Q2: if-else逻辑
│   │   ├── Q3: 猜大了条件
│   │   └── Q4: [新增]循环前概念
│   └── Section 4: fillblank (4空, 40XP)
│       ├── A: guess == secret
│       ├── B: else if
│       ├── C: else
│       └── D: 太大了提示
│
├── Phase 2: Practice (100XP)
│   ├── title: "开发完整的猜数字游戏"
│   ├── steps: 随机数+输入+分支
│   ├── template: main.cpp (随机数框架)
│   ├── checkRules: 5项规则
│   └── badge: branch_master
│
└── Phase 3: Challenge (30XP)
    ├── title: "猜数字的逻辑陷阱"
    ├── buggyCode: == vs = Bug
    ├── hints: 赋值vs比较区别
    └── badge: branch_bug_hunter
```

### 1.4 三课程一致性

| 维度 | L01 | L02 | L03 | 标准 |
|------|-----|-----|-----|------|
| **阶段数** | 3 | 3 | 3 | ✅ 统一 |
| **结构** | 知+实+挑 | 知+实+挑 | 知+实+挑 | ✅ 统一 |
| **测验数** | 3→4 | 4 | 3→4 | ✅ 统一为4 |
| **填空数** | 3 | 4 | 4 | ⚠️ L01调整为4 |
| **知识XP** | 50 | 60 | 60 | ⚠️ L01调整为60 |
| **实践XP** | 100 | 100 | 100 | ✅ 统一 |
| **挑战XP** | 20→30 | 30 | 30 | ✅ 统一 |
| **总XP** | 170→190 | 190 | 190 | ✅ 统一 |

**调整建议**:
- L01测验: 3→4题 (增加一道概念题)
- L01填空: 3→4空 (增加一道综合填空)
- L01挑战XP: 20→30
- L01知识XP: 50→60

---

## 二、数据模型设计

### 2.1 lesson.json 结构

```json
{
  "id": "L01|L02|L03",
  "sequence": 1|2|3,
  "title": "...",
  "subtitle": "...",
  "prerequisiteId": null|"L01"|"L02",
  "requiredExp": 0|100|250,
  "totalXp": 190,
  "iconPath": "assets/icons/lesson0X.png",
  "knowledgePoints": [...],
  
  "phases": [
    {
      "type": "knowledge",
      "title": "...",
      "sections": [
        {
          "type": "scene|concept|quiz|fillblank",
          "order": 1|2|3|4,
          "title": "..."
          // ... 详细字段见L03
        }
      ],
      "totalXp": 60
    },
    {
      "type": "practice",
      // ... 
    },
    {
      "type": "challenge",
      // ...
    }
  ]
}
```

### 2.2 数据库表 (复用现有)

**LessonPhaseProgress 表**:
```sql
CREATE TABLE LessonPhaseProgress (
    Id INTEGER PRIMARY KEY,
    LessonId TEXT,          -- L01/L02/L03
    PhaseType TEXT,         -- knowledge/practice/challenge
    Status TEXT,            -- locked/available/completed
    CurrentSection INTEGER, -- 当前小节序号
    CompletedSections TEXT, -- JSON数组 [1,2,3]
    EarnedXp INTEGER,       -- 已获得XP
    CompletedAt DATETIME    -- 完成时间
);
```

**数据迁移**:
- 旧格式L01/L02进度 → 映射到新阶段
- 已完成关卡 → 所有阶段标记为completed

### 2.3 徽章定义

```json
{
  "badges": [
    {
      "id": "hello_master",
      "name": "入门大师",
      "icon": "👋",
      "color": "#FFC850",
      "unlockCondition": "完成L01实践阶段"
    },
    {
      "id": "hello_bug_hunter",
      "name": "语法猎手",
      "icon": "🐛",
      "color": "#B050E0",
      "unlockCondition": "完成L01挑战阶段"
    },
    {
      "id": "branch_master",
      "name": "分支大师",
      "icon": "🔀",
      "color": "#5090E0",
      "unlockCondition": "完成L02实践阶段"
    },
    {
      "id": "branch_bug_hunter",
      "name": "逻辑猎手",
      "icon": "🐛",
      "color": "#B050E0",
      "unlockCondition": "完成L02挑战阶段"
    }
  ]
}
```

---

## 三、技术实现路线

### 3.1 复用组件清单

| 组件 | 用途 | 复用程度 |
|------|------|----------|
| ModernLessonForm | 三阶段导航 | ✅ 完全复用 |
| KnowledgePhaseForm | 知识阶段UI | ✅ 完全复用 |
| PracticePhaseForm | 实践阶段UI | ✅ 完全复用 |
| BugHuntForm | 挑战阶段UI | ✅ 完全复用 |
| LessonPhaseProgress | 进度存储 | ✅ 完全复用 |
| CheckCodeService | 代码检查 | ✅ 完全复用 |

### 3.2 新开发内容

| 内容 | 工作量 | 优先级 |
|------|--------|--------|
| L01 lesson.json | 2h | P0 |
| L02 lesson.json | 2h | P0 |
| L01 templates/main.cpp | 30min | P0 |
| L02 templates/main.cpp | 30min | P0 |
| L01 checks/rules.json | 1h | P0 |
| L02 checks/rules.json | 1h | P0 |
| UI布局调整 | 4h | P1 |
| 高分辨率测试 | 2h | P1 |

### 3.3 技术路线

```
Day 1: Content
- 编写L01/L02 lesson.json
- 编写代码模板
- 编写检查规则

Day 2: Integration
- 集成到课程系统
- 测试数据加载
- 测试阶段切换

Day 3: UI Polish
- 解决布局偏右
- 按钮遮挡修复
- 分辨率适配

Day 4: Testing
- L01通关测试
- L02通关测试
- L01→L02→L03衔接测试
```

### 3.4 风险缓解

| 风险 | 缓解策略 |
|------|----------|
| L03框架不兼容 | 提前验证数据格式 |
| UI适配复杂 | 优先1366×768，渐进优化 |
| 内容编写超时 | 优先P0内容，P1可延后 |
| 测试不充分 | 分阶段验收，每课单独测试 |

---

**Architect Agent 设计完成**

**下一步**: 与Storyteller输出合并 → 完整概念文档
