# L01/L02 课程界面重设计方案

## 概述

使用 frontend-design 优化 L01/L02 课程界面，解决布局偏右、按钮遮挡、解锁显示等问题。

---

## 一、当前问题诊断

### 1.1 布局问题

| 问题 | 现象 | 根因 | 解决方案 |
|------|------|------|----------|
| **整体偏右** | 内容偏向右上方 | Padding/Margin 或 Anchor 设置 | 居中布局 + 左对齐 |
| **按钮遮挡** | 底部按钮显示不全 | 窗体高度不足或 Dock 设置 | 底部区域固定高度 + Padding |
| **分辨率不适配** | 不同分辨率布局错乱 | 固定坐标 | Anchor + Dock + 最小尺寸 |
| **解锁显示问题** | 通后后按钮仍为灰色 | 状态刷新逻辑 | 数据绑定 + 实时更新 |

### 1.2 设计系统应用

参考 `docs/design/DESIGN_SYSTEM.md`

```
主背景:   #2D3447 (深蓝)
卡片:     #383F50 (次级背景)
强调:     #FFC850 (金色)
成功:     #64C880 (绿色)
错误:     #FF6464 (红色)

阶段配色:
- 知识: #5090E0 (蓝色)
- 实践: #F09040 (橙色)
- 挑战: #B050E0 (紫色)
```

---

## 二、布局重构方案

### 2.1 整体布局

```
┌─────────────────────────────────────────────────────────────┐
│ Header (60px)                                               │
│ ←返回  课程标题                          [XP] [徽章]         │
├─────────────────────────────────────────────────────────────┤
│ Navigation (50px)                                           │
│ [知识🧠]  [实践💻]  [挑战🐛]  ← Tab导航 (居中)               │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│ Content Area (自适应)                                       │
│ ┌──────────────────────────────────────────────────────┐   │
│ │                                                      │   │
│ │   阶段内容                                            │   │
│ │   - 知识: 场景故事 + 概念 + 测验/填空                  │   │
│ │   - 实践: 任务描述 + 代码编辑器                        │   │
│ │   - 挑战: Bug猎手界面                                 │   │
│ │                                                      │   │
│ └──────────────────────────────────────────────────────┘   │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│ Footer (80px) ← 固定高度，防止遮挡                         │
│ ┌──────────────────────────────────────────────────────┐   │
│ │          [检查代码]  [生成项目]  [导师放行]           │   │
│ └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 WinForms 布局实现

```csharp
// MainLayout Panel
mainPanel.Dock = DockStyle.Fill;
mainPanel.BackColor = Color.FromArgb(45, 52, 71); // #2D3447

// Header
headerPanel.Dock = DockStyle.Top;
headerPanel.Height = 60;

// Navigation
navPanel.Dock = DockStyle.Top;
navPanel.Height = 50;

// Content (填充)
contentPanel.Dock = DockStyle.Fill;

// Footer (固定高度)
footerPanel.Dock = DockStyle.Bottom;
footerPanel.Height = 80;
footerPanel.Padding = new Padding(16); // 防止按钮贴边
```

---

## 三、三阶段导航设计

### 3.1 Tab 样式

```csharp
// 三个阶段按钮
Size: 120x36
Margin: 8px
Font: Microsoft YaHei, 12px
BorderRadius: 4px (通过 Panel 实现)

// 知识按钮 (蓝色)
知识按钮.BackColor = Color.FromArgb(80, 144, 224);
知识按钮.ForeColor = Color.White;
知识按钮.Tag = "knowledge";

// 实践按钮 (橙色)
实践按钮.BackColor = Color.FromArgb(240, 144, 64);
实践按钮.ForeColor = Color.White;
实践按钮.Tag = "practice";

// 挑战按钮 (紫色)
挑战按钮.BackColor = Color.FromArgb(176, 80, 224);
挑战按钮.ForeColor = Color.White;
挑战按钮.Tag = "challenge";
```

### 3.2 选中状态

```csharp
// 选中时增加边框发光效果
选中按钮.BorderStyle = BorderStyle.FixedSingle;
选中按钮.Padding = new Padding(2);

