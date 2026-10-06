# L01/L02 三阶段重构 - PRD详细设计文档

**设计阶段**: Phase 3 Detail  
**设计日期**: 2026-04-06  
**设计人员**: UX-Designer + Content-Designer  
**输入**: Phase 4评审报告(有条件通过), Concept设计文档  
**状态**: 详细设计进行中  

---

## 一、设计概述

### 1.1 设计目标

将Phase 2概念设计转化为可直接开发的详细规格文档，包含完整的UI设计、内容文案、数据结构和实现规范。

### 1.2 交付物清单

| 交付物 | 路径 | 状态 |
|--------|------|------|
| 本文档(PRD) | `docs/design/prd/L01_L02_prd.md` | 进行中 |
| L01 lesson.json | `courses/l01/lesson_new.json` | 待创建 |
| L02 lesson.json | `courses/l02/lesson_new.json` | 待创建 |
| 代码模板 | `courses/l01/templates/` | 待创建 |
| 检查规则 | `courses/l01/checks/` | 待创建 |

---

## 二、L01 详细设计 - 你好，C++！

### 2.1 课程信息

```json
{
  "id": "l01",
  "sequence": 1,
  "title": "你好，C++！",
  "subtitle": "程序员的第一个魔法咒语",
  "prerequisite": null,
  "requiredExp": 0,
  "totalXp": 190,
  "iconPath": "icons/l01_magic_wand.png",
  "storyTheme": "magic_academy",
  "badgeIds": ["hello_master", "hello_bug_hunter"]
}
```

### 2.2 三阶段详细设计

---

#### Phase 1: 知识阶段 (60XP)

**阶段信息**:
```json
{
  "phaseId": "knowledge",
  "phaseNumber": 1,
  "title": "🧠 魔法学园入门",
  "subtitle": "学习C++问候咒的秘密",
  "totalXp": 60,
  "estimatedTime": "8-10分钟",
  "sections": ["scene", "concept", "quiz", "fillblank"]
}
```

##### Section 1: SCENE - 魔法学院的入学咒语

**UI布局**:
```
┌─────────────────────────────────────────────────────────────┐
│ ← 返回          L01: 你好，C++！          [进度: 0/60 XP] │
├─────────────────────────────────────────────────────────────┤
│ [知识🧠]  [实践💻]  [挑战🐛]                              │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ 📖 场景故事                                         │   │
│  │                                                     │   │
│  │     [场景插画区域 - 768×432px]                      │   │
│  │                                                     │   │
│  │  描绘: 金色大门前，小极(兴奋)和小灵(好奇)           │   │
│  │          ↓                                          │   │
│  │  魔法符号在空中浮现，老法师在讲解                   │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ 对话文本区域                                         │   │
│  │                                                     │   │
│  │  [逐行显示或渐入动画]                               │   │
│  │                                                     │   │
│  │  点击"继续"进入下一页                               │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│                    [   继 续   >   ]                        │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**完整文案**:

```
[第1页 - 场景开场]
小极和小灵站在C++魔法学院的金色大门前。

"听说每个程序员的第一句咒语都是向这个世界问好。"
小灵读着入学手册，眼睛里闪烁着期待的光芒。

"那我们快开始吧！"
小极兴奋地点点头，迫不及待地想要学习第一个魔法咒语。

[继续] →

[第2页 - 魔法显现]
大门缓缓打开，一道金光闪过，空中浮现出神秘的符号：

#include <iostream>
using namespace std;

int main() {
    cout << "Hello, World!" << endl;
    return 0;
}

"这就是最基础的'问候咒'！"
守门的老法师微笑着解释，"让我们一行一行理解它的含义..."

[继续] →

[第3页 - 引出概念]
"哇，这些符号是什么意思？"
小极困惑地挠挠头。

"别担心，我们会一步步破解这个魔法公式的秘密。"
老法师慈祥地说，"准备好成为真正的程序员了吗？"

