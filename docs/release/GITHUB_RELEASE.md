# GitHub 发布指南

> 将项目发布到 GitHub 的完整步骤

---

## 准备工作

### 1. 创建 GitHub 仓库

访问 https://github.com/new 创建新仓库：
- **Repository name**: `TeenCppEdu` 或 `TeenagerCPlusPlusEduSystem`
- **Description**: 青少年 C++ 编程教学系统 - 游戏化学习平台
- **Visibility**: Public (推荐) 或 Private
- **Initialize**: 不勾选 (已有本地仓库)

创建后获取仓库地址：
```bash
# HTTPS
https://github.com/yourusername/TeenCppEdu.git

# SSH (推荐)
git@github.com:yourusername/TeenCppEdu.git
```

### 2. 关联远程仓库

```bash
# 添加远程仓库
git remote add origin https://github.com/yourusername/TeenCppEdu.git

# 验证
git remote -v
```

### 3. 推送代码

```bash
# 推送 main 分支
git push -u origin main

# 推送标签
git push origin v1.2.0

# 推送所有标签
git push origin --tags
```

---

## GitHub Release 创建

### 方法 1: Web 界面 (推荐)

1. 访问 `https://github.com/yourusername/TeenCppEdu/releases`
2. 点击 **"Draft a new release"**
3. 填写信息：

| 字段 | 内容 |
|------|------|
| Choose a tag | `v1.2.0` |
| Target | `main` |
| Release title | `TeenC++ v1.2.0 - Array Adventure` |
| Description | (见下方模板) |

4. 上传文件：
   - `releases/TeenCppEdu-v1.2.0.tar.gz`
   - 或单独上传 `releases/v1.2.0/TeenCppEdu.exe`

5. 勾选 **"Set as the latest release"**
6. 点击 **"Publish release"**

### Release 描述模板

```markdown
## 🎉 TeenC++ v1.2.0 正式发布

### ✨ 主要功能
- **第3课"数组探险"** - 全新的三阶段课程结构
  - 🧠 知识阶段 (60XP): 场景故事 + 概念讲解 + 测验 + 填空
  - 💻 实践阶段 (100XP): 数组编程任务
  - 🐛 挑战阶段 (30XP): Bug猎手模式

- **新格式课程系统** - Phase模型架构
- **阶段进度持久化** - 断点续学功能
- **新旧格式兼容** - L01/L02旧格式 + L03新格式

### 📦 系统要求
- Windows 10/11 (64位)
- .NET Framework 4.8

### 📥 安装使用
1. 下载 `TeenCppEdu-v1.2.0.tar.gz`
2. 解压到任意目录
3. 运行 `TeenCppEdu.exe`

### 📚 包含课程
- ✅ L01: 你好，C++！
- ✅ L02: 分支魔法
- ✅ L03: 数组探险 (新)

### 📋 质量验证
- [x] 编译 0 error
- [x] 单元测试 8/8 通过
- [x] 用户验收测试通过

---
**完整更新日志**: 参见 [CHANGELOG.md](CHANGELOG.md)

**项目文档**: 参见 [docs/](docs/) 目录
```

### 方法 2: GitHub CLI

```bash
# 安装 gh CLI: https://cli.github.com/

# 登录
gitHub auth login

# 创建 Release
gh release create v1.2.0 \
  --title "TeenC++ v1.2.0 - Array Adventure" \
  --notes-file releases/v1.2.0/RELEASE_NOTE.md \
  releases/TeenCppEdu-v1.2.0.tar.gz
```

---

## GitHub 仓库配置建议

### 1. 添加 README 徽章

编辑 `README.md` 添加：

```markdown
# TeenC++ 教学系统

[![Release](https://img.shields.io/github/v/release/yourusername/TeenCppEdu)](https://github.com/yourusername/TeenCppEdu/releases)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

青少年 C++ 编程教学系统 - 游戏化学习平台
```

### 2. 设置 Topics

在仓库页面点击右侧 **"About"** → **"⚙️"** 添加：
- `education`
- `cpp`
- `csharp`
- `winforms`
- `learning-platform`
- `noi`

### 3. 启用功能

Settings 中启用：
- ✅ Issues (问题反馈)
- ✅ Discussions (讨论区)
- ✅ Projects (项目管理)
- ✅ Wiki (文档)

### 4. 添加 LICENSE

```bash
# 创建 MIT License
echo "MIT License

Copyright (c) 2026 Carl

Permission is hereby granted..." > LICENSE
git add LICENSE
git commit -m "Add MIT License"
git push
```

或在 GitHub 创建时选择模板

---

## 发布后的工作

### 立即执行
- [ ] 验证 Release 页面可访问
- [ ] 测试下载链接有效
- [ ] 分享 Release 链接

### 本周内
- [ ] 收集 Issues 反馈
- [ ] 回复用户问题
- [ ] 更新项目介绍视频/GIF (可选)

---

## 快速命令速查

```bash
# 查看远程仓库
git remote -v

# 推送代码
git push origin main

# 推送所有标签
git push origin --tags

# 推送单个标签
git push origin v1.2.0

# 强制推送 (危险！)
git push origin main --force
```

---

*准备时间: 2026-04-04*  
*对应版本: v1.2.0*