// 非选中按钮降低透明度
非选中按钮.BackColor = Color.FromArgb(
    原色.R / 2,
    原色.G / 2,
    原色.B / 2
);
```

### 3.3 进度指示

```
在 Tab 上显示完成状态:

[知识🧠 ✓]  [实践💻]  [挑战🐛 🔒]
              ↑
          当前进行

✓ = 已完成 (绿色)
  = 进行中 (高亮)
🔒 = 未解锁 (灰色)
```

---

## 四、知识阶段 UI

### 4.1 场景故事面板

```
┌────────────────────────────────────────────────────────┐
│ 🎬 场景：魔法学院的入门咒语                            │
├────────────────────────────────────────────────────────┤
│                                                        │
│  小极和小灵站在魔法学院的大门前...                     │
│                                                        │
│  「这就是最基础的'问候咒'！」...                       │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**样式**:
```csharp
scenePanel.BackColor = Color.FromArgb(56, 63, 80);
scenePanel.Padding = new Padding(16);
scenePanel.BorderStyle = BorderStyle.None;
// 底部添加金色装饰线
scenePanel.Height = 200;
```

### 4.2 概念卡片

```
┌────────────────────────────────────────────────────────┐
│ 📚 概念：C++程序的魔法公式                             │
├────────────────────────────────────────────────────────┤
│                                                        │
│ ┌──────────────────────────────────────────────────┐  │
│ │ 📦 第1步：准备工具包                              │  │
│ │ #include <iostream>                              │  │
│ │ → 就像准备魔法材料...                            │  │
│ └──────────────────────────────────────────────────┘  │
│                                                        │
│ ┌──────────────────────────────────────────────────┐  │
│ │ 🎭 第2步：命名空间                                │  │
│ │ ...                                              │  │
│ └──────────────────────────────────────────────────┘  │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**卡片样式**:
```csharp
conceptCard.BackColor = Color.FromArgb(45, 52, 71);
conceptCard.Padding = new Padding(12);
conceptCard.Margin = new Padding(0, 8, 0, 8);
conceptCard.BorderStyle = BorderStyle.None;
// 左边添加彩色边框
左侧边框.BackColor = Color.FromArgb(80, 144, 224); // 知识蓝
左侧边框.Width = 4;
左侧边框.Dock = DockStyle.Left;
```

### 4.3 测验界面

```
┌────────────────────────────────────────────────────────┐
│ 📋 知识测验 (1/3)                                      │
├────────────────────────────────────────────────────────┤
│                                                        │
│  问题：程序从哪里开始执行？                            │
│                                                        │
│  ○ A. 从第一行#include开始                          │
│  ● B. 从main()函数开始  ← 选中状态                   │
│  ○ C. 从return 0开始                                │
│  ○ D. 从cout开始                                    │
│                                                        │
│  [提交答案]                                            │
│                                                        │
│  ✅ 回答正确！ +10 XP                                  │
│  解释：main()是程序的入口点...                        │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**选项样式**:
```csharp
optionButton.Size = new Size(400, 40);
optionButton.BackColor = Color.FromArgb(56, 63, 80);
optionButton.FlatStyle = FlatStyle.Flat;
optionButton.FlatAppearance.BorderSize = 1;
optionButton.FlatAppearance.BorderColor = Color.FromArgb(96, 104, 120);

// 选中
选中.BackColor = Color.FromArgb(80, 144, 224);
选中.FlatAppearance.BorderColor = Color.FromArgb(255, 200, 80);

// 正确反馈
正确.BackColor = Color.FromArgb(100, 200, 128);
```

---

## 五、实践阶段 UI

### 5.1 任务描述面板