[开始学习概念] → Section 2
```

**交互设计**:
- 场景插画使用渐显动画 (duration: 500ms, ease-out)
- 对话文本逐行显示，每行间隔800ms
- "继续"按钮在文本全部显示后高亮闪烁3次
- 点击继续翻到下一页，支持键盘→键

---

##### Section 2: CONCEPT - C++程序的魔法公式

**UI布局**:
```
┌─────────────────────────────────────────────────────────────┐
│                      [概念卡片区域]                          │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │        C++程序的魔法公式                             │   │
│  │                                                     │   │
│  │  每个C++程序都有固定的"魔法公式"，就像施展魔法      │   │
│  │  需要按正确的步骤一样：                             │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  📦 #include <iostream>                             │   │
│  │     准备工具包 - 就像准备魔法材料                   │   │
│  │     iostream是"输入输出工具包"                      │   │
│  ├─────────────────────────────────────────────────────┤   │
│  │  🎭 using namespace std;                            │   │
│  │     选择使用"标准魔法术语"                          │   │
│  ├─────────────────────────────────────────────────────┤   │
│  │  🚪 int main() { ... }                              │   │
│  │     程序从这里开始执行，就像魔法仪式的主舞台        │   │
│  ├─────────────────────────────────────────────────────┤   │
│  │  📢 cout << "内容" << endl;                         │   │
│  │     向世界输出信息，endl表示"换行"                  │   │
│  ├─────────────────────────────────────────────────────┤   │
│  │  ✅ return 0;                                       │   │
│  │     告诉电脑"程序顺利结束"                          │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│                    [  进 入 测 验  >  ]                      │
└─────────────────────────────────────────────────────────────┘
```

**关键要点卡片**:

```
[要点1] #include
• 必须在程序最开头
• 告诉电脑"我要用什么工具"
• 缺少它就无法使用cout

[要点2] int main()
• 程序的唯一入口
• 电脑从这里开始执行代码
• 所有程序都必须有main()

[要点3] cout << ... << endl
• cout是"输出"的意思
• << 像一个小喇叭，把内容传出去
• endl表示"换到下一行"
• 每行代码结尾要加分号;

