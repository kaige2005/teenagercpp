# 系统架构设计文档

## 1. 设计决策确认

| 决策项 | 确认方案 |
|--------|----------|
| 系统形态 | Windows 桌面内容管理系统 |
| 运行时 | 不集成代码执行，调用外部 Dev-C++ |
| 存储方案 | SQLite 单用户本地存储 |
| 开发策略 | 敏捷迭代，先做第1-2周试点 |
| 技术栈 | C# WinForms / WPF + SQLite |

---

## 2. 练习检查与评分方案

### 2.1 整体策略

```
┌─────────────────────────────────────────────────────────────────┐
│                    练习检查评分流程                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  学生侧:                        系统侧:                          │
│  ┌──────────────┐              ┌───────────────────────────┐    │
│  │ 在Dev-C++中   │              │ 1. 读取学生代码文件        │    │
│  │ 编写代码      │ ──────────▶  │    (监控指定目录)         │    │
│  │              │              │                           │    │
│  │ 运行测试      │ ──────────▶  │ 2. 验证输出结果           │    │
│  │ 复制输出      │   粘贴输出   │    (学生提交运行结果)      │    │
│  │              │              │                           │    │
│  │ 点击"完成"   │ ──────────▶  │ 3. 综合评分               │    │
│  └──────────────┘              │    - 代码结构检查 ✓        │    │
│                                │    - 输出正确性 ✓          │    │
│                                │    - 完成时间记录 ✓        │    │
│                                └───────────────────────────┘    │
│                                                                 │
│  导师侧(家长):                                                  │
│  ┌──────────────────────────────────────────────┐              │
│  │ • 查看学生提交的代码和输出                     │              │
│  │ • 人工确认代码逻辑正确性                       │              │
│  │ • 手动调整评分/打回重做                        │              │
│  │ • 在代码中插入BUG开启"BUG猎手"模式             │              │
│  └──────────────────────────────────────────────┘              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 2.2 具体检查机制

#### 机制1: 代码结构验证 (静态检查)
```csharp
// 系统检查清单示例
检查项:
├── 语法关键字检查
│   ├── 是否使用了 "cout" (第1课)
│   ├── 是否使用了 "int" 变量 (第1课)
│   ├── 是否使用了 "if" 语句 (第2课)
│   └── 是否使用了 "while" 循环 (第3课)
│
├── 代码完整性检查
│   ├── 是否包含 main 函数
│   ├── 是否包含必要的头文件 (#include)
│   └── 是否包含返回值 (return 0)
│
└── 代码质量提示 (非强制)
    ├── 变量命名是否有意义
    ├── 是否有适当的缩进
    └── 是否包含注释
```

#### 机制2: 输出结果验证
```
场景：第1课 "Hello 代码小勇者"

预期输出:
Hello 代码小勇者！

学生提交:
Hello 代码小勇者！
   ↓ 系统比对
✓ 完全匹配 → 通过
✗ 不匹配 → 提示差异 (忽略大小写/空格可配置)

场景：第1课 "勇者属性计算器"

预期输出格式:
总攻击力: XX

学生提交:
总攻击力: 150
   ↓ 系统比对
✓ 包含关键词"总攻击力" → 部分通过
? 导师确认数值计算是否正确
```

#### 机制3: 项目里程碑检查
```
周项目采用"里程碑"制：

第1周项目：猜数字游戏
├── 里程碑1：完成基础输出 ✓ (10分)
├── 里程碑2：实现输入交互 ✓ (10分)
├── 里程碑3：完成大小判断 ✓ (20分)
└── 里程碑4：增加循环retry ✓ (10分)

学生可逐条完成，系统记录每个里程碑的完成状态
```

### 2.3 评分算法

```python
# 伪代码
def calculate_score(task, student_submission):
    score = 0
    checks = []

    # 1. 完成度基础分 (只要提交就有)
    score += 5
    checks.append("提交作业: +5")

    # 2. 代码结构分
    if check_required_keywords(student_submission.code, task.required_keywords):
        score += 30
        checks.append("使用要求语法: +30")

    # 3. 输出验证分
    output_match = compare_output(student_submission.output, task.expected_output)
    if output_match == "exact":
        score += 40
        checks.append("输出完全正确: +40")
    elif output_match == "partial":
        score += 20
        checks.append("输出部分正确: +20")

    # 4. 首次通过奖励
    if student_submission.attempts == 1:
        score += 10
        checks.append("首次通过奖励: +10")

    # 5. 时间奖励 (提前完成)
    if student_submission.duration < task.estimated_duration * 0.5:
        score += 5
        checks.append("快速完成奖励: +5")

    return ScoreResult(score, checks)
```

### 2.4 数据交换协议

```yaml
# 学生任务提交数据结构
TaskSubmission:
  task_id: "week1_lesson1_practice1"
  submitted_at: "2025-03-15T14:30:00"
  code_file_path: "D:/CppLearning/Week1/practice1.cpp"
  code_content: "#include <iostream>..."
  runtime_output: "Hello 代码小勇者！"
  completion_time_minutes: 25
  attempts_count: 2
  student_notes: "我改了三次才让输出正确"
```

---

## 3. 系统模块详细设计

### 3.1 模块依赖图

```
┌─────────────────────────────────────────────────────────────────┐
│                        表现层 (UI)                               │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────────┐ │
│  │ MainWindow  │ │ LessonView  │ │ CodeSubmit  │ │ Dashboard │ │
│  │ (主窗口)     │ │ (课时学习)   │ │ (提交面板)   │ │ (仪表盘)   │ │
│  └──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └─────┬─────┘ │
└─────────┼───────────────┼───────────────┼──────────────┼───────┘
          │               │               │              │
          └───────────────┼───────────────┘              │
                          ▼                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                       业务逻辑层                                 │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐             │
│  │CourseManager │ │TaskValidator │ │ScoreEngine   │             │
│  │(课程管理)     │ │(任务验证)      │ │(评分引擎)      │             │
│  └──────┬───────┘ └──────┬───────┘ └──────┬───────┘             │
│         │                │                │                      │
│  ┌──────▼───────┐ ┌──────▼───────┐ ┌──────▼───────┐             │
│  │ProjectBuilder│ │CodeChecker   │ │Achievement   │             │
│  │(项目生成器)   │ │(代码检查器)   │ │Manager(成就)  │             │
│  └──────┬───────┘ └──────┬───────┘ └──────┬───────┘             │
└─────────┼────────────────┼────────────────┼─────────────────────┘
          │                │                │
          └────────────────┼────────────────┘
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│                        数据层                                    │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐             │
│  │CourseRepo    │ │ProgressRepo  │ │SubmissionRepo│             │
│  │(课程数据)     │ │(进度数据)     │ │(提交记录)     │             │
│  └──────┬───────┘ └──────┬───────┘ └──────┬───────┘             │
│         │                │                │                      │
│         └────────────────┼────────────────┘                      │
│                          ▼                                      │
│                    ┌──────────────┐                             │
│                    │ SQLite DB    │                             │
│                    │ cppedu.db    │                             │
│                    └──────────────┘                             │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 核心类设计

#### 3.2.1 课程管理模块
```csharp
namespace TeenCppEdu.Course
{
    // 课程定义
    public class Course
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string StoryBackground { get; set; }
        public List<Week> Weeks { get; set; }
    }

    // 周
    public class Week
    {
        public int WeekNumber { get; set; }
        public string Title { get; set; }
        public string StoryLine { get; set; }
        public string BadgeName { get; set; }
        public List<Lesson> Lessons { get; set; }
        public Project WeeklyProject { get; set; }
    }

    // 课时
    public class Lesson
    {
        public string Id { get; set; }
        public int LessonNumber { get; set; }
        public string Title { get; set; }
        public string StoryIntro { get; set; }  // 故事导入
        public List<string> KeyPoints { get; set; }  // 知识点
        public string CodeExample { get; set; }  // 代码示例
        public List<Practice> Practices { get; set; }  // 练习题
        public Homework Homework { get; set; }  // 课后作业
    }

    // 练习任务
    public class Practice
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CodeTemplate { get; set; }  // 代码模板（填空式）
        public List<string> RequiredKeywords { get; set; }  // 必须使用的关键字
        public string ExpectedOutput { get; set; }  // 预期输出
        public int Points { get; set; }  // 分值
    }
}
```

#### 3.2.2 代码检查模块
```csharp
namespace TeenCppEdu.Validation
{
    // 代码检查结果
    public class CodeCheckResult
    {
        public bool IsValid { get; set; }
        public List<CheckItem> Checks { get; set; }
        public int StructureScore { get; set; }
    }

    public class CheckItem
    {
        public string Name { get; set; }
        public bool Passed { get; set; }
        public string Message { get; set; }
    }

    // 代码检查器
    public class CodeChecker
    {
        // 检查是否包含必要关键字
        public CodeCheckResult CheckRequiredKeywords(string code, List<string> keywords)
        {
            // 实现：正则匹配或简单字符串查找
        }

        // 检查代码完整性
        public CodeCheckResult CheckCodeCompleteness(string code)
        {
            // 检查：main函数、头文件、return语句
        }

        // 对比输出结果
        public OutputMatchResult CompareOutput(string actual, string expected)
        {
            // exact: 完全匹配
            // partial: 部分匹配（包含关键信息）
            // none: 不匹配
        }
    }
}
```

#### 3.2.3 项目生成器
```csharp
namespace TeenCppEdu.Projects
{
    // 项目模板
    public class ProjectTemplate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Requirement> Requirements { get; set; }
        public string StarterCode { get; set; }
        public List<Milestone> Milestones { get; set; }
    }

    // 里程碑
    public class Milestone
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public List<string> CheckCriteria { get; set; }
    }

    // 项目生成器
    public class ProjectBuilder
    {
        // 生成Dev-C++项目文件
        public string GenerateProject(string outputPath, ProjectTemplate template)
        {
            // 创建项目目录结构
            // 生成 starter_code.cpp
            // 生成 requirements.txt (需求文档)
            // 生成 README.md (项目说明)
        }

        // 验证里程碑完成情况
        public MilestoneValidation ValidateMilestone(string code, Milestone milestone)
        {
            // 根据里程碑的CheckCriteria验证代码
        }
    }
}
```

---

## 4. 数据存储设计

### 4.1 数据库表结构

```sql
-- 课程定义表
CREATE TABLE courses (
    id TEXT PRIMARY KEY,
    title TEXT NOT NULL,
    story_background TEXT,
    total_weeks INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 周定义表
CREATE TABLE weeks (
    id TEXT PRIMARY KEY,
    course_id TEXT REFERENCES courses(id),
    week_number INTEGER,
    title TEXT,
    story_line TEXT,
    badge_name TEXT,
    badge_icon_path TEXT
);

-- 课时定义表
CREATE TABLE lessons (
    id TEXT PRIMARY KEY,
    week_id TEXT REFERENCES weeks(id),
    lesson_number INTEGER,
    title TEXT,
    story_intro TEXT,
    key_points TEXT, -- JSON数组
    code_example TEXT,
    estimated_duration INTEGER -- 预估分钟数
);

-- 练习题定义表
CREATE TABLE practices (
    id TEXT PRIMARY KEY,
    lesson_id TEXT REFERENCES lessons(id),
    title TEXT,
    description TEXT,
    code_template TEXT,
    required_keywords TEXT, -- JSON数组
    expected_output TEXT,
    points INTEGER DEFAULT 10,
    difficulty INTEGER -- 1=简单, 2=中等, 3=困难
);

-- 用户进度表
CREATE TABLE user_progress (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    week_id TEXT,
    lesson_id TEXT,
    practice_id TEXT,
    status TEXT, -- locked/unavailable/completed
    started_at TIMESTAMP,
    completed_at TIMESTAMP,
    score INTEGER,
    UNIQUE(lesson_id, practice_id) ON CONFLICT REPLACE
);

-- 提交记录表
CREATE TABLE submissions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    practice_id TEXT,
    submitted_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    code_content TEXT,
    runtime_output TEXT,
    completion_time_minutes INTEGER,
    attempts_count INTEGER DEFAULT 1,
    structure_score INTEGER,
    output_score INTEGER,
    total_score INTEGER,
    mentor_approved BOOLEAN DEFAULT NULL,
    mentor_notes TEXT
);

-- 积分记录表
CREATE TABLE score_history (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    source_type TEXT, -- task/homework/project/achievement
    source_id TEXT,
    points INTEGER,
    earned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    description TEXT
);

-- 成就表
CREATE TABLE achievements (
    id TEXT PRIMARY KEY,
    name TEXT,
    description TEXT,
    icon_path TEXT,
    unlock_condition TEXT
);

-- 用户成就关联表
CREATE TABLE user_achievements (
    achievement_id TEXT REFERENCES achievements(id),
    unlocked_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (achievement_id)
);
```

### 4.2 文件目录结构

```
TeenCppEduSystem/
├── TeenCppEdu.exe          # 主程序
├── cppedu.db               # SQLite数据库
├── config.ini              # 配置文件
│
├── Content/                # 课程内容（可更新）
│   ├── courses/
│   │   └── cpp_fundamentals.json  # 课程定义
│   ├── templates/
│   │   ├── week1/          # 第1周项目模板
│   │   ├── week2/          # 第2周项目模板
│   │   └── ...
│   └── assets/
│       ├── images/         # 图片资源
│       ├── badges/         # 徽章图标
│       └── animations/     # 动画资源
│
├── StudentWorkspace/       # 学生学习工作区
│   ├── Week1/              # 第1周代码
│   ├── Week2/              # 第2周代码
│   └── ...
│
└── Logs/                   # 日志文件
    └── app.log
```

---

## 5. 第1-2周试点范围定义

### 5.1 试点功能清单

#### P0 - 必须完成
| 功能 | 说明 |
|------|------|
| ✅ 课程导航 | 第1-2周课程结构展示，课时切换 |
| ✅ 内容展示 | 故事导入、知识点、代码示例 |
| ✅ 代码模板生成 | 生成带填空标记的 .cpp 文件到工作区 |
| ✅ 提交面板 | 读取学生代码文件、粘贴输出结果 |
| ✅ 基础检查 | 关键字检查、代码完整性检查 |
| ✅ 评分反馈 | 显示得分明细和通过/未通过 |
| ✅ 进度保存 | 课时完成状态、已得分统计 |
| ✅ 周项目生成 | 生成项目目录结构和启动代码 |

#### P1 - 建议完成
| 功能 | 说明 |
|------|------|
| ⭕ 成就系统 | 第1周章、第2周章解锁 |
| ⭕ 积分系统 | 完成任务获得积分 |
| ⭕ 代码展示墙 | 展示已完成的项目 |

#### P2 - 可选
| 功能 | 说明 |
|------|------|
| ⭕ 动画演示 | 代码执行流程可视化 |
| ⭕ BUG猎手模式 | 导师插入BUG供学生修复 |

### 5.2 试点验收标准

```
学生使用流程验证:
1. 打开系统 → 看到第1周课程锁定/解锁状态 ✓
2. 点击第1课 → 看到故事导入和知识点 ✓
3. 点击"开始练习" → 系统在StudentWorkspace生成代码文件 ✓
4. 在Dev-C++中打开文件 → 编写代码 → 运行测试 ✓
5. 返回系统 → 提交代码文件路径 → 系统检测关键字 ✓
6. 粘贴运行结果 → 系统比对预期输出 ✓
7. 查看得分 → 通过 → 解锁下一课时 ✓
8. 完成第1周所有课时 → 获得第1周章 ✓
```

---

## 6. 界面原型详细设计

### 6.1 主窗口布局

```
┌─────────────────────────────────────────────────────────────────┐
│  [🏰图标] TeenC++学院          积分: 150 | 徽章: 2 | [设置]     │  ─ TitleBar
├────────┬──────────────────────────────────────────────────────┤
│        │                                                      │
│  📚    │  ┌────────────────────────────────────────────────┐   │
│  课程  │  │                                                │   │
│  导航   │  │              主内容区域                         │   │
│        │  │                                                │   │
│  ─────  │  │    • 课程展示 / 代码编辑 / 提交面板             │   │
│  🎯    │  │    • 根据当前状态动态切换                       │   │
│  成就  │  │                                                │   │
│        │  └────────────────────────────────────────────────┘   │
│  ─────  │                                                      │
│  📊    │  ┌────────────────────────────────────────────────┐   │
│  进度  │  │  状态栏: 当前任务 | 学习时长 | 提示信息          │   │
│        │  └────────────────────────────────────────────────┘   │
└────────┴──────────────────────────────────────────────────────┘
     Sidebar                                              MainContent
```

### 6.2 课程详情页

```
┌─────────────────────────────────────────────────────────────────┐
│  <返回  第1周 · 勇者启程 · 新手村勇者                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────┐      │
│  │  🎭 故事：勇者的启程                                  │      │
│  │  ...... (故事内容)                                    │      │
│  └──────────────────────────────────────────────────────┘      │
│                                                                 │
│  📋 课时列表：                                                   │
│  ┌──────────────────────────────────────────────────────┐      │
│  │ [✓] 第1课 - 初识C++大陆  [15分]   (已完成)            │      │
│  │ [✓] 第2课 - 变量与背包   [25分]   (已完成)            │      │
│  │ [▶] 第3课 - 伤害计算     【当前】                     │      │
│  │ [🔒] 课后作业 - 勇者属性计算器                        │      │
│  └──────────────────────────────────────────────────────┘      │
│                                                                 │
│  🎮 周项目：                                                     │
│  ┌──────────────────────────────────────────────────────┐      │
│  │ 猜数字游戏 (基础版)     [进度: 2/4]  [继续挑战 ▶]      │      │
│  └──────────────────────────────────────────────────────┘      │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 6.3 代码提交面板

```
┌─────────────────────────────────────────────────────────────────┐
│  <返回    练习：制作「勇者属性计算器」                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  题目要求：                                                      │
│  ┌──────────────────────────────────────────────────────┐      │
│  │ 输入：基础攻击力、武器加成        输出：总攻击力       │      │
│  │ 必须使用：变量、cin、cout                            │      │
│  └──────────────────────────────────────────────────────┘      │
│                                                                 │
│  步骤1: 选择代码文件                                             │
│  ┌──────────────────────────────────────────────────────┐      │
│  │  📁 C:\C++Learning\Week1\practice2.cpp     [浏览...]  │      │
│  └──────────────────────────────────────────────────────┘      │
│                                                                 │
│  步骤2: 粘贴程序运行结果                                         │
│  ┌──────────────────────────────────────────────────────┐      │
│  │  请输入基础攻击力: 50                                 │      │
│  │  请输入武器加成: 20                                   │      │
│  │  总攻击力: 70                                         │      │
│  └──────────────────────────────────────────────────────┘      │
│                                                                 │
│  [运行检查]                                                      │
│                                                                 │
│  ┌──────────────────────────────────────────────────────┐      │
│  │  📊 检查结果                                          │      │
│  │  ─────────────────────────────────────────────────   │      │
│  │  ✓ 代码包含必要关键字: cout、cin、int      [+30]      │      │
│  │  ✓ 代码结构完整: 有main函数和include       [+20]      │      │
│  │  ✓ 输出格式正确: 包含"总攻击力"关键词       [+30]      │      │
│  │  ✓ 首次提交奖励                           [+10]      │      │
│  │  ─────────────────────────────────────────────────   │      │
│  │  总分: 90/100                                        │      │
│  │  🎉 恭喜！通过本练习！                               │      │
│  │              [确认完成]                              │      │
│  └──────────────────────────────────────────────────────┘      │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 7. 技术选型建议

### 7.1 技术方案对比

| 方案 | 技术栈 | 优点 | 缺点 | 推荐指数 |
|------|--------|------|------|----------|
| **A** | C# WinForms + SQLite | 开发快、文档多、部署简单 | 界面美观度一般 | ⭐⭐⭐⭐⭐ |
| **B** | C# WPF + SQLite | 界面漂亮、XAML灵活 | 学习曲线较陡 | ⭐⭐⭐⭐ |
| **C** | Python PyQt + SQLite | 跨平台、开发快 | 打包体积大、性能一般 | ⭐⭐⭐ |
| **D** | Electron + SQLite | Web技术、界面美观 | 内存占用高 | ⭐⭐ |

### 7.2 推荐方案：C# WinForms

**选型理由：**
1. **开发效率**：WinForms 拖拽式开发，适合快速原型
2. **生态成熟**：.NET Framework 在 Windows 上开箱即用
3. **部署简单**：单文件发布，无需安装运行时
4. **SQLite 支持**：System.Data.SQLite 库成熟稳定
5. **团队技能**：假设团队熟悉 C# 开发

**最低系统要求：**
- Windows 7 SP1+
- .NET Framework 4.6.1+
- 2GB RAM
- 100MB 磁盘空间

---

## 8. 下一步行动

### 8.1 开发任务拆分

```
Week 1: 基础框架搭建
├── Day 1-2: 项目创建、数据库设计、基础实体类
├── Day 3-4: 课程数据模型、JSON内容加载
└── Day 5: 主窗口、导航框架

Week 2: 核心功能实现
├── Day 1-2: 课程展示页面、内容渲染
├── Day 3-4: 代码提交面板、文件选择
└── Day 5: 代码检查器、评分逻辑

Week 3: 项目生成与验证
├── Day 1-2: 项目生成器、模板管理
├── Day 3-4: 里程碑验证、进度跟踪
└── Day 5: 成就系统、积分系统

Week 4: 完善与测试
├── Day 1-2: 第1-2周内容录入
├── Day 3-4: 端到端测试、BUG修复
└── Day 5: 打包发布、文档完善
```

### 8.2 待确认事项

请确认以下内容以继续详细设计：

1. **技术选型**：C# WinForms 是否可接受？还是你有偏好的技术栈？
2. **界面语言**：简体中文即可，是否需要繁体/英文预留？
3. **内容格式**：课程数据用 JSON 存储是否合适？还是倾向数据库存储一切？
4. **项目模板**：是否需要提供完整的 Dev-C++ 项目文件(.dev)或仅 .cpp 文件？
5. **检查严格度**：代码检查是"建议性"的还是"必须通过才能解锁"？