```
┌────────────────────────────────────────────────────────┐
│ 💻 实践：编写个性化问候程序                            │
│ 奖励：100 XP | 徽章：hello_master                     │
├────────────────────────────────────────────────────────┤
│                                                        │
│  恭喜你掌握了基础咒语！                                │
│                                                        │
│  任务要求：                                            │
│  1. 输出你的名字                                       │
│  2. 输出一句欢迎语                                     │
│  3. 输出你最喜欢的一个数字                             │
│                                                        │
│  示例输出：                                            │
│  ╔════════════════════════╗                           │
│  ║ 你好，我是小极！      ║                           │
│  ║ 欢迎来到C++魔法世界！ ║                           │
│  ║ 我的幸运数字是：7     ║                           │
│  ╚════════════════════════╝                           │
│                                                        │
└────────────────────────────────────────────────────────┘
```

### 5.2 代码编辑器

```
┌────────────────────────────────────────────────────────┐
│ 📝 代码编辑                                            │
├────────────────────────────────────────────────────────┤
│                                                        │
│  1 │ #include <iostream>                               │
│  2 │ using namespace std;                              │
│  3 │                                                   │
│  4 │ int main() {                                      │
│  5 │     // TODO 1: 输出你的名字                         │
│  6 │                                                   │
│  7 │     // TODO 2: 输出一句欢迎语                       │
│  8 │                                                   │
│  9 │     // TODO 3: 输出你最喜欢的一个数字               │
│ 10 │                                                   │
│ 11 │     return 0;                                     │
│ 12 │ }                                                 │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**编辑器样式**:
```csharp
editor.BackColor = Color.FromArgb(30, 36, 48); // #1E2430
editor.ForeColor = Color.FromArgb(212, 212, 212);
editor.Font = new Font("Consolas", 14);
editor.BorderStyle = BorderStyle.FixedSingle;
editor.Padding = new Padding(12);

// TODO 标记高亮
TODO.ForeColor = Color.FromArgb(255, 200, 80); // 金色
```

### 5.3 底部按钮栏

```
┌────────────────────────────────────────────────────────┐
│                                                     🎮  │
│  [检查代码]  [生成项目]  [导师放行]                    │
│  (↑金色)   (↑蓝色)   (↑灰色)                        │
└────────────────────────────────────────────────────────┘
```

**按钮样式**:
```csharp
// 检查代码 (主按钮 - 金色)
checkBtn.BackColor = Color.FromArgb(255, 200, 80);
checkBtn.ForeColor = Color.FromArgb(35, 41, 56);
checkBtn.Font = new Font("Microsoft YaHei", 14, FontStyle.Bold);
checkBtn.Size = new Size(140, 44);
checkBtn.FlatStyle = FlatStyle.Flat;

// 生成项目 (次按钮 - 蓝色)
generateBtn.BackColor = Color.FromArgb(80, 144, 224);
generateBtn.ForeColor = Color.White;

// 导师放行 (辅助按钮)
skipBtn.BackColor = Color.FromArgb(64, 72, 88);
skipBtn.ForeColor = Color.FromArgb(176, 184, 192);
```

---

## 六、挑战阶段 UI (Bug猎手)

### 6.1 Bug猎手界面

```
┌────────────────────────────────────────────────────────┐
│ 🐛 Bug猎手：找出语法小偷                               │
│ 奖励：20 XP | 徽章：hello_bug_hunter                   │
├────────────────────────────────────────────────────────┤
│                                                        │
│  糟糕！一个调皮的"语法小偷"偷走了重要符号！             │
│                                                        │
│ ┌──────────────────────────────────────────────────┐  │
│ │ 📝 问题代码                                       │  │
│ │ 1  │ #include <iostream>                          │  │
│ │ 2  │ using namespace std                          │  │
│ │ 3  │                                              │  │
│ │ 4  │ int main()                                   │  │
│ │ 5  │     cout >> "Hello" << endl                  │  │
│ │ 6  │     return 0                                 │  │
│ │ 7  │ }                                            │  │
│ └──────────────────────────────────────────────────┘  │
│                                                        │
│  点击错误行，输入你的修复：                            │
│  ┌────────────────────────────────────────────────┐   │
│  │ using namespace std;                           │   │
│  └────────────────────────────────────────────────┘   │
│                                                        │
│  [提交修复]  [获取提示 💡]                             │
│                                                        │
│  ✅ 全部修复正确！                                     │
│  发现 3/3 个Bug                                        │
│                                                        │
└────────────────────────────────────────────────────────┘
```

### 6.2 行选择交互

```csharp
// 每一行代码是一个Panel
linePanel.Height = 28;
linePanel.BackColor = Color.FromArgb(30, 36, 48);
linePanel.Cursor = Cursors.Hand;