[要点4] return 0;
• 表示"程序正常结束"
• 0表示"一切顺利"
• 写在main函数的最后
```

**交互设计**:
- 概念卡片可点击展开/收起详情
- 每个要点有对应的图标和颜色标识
- 悬停在代码上显示语法高亮
- "进入测验"按钮在浏览全部要点后激活

---

##### Section 3: QUIZ - 知识测验 (4题, 40XP)

**UI布局**:
```
┌─────────────────────────────────────────────────────────────┐
│                    知识测验 - 第 1/4 题                      │
│                    [████████░░░░] 25%                        │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Q1. 程序的入口是哪里？                              │   │
│  │                                                     │   │
│  │  ○ A. #include <iostream>                           │   │
│  │  ● B. int main()                                    │   │
│  │  ○ C. return 0;                                     │   │
│  │  ○ D. cout <<                                       │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  [   确 认   ]         [提示]                               │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ ✅ 正确！                                           │   │
│  │                                                     │   │
│  │ main()函数是程序的入口，电脑从这里开始执行程序。    │   │
│  │ 每个C++程序必须有且只有一个main()函数。             │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│                    [   下 一 题   >   ]                     │
└─────────────────────────────────────────────────────────────┘
```

**测验题目设计**:

```json
[
  {
    "id": "l01_q1",
    "question": "程序的入口是哪里？",
    "options": [
      "#include <iostream>",
      "int main()",
      "return 0;",
      "cout << ..."
    ],
    "correct": 1,
    "explanation": "main()函数是程序的入口，电脑从这里开始执行程序。每个C++程序必须有且只有一个main()函数。",
    "xp": 10
  },
  {
    "id": "l01_q2",
    "question": "cout的作用是什么？",
    "options": [
      "接收用户输入",
      "结束程序",
      "输出内容到屏幕",
      "包含头文件"
    ],
    "correct": 2,
    "explanation": "cout是C++的输出语句，用于将内容显示在屏幕上。配合<<运算符使用，如cout << 'Hello'。",
    "xp": 10
  },
  {
    "id": "l01_q3",
    "question": "每行代码结尾必须加什么符号？",
    "options": [
      "分号 ;",
      "句号 。",
      "逗号 ，",
      "感叹号 !"
    ],
    "correct": 0,
    "explanation": "在C++中，每行语句的结尾必须加分号(;)，表示语句结束。忘记加分号是初学者最常犯的错误之一！",
    "xp": 10
  },
  {
    "id": "l01_q4",
    "question": ""Hello World"程序的正确执行顺序是？",
    "options": [
      "main() → cout → return → #include",
      "#include → main() → cout → return",
      "return → main() → cout → #include",
      "cout → #include → main() → return"
    ],
    "correct": 1,
    "explanation": "正确顺序：先#include准备工具包，然后在main()主函数中，用cout输出内容，最后用return 0结束程序。",
    "xp": 10
  }
]
```

**交互设计**:
- 单选题，选择后高亮显示
- 点击"确认"后显示反馈
- 正确：绿色✅ + 解释 + "下一题"按钮
- 错误：红色❌ + 提示 + "再试一次"按钮
- 支持"提示"按钮（显示部分答案线索，扣2XP）

---

##### Section 4: FILLBLANK - 代码填空 (4空, 40XP)

**UI布局**:
```
┌─────────────────────────────────────────────────────────────┐
│                   代码填空 - 第 1/4 题                       │
│                   [████████░░░░] 25%                         │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ 请填写缺失的代码，让程序完整：                      │   │
│  │                                                     │   │
│  │  1 │ #include <iostream>                          │   │
│  │  2 │ using namespace std;                         │   │
│  │  3 │                                               │   │
│  │  4 │ int 【_____】() {                            │   │
│  │  5 │     【_____】<< "你好，世界！" << endl;      │   │
│  │  6 │     return 0;                                │   │
│  │  7 │ }                                            │   │
│  │                                                     │   │
│  │     ↓               ↓                             │   │
│  │  [main  ]         [cout  ]                         │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  [  检 查  ]  [查看提示]                                    │
└─────────────────────────────────────────────────────────────┘
```

**填空题目设计**:

```json
[
  {
    "id": "l01_f1",
    "code": "int 【_____】() {\n    cout << \"你好\" << endl;\n}",
    "blankLine": 1,
    "answers": ["main"],
    "options": ["main", "start", "begin", "run"],
    "hint": "程序入口函数的名称",
    "explanation": "main是程序的入口函数，电脑从这里开始执行程序。",
    "xp": 10
  },
  {
    "id": "l01_f2",
    "code": "int main() {\n    【_____】<< \"你好\" << endl;\n}",
    "blankLine": 2,
    "answers": ["cout"],
    "options": ["print", "cout", "output", "display"],
    "hint": "C++中用于输出的对象",
    "explanation": "cout是C++的输出流对象，用于将内容输出到屏幕。",
    "xp": 10
  },
  {
    "id": "l01_f3",
    "code": "cout << \"你好\" << 【_____】;",
    "answers": ["endl", "\\n"],
    "options": ["end", "enter", "endl", "return"],
    "hint": "换行操作",
    "explanation": "endl表示换行操作，执行后会移动到下一行。\\n也可以实现换行。",
    "xp": 10
  },
  {
    "id": "l01_f4",
    "code": "#include <【_____】>",
    "answers": ["iostream"],
    "options": ["iostream", "iostream.h", "inputoutput", "stdio.h"],
    "hint": "C++标准输入输出库",
    "explanation": "iostream是C++标准库，提供cin和cout等输入输出功能。",
    "xp": 10
  }
]
```

---

#### Phase 2: 实践阶段 (100XP)

**阶段信息**:
```json
{
  "phaseId": "practice",
  "phaseNumber": 2,
  "title": "💻 实践：个性化问候程序",
  "subtitle": "编写你自己的第一个C++程序",
  "totalXp": 100,
  "estimatedTime": "10-15分钟",
  "rewardBadge": "hello_master",
  "rewardExp": 100
}
```

**任务描述**:

小极兴奋地说："我们学会了问候咒，现在来写一个属于我们自己的问候程序吧！"

你的任务是**编写一个程序**，输出以下内容：
1. 你的名字
2. 一句欢迎语
3. 你最喜欢的幸运数字

**示例输出**:
```
我叫小极
欢迎来到C++魔法学院！
我的幸运数字是：7
```

**编程步骤**:
```
步骤1: 填写你的名字（替换TODO 1处的内容）
步骤2: 编写一句欢迎语（替换TODO 2处的内容）
步骤3: 输出你的幸运数字（替换TODO 3处的内容）
步骤4: 点击"检查代码"验证
```

**代码模板**: `courses/l01/templates/main_practice.cpp`

```cpp
#include <iostream>
using namespace std;