// 悬停效果
linePanel.MouseEnter += (s, e) => {
    linePanel.BackColor = Color.FromArgb(48, 56, 72);
};

// 选中状态
linePanel.Click += (s, e) => {
    linePanel.BackColor = Color.FromArgb(255, 100, 100, 0.3); // 红色半透明
    linePanel.BorderStyle = BorderStyle.FixedSingle;
    linePanel.BorderColor = Color.FromArgb(255, 100, 100);
};
```

---

## 七、分辨率适配方案

### 7.1 断点设计

```csharp
// 最小支持分辨率
MinimumSize = new Size(1024, 768);

// 窗体默认大小 (标准桌面)
Size = new Size(1280, 800);

// 高分屏适配 (缩放处理)
// 使用 AutoScaleMode.Dpi
this.AutoScaleMode = AutoScaleMode.Dpi;
```

### 7.2 响应式规则

| 分辨率 | 处理策略 |
|--------|----------|
| < 1280×768 | 显示滚动条，确保最小可用 |
| 1280-1600 | 标准布局 |
| 1600-1920 | 增加间距，更宽松 |
| > 1920 | 保持最大宽度，居中显示 |

### 7.3 测试矩阵

```
□ 1024×768  (上网本) - 最小可用
□ 1366×768  (笔记本) - 主要适配
□ 1920×1080 (标准)   - 最佳效果
□ 2560×1440 (高分)   - 缩放测试
□ 125% DPI 缩放
□ 150% DPI 缩放
```

---

## 八、微交互动效

### 8.1 阶段切换动画

```csharp
// 切换时淡入效果
contentPanel.Opacity = 0;
timer = new Timer();
timer.Interval = 16; // 60fps
timer.Tick += (s, e) => {
    contentPanel.Opacity += 0.1;
    if (contentPanel.Opacity >= 1) timer.Stop();
};
timer.Start();
```

### 8.2 XP 增长动画

```csharp
// XP 数字滚动
startXP = currentXP;
targetXP = currentXP + earnedXP;
animationDuration = 500ms;

// 使用 Timer 逐步增加显示值
displayedXP = Lerp(startXP, targetXP, progress);
xpLabel.Text = $"+{displayedXP} XP";
```

### 8.3 徽章解锁效果

```csharp
// 金色光芒背景
badgePanel.BackColor = Color.FromArgb(255, 200, 80);
// 脉冲动画
animation = new Animation(badgePanel, "BackColor", 
    Color.FromArgb(255, 200, 80), 
    Color.FromArgb(56, 63, 80),
    500);
```

---

## 九、实施清单

### 9.1 ModernLessonForm 优化

- [ ] 重构布局结构 (Header/Nav/Content/Footer)
- [ ] 实现三阶段 Tab 导航
- [ ] 修复整体偏右问题 (居中/左对齐)
- [ ] 底部按钮区域固定高度 80px
- [ ] 添加分辨率适配逻辑

### 9.2 KnowledgePhaseForm 适配

- [ ] 场景故事面板样式
- [ ] 概念卡片样式
- [ ] 测验选项样式
- [ ] 填空交互样式

### 9.3 测试验证

- [ ] 1366×768 分辨率测试
- [ ] 1920×1080 分辨率测试
- [ ] 高 DPI 缩放测试
- [ ] 三阶段切换流畅度
- [ ] L01/L02 完整通关测试

---

*UI设计文档完成*
*下一步: 开始编码实施*