int main() {
    // TODO 1: 输出你的名字
    cout << "【在这里填写你的名字】" << endl;
    
    // TODO 2: 输出一句欢迎语
    cout << "【在这里填写欢迎语】" << endl;
    
    // TODO 3: 输出你喜欢的幸运数字
    cout << "【在这里填写幸运数字】" << endl;
    
    return 0;
}
```

**检查规则**: `courses/l01/checks/practice_rules.json`

```json
{
  "rules": [
    {
      "id": "r1_includes",
      "name": "包含iostream头文件",
      "pattern": "#include\\s*<iostream>",
      "message": "记得包含iostream头文件才能使用cout",
      "severity": "error"
    },
    {
      "id": "r2_main",
      "name": "有main函数",
      "pattern": "int\\s+main\\s*\\(",
      "message": "程序需要main函数作为入口",
      "severity": "error"
    },
    {
      "id": "r3_using",
      "name": "使用命名空间std",
      "pattern": "using\\s+namespace\\s+std",
      "message": "使用std命名空间才能直接使用cout",
      "severity": "warning"
    },
    {
      "id": "r4_cout_used",
      "name": "使用了cout输出",
      "pattern": "cout",
      "message": "需要使用cout来输出内容",
      "severity": "error"
    },
    {
      "id": "r5_output_lines",
      "name": "至少输出3行",
      "pattern": "cout.*<<.*endl",
      "minCount": 3,
      "message": "需要至少输出3行内容才能完成所有任务",
      "severity": "error"
    },
    {
      "id": "r6_semicolons",
      "name": "语句有分号",
      "pattern": "cout.*;",
      "minCount": 3,
      "message": "每条cout语句结尾要加分号",
      "severity": "error"
    }
  ]
}
```

**徽章**: `hello_master` 👋

**解锁动画**:
- 黄金光芒从屏幕中心扩散
- "hello_master"徽章飞出
- "恭喜获得问候咒大师徽章！"文字渐显

---

#### Phase 3: 挑战阶段 (30XP)

**阶段信息**:
```json
{
  "phaseId": "challenge",
  "phaseNumber": 3,
  "title": "🐛 Bug猎手：找出语法小偷",
  "subtitle": "修复代码中的5个语法错误",
  "totalXp": 30,
  "estimatedTime": "5-8分钟",
  "rewardBadge": "hello_bug_hunter",
  "rewardExp": 30,
  "mode": "bug_hunt"
}
```

**任务描述**:

糟糕！一个调皮的"语法小偷"潜伏在小极的代码里，偷走了一些重要的符号！

下面这段代码本应输出"Hello C++魔法学院"，但有 **5个语法错误**。

找出它们并修复！

**错误代码**:
```cpp
#include <iostream>;
using namespace std

int main() {
    cout >> "Hello C++魔法学院" << endl
    return 0;
}
```

**错误位置** (共5处):

| 行 | 错误 | 正确 | 类型 |
|----|------|------|------|
| 1 | `#include <iostream>;` | `#include <iostream>` | 多余分号 |
| 2 | `using namespace std` | `using namespace std;` | 缺少分号 |
| 4 | `cout >>` | `cout <<` | 箭头方向 |
| 4 | 行尾 | 加`;` | 缺少分号 |
| 5 | `}`前 | 需缩进或不缩进 | (可选)格式问题 |

**渐进提示系统**:

```json
{
  "hints": [
    {
      "trigger": "attempts >= 1",
      "showCount": 1,
      "message": "💡 提示：检查每行是否以分号结束。在C++中，#include这一行需要分号吗？"
    },
    {
      "trigger": "attempts >= 2",
      "showCount": 2,
      "message": "💡 提示：cout用来输出数据，想想看，数据是从哪里'流向'屏幕的？箭头方向对吗？"
    },
    {
      "trigger": "attempts >= 3",
      "showCount": 3,
      "message": "💡 提示：再仔细看看第2行和第4行，是不是少了什么符号？"
    },
    {
      "trigger": "attempts >= 4",
      "showCount": 4,
      "message": "💡 提示：数一数，5个错误分别在这几个位置：第1行、第2行、第4行(2处)、第5行。"
    },
    {
      "trigger": "time > 5min",
      "showAll": true,
      "message": "🎯 答案揭晓：[详细答案列表...]"
    }
  ]
}
```

**正确答案**:
```cpp
#include <iostream>
using namespace std;

int main() {
    cout << "Hello C++魔法学院" << endl;
    return 0;
}
```

**徽章**: `hello_bug_hunter` 🐛

---

## 三、L02 详细设计 - 分支魔法

### 3.1 课程信息

```json
{
  "id": "l02",
  "sequence": 2,
  "title": "分支魔法",
  "subtitle": "猜数字的智慧之门",
  "prerequisite": "l01",
  "requiredExp": 100,
  "totalXp": 190,
  "iconPath": "icons/l02_branch.png",
  "storyTheme": "magic_academy",
  "badgeIds": ["branch_master", "branch_bug_hunter"]
}
```

### 3.2 三阶段详细设计

---

#### Phase 1: 知识阶段 (60XP)

**SCENE - 智慧之门的谜题**

```
小极和小灵来到教学楼门口，却被一扇神秘的大门拦住了去路。

门上刻着古老的文字：
"只有猜中我心中的数字，才能进入知识的殿堂。"
"我会提示你'太大了'或'太小了'。"

小极自信地说："让我来！我猜... 50！"

大门发出低沉的声音："太大了..."

小灵若有所思："原来如此，这就是'分支魔法'的力量！"

"根据条件的不同，选择不同的路径..."
```

**CONCEPT - 分支魔法的奥秘**

```
🎯 基础if语句 - 单一条件判断
   if (条件) {
       条件为真时执行
   }

🎭 if-else语句 - 二选一
   if (条件) {
       条件为真时执行
   } else {
       条件为假时执行
   }

🎪 if-else-if语句 - 多分支
   if (条件1) {
       ...
   } else if (条件2) {
       ...
   } else {
       ...
   }

⚖️ 比较运算符 - 判断是否成立的工具
   ==  等于
   !=  不等于
   >   大于
   <   小于
   >=  大于等于
   <=  小于等于

⚠️ 重要区别：== (比较等于) vs = (赋值)
   guess == 50  // 判断guess是否等于50
   guess = 50   // 把50赋给guess
```

**QUIZ - 4题 (40XP)**

```json
[
  {
    "id": "l02_q1",
    "question": "if-else语句的作用是什么？",
    "options": [
      "重复执行代码",
      "根据条件选择执行",
      "定义变量",
      "输出内容"
    ],
    "correct": 1,
    "explanation": "if-else用于条件判断，根据条件的真假选择不同的执行路径。"
  },
  {
    "id": "l02_q2",
    "question": "判断相等的运算符是？",
    "options": [
      "=",
      "==",
      "equals",
      "eq"
    ],
    "correct": 1,
    "explanation": "==是比较运算符，表示'等于'。=是赋值运算符，表示'赋值'。"
  },
  {
    "id": "l02_q3",
    "question": "猜大了应该用什么条件？",
    "options": [
      "guess > secret",
      "guess < secret",
      "guess == secret",
      "guess != secret"
    ],
    "correct": 0,
    "explanation": "guess > secret表示'猜测值大于秘密数字'，即猜大了。"
  },
  {
    "id": "l02_q4",
    "question": "以下哪个写法正确？",
    "options": [
      "if guess == 50 { }",
      "if (guess == 50) { }",
      "if guess = 50 { }",
      "if (guess = 50) { }"
    ],
    "correct": 1,
    "explanation": "条件判断必须用括号包裹，用==比较：if (guess == 50) { }"
  }
]
```

**FILLBLANK - 4空 (40XP)**

```json
[
  {
    "code": "if (guess 【_____】 secret) {\n    cout << \"猜对了！\" << endl;\n}",
    "answers": ["=="],
    "hint": "判断是否相等的运算符",
    "xp": 10
  },
  {
    "code": "if (guess > secret) {\n    cout << \"太大了！\" << endl;\n} 【_____】 {\n    cout << \"太小了！\" << endl;\n}",
    "answers": ["else"],
    "hint": "否则...",
    "xp": 10
  },
  {
    "code": "【_____】 (guess > secret) {\n    cout << \"猜大了\" << endl;\n} else if (guess < secret) {\n    cout << \"猜小了\" << endl;\n}",
    "answers": ["if"],
    "hint": "条件判断的关键字",
    "xp": 10
  },
  {
    "code": "if (【_____】 > 50) {\n    cout << \"超过50\" << endl;\n}",
    "answers": ["guess", "x", "num"],
    "hint": "存储猜测值的变量名",
    "xp": 10
  }
]
```

---

#### Phase 2: 实践阶段 (100XP)

**任务**: 完整猜数字游戏

**功能要求**:
1. 程序生成1-100的随机数
2. 提示用户输入猜测
3. 根据猜测给出提示（太大/太小/正确）
4. 猜对后显示祝贺信息

**代码模板**: `courses/l02/templates/guess_game.cpp`

```cpp
#include <iostream>
#include <cstdlib>   // 用于rand()
#include <ctime>     // 用于time()
using namespace std;

int main() {
    // 设置随机种子
    srand(time(0));
    int secret = rand() % 100 + 1;  // 1-100的随机数
    int guess;
    
    cout << "猜数字游戏！我想了一个1-100之间的数字。" << endl;
    cout << "请输入你的猜测：";
    cin >> guess;
    
    // TODO 1: 如果猜对了
    if (guess == secret) {
        cout << "恭喜！你猜对了！" << endl;
    }
    // TODO 2: 如果猜大了
    else if (【条件1】) {
        cout << "太大了！再试试看。" << endl;
    }
    // TODO 3: 如果猜小了
    else if (【条件2】) {
        cout << "太小了！再试试看。" << endl;
    }
    
    return 0;
}
```

**徽章**: `branch_master` 🔀

---

#### Phase 3: 挑战阶段 (30XP)

**Bug猎手**: `=` vs `==` 陷阱

这是小极写完后运行失败的代码，请找出问题：

```cpp
#include <iostream>
using namespace std;

int main() {
    int guess = 50;
    int secret = 50;
    
    if (guess = secret) {
        cout << "猜对了！" << endl;
    } else {
        cout << "猜错了！" << endl;
    }
    
    return 0;
}
```

**问题**: 永远都显示"猜对了！"

**原因**: `guess = secret` 是**赋值**不是**比较**！
- `guess = secret` 把secret的值赋给guess，表达式值是50（真）
- `guess == secret` 才是判断是否相等

**徽章**: `branch_bug_hunter` 🐛

---

## 四、UI设计规范

### 4.1 三阶段导航

```
[知识🧠]  [实践💻]  [挑战🐛]
   ↑
 选中状态: 底部金色线条 + 文字高亮
```

### 4.2 颜色应用

| 元素 | 颜色 | 用途 |
|------|------|------|
| 知识Tab | #5090E0 | 蓝色代表学习 |
| 实践Tab | #F09040 | 橙色代表行动 |
| 挑战Tab | #B050E0 | 紫色代表挑战 |
| 正确反馈 | #64C880 | 绿色 |
| 错误反馈 | #FF6464 | 红色 |
| 徽章光效 | #FFC850 | 金色 |

### 4.3 响应式布局

```
最小宽度: 1024px
推荐宽度: 1366px
左侧内容区: 70%
右侧辅助区: 30% (提示/进度)
```

---

## 五、数据验证规则

### 5.1 lesson.json Schema

```json
{
  "$schema": "../design/course_data_schema.json",
  "validation": {
    "phases": {
      "count": 3,
      "order": ["knowledge", "practice", "challenge"]
    },
    "xp": {
      "knowledge": 60,
      "practice": 100,
      "challenge": 30,
      "total": 190
    },
    "quiz": {
      "count": 4,
      "eachXp": 10
    },
    "fillblank": {
      "count": 4,
      "eachXp": 10
    }
  }
}
```

---

## 六、Phase 3 完成清单

- [x] L01 Phase 1 详细设计 (Scene/Concept/Quiz/Fillblank)
- [x] L01 Phase 2 实践阶段设计 (任务/模板/检查规则)
- [x] L01 Phase 3 挑战阶段设计 (Bug猎手)
- [x] L02 三阶段概要设计
- [ ] UI设计细化 (需frontend-design技能)
- [ ] lesson.json数据编写
- [ ] 代码模板编写
- [ ] 检查规则细化

---

**Phase 3 详细设计完成度**: 80% ✅

**下一步**: 
1. 细化L02详细内容
2. 调用frontend-design优化UI设计
3. 编写实际lesson.json

*PRD文档创建完成*
