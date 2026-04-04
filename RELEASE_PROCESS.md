# TeenC++ 标准化发布流程

> 从开发完成到正式发布的一整套标准化操作流程

---

## 流程概览

```
┌─────────────────────────────────────────────────────────────────┐
│                    TeenC++ 发布流程 v1.0                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  第一阶段: 代码冻结与准备 (发布前3天)                              │
│  ├── 创建发布分支 release/v{x}.{y}.{z}                          │
│  ├── 更新版本号 (AssemblyInfo.cs)                               │
│  ├── 更新 CHANGELOG.md                                          │
│  └── 提交并推送发布分支                                          │
│                                                                 │
│  第二阶段: 自动化构建 (发布前2天)                                  │
│  ├── 执行 publish.ps1 脚本                                       │
│  ├── 编译检查 (0 error)                                         │
│  ├── 单元测试 (100%通过)                                         │
│  └── 文件完整性验证                                              │
│                                                                 │
│  第三阶段: 人工验证 (发布前1天)                                    │
│  ├── 干净环境测试 (虚拟机/新目录)                                 │
│  ├── 核心功能回归测试                                            │
│  ├── 课程数据验证                                               │
│  └── 签名确认                                                   │
│                                                                 │
│  第四阶段: 正式发布 (发布日)                                       │
│  ├── 合并发布分支到 main/master                                 │
│  ├── 创建 Git 标签 v{x}.{y}.{z}                                  │
│  ├── 生成发布包压缩文件                                          │
│  ├── 创建 GitHub Release (可选)                                  │
│  └── 更新文档和通知                                              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 详细操作流程

### 🚦 第一阶段：代码冻结与准备

**触发条件**: 功能开发完成，测试通过

**执行时间**: 发布日前3天

#### 步骤 1.1: 创建发布分支
```bash
# 从 main 分支创建
 git checkout main
 git pull origin main
 git checkout -b release/v1.2.1
```

#### 步骤 1.2: 更新版本号
```bash
# 更新 AssemblyInfo.cs
# 位置: src/TeenCppEdu/Properties/AssemblyInfo.cs
# 修改以下字段:
[assembly: AssemblyVersion("1.2.1.0")]
[assembly: AssemblyFileVersion("1.2.1.0")]
```

#### 步骤 1.3: 更新 CHANGELOG.md
```bash
# 在 CHANGELOG.md 顶部添加新版本章节
## [1.2.1] - YYYY-MM-DD

### 变更内容
- 修复: xxx
- 优化: xxx
```

#### 步骤 1.4: 提交发布分支
```bash
git add .
git commit -m "Release v1.2.1 preparation

- Update version to 1.2.1
- Update CHANGELOG

Co-Authored-By: Claude <noreply@anthropic.com>"
git push origin release/v1.2.1
```

---

### 🔨 第二阶段：自动化构建

**触发条件**: 发布分支已推送

**执行时间**: 发布日前2天

#### 步骤 2.1: 执行发布脚本
```bash
# 进入工具目录
cd tools

# 执行发布脚本 (PowerShell)
.\publish.ps1 -Version "v1.2.1"

# 或手动执行 (Bash)
# 进入 src 目录
cd src

# 编译 Release
dotnet build TeenCppEdu/TeenCppEdu.csproj -c Release

# 运行单元测试
dotnet test TeenCppEdu.Tests/TeenCppEdu.Tests.csproj

# 检查编译结果
if [ $? -eq 0 ]; then
    echo "✅ 编译和测试通过"
else
    echo "❌ 编译或测试失败"
    exit 1
fi
```

#### 步骤 2.2: 验证编译输出
检查 `src/TeenCppEdu/bin/Release/net48/` 目录包含：
- [ ] TeenCppEdu.exe
- [ ] TeenCppEdu.exe.config
- [ ] Newtonsoft.Json.dll
- [ ] System.Data.SQLite.dll
- [ ] x64/SQLite.Interop.dll
- [ ] x86/SQLite.Interop.dll

#### 步骤 2.3: 创建发布目录
```bash
# 创建发布目录
mkdir -p releases/v1.2.1

# 复制核心文件
cp src/TeenCppEdu/bin/Release/net48/TeenCppEdu.exe releases/v1.2.1/
cp src/TeenCppEdu/bin/Release/net48/*.dll releases/v1.2.1/
cp src/TeenCppEdu/bin/Release/net48/*.config releases/v1.2.1/
cp -r src/TeenCppEdu/bin/Release/net48/x64 releases/v1.2.1/
cp -r src/TeenCppEdu/bin/Release/net48/x86 releases/v1.2.1/

# 复制课程数据
cp -r courses releases/v1.2.1/

# 创建输出目录
mkdir -p releases/v1.2.1/output

# 复制发布说明
cp releases/v1.2.0/RELEASE_NOTE.md releases/v1.2.1/
# 或者直接创建新版本的发布说明
```

---

### ✅ 第三阶段：人工验证

**触发条件**: 自动构建成功

**执行时间**: 发布日前1天

#### 步骤 3.1: 干净环境测试
```bash
# 在干净目录测试 (可以在虚拟机或新建目录测试)
mkdir -p /tmp/teencpp-test
cp -r releases/v1.2.1/* /tmp/teencpp-test/

# 运行测试
/tmp/teencpp-test/TeenCppEdu.exe
```

#### 步骤 3.2: 执行验证清单

**RELEASE_CHECKLIST.md** 人工检查项：

| # | 检查项 | 验证方法 | 结果 |
|---|--------|----------|------|
| 1 | x64/SQLite.Interop.dll 存在 | `ls releases/v1.2.1/x64/` | ☐ |
| 2 | x86/SQLite.Interop.dll 存在 | `ls releases/v1.2.1/x86/` | ☐ |
| 3 | L01课程文件完整 | `cat releases/v1.2.1/courses/l01/lesson.json` | ☐ |
| 4 | L02课程文件完整 | `cat releases/v1.2.1/courses/l02/lesson.json` | ☐ |
| 5 | L03课程文件完整 | `cat releases/v1.2.1/courses/l03/lesson.json` | ☐ |
| 6 | 应用可启动 | 双击运行EXE无错误 | ☐ |
| 7 | 主界面正常显示 | 看到地图、等级、课程按钮 | ☐ |
| 8 | L01可进入 | 点击进入第1课 | ☐ |
| 9 | 代码编辑器显示 | 看到模板代码 | ☐ |
| 10 | 代码检查可用 | 点击检查有反馈 | ☐ |

#### 步骤 3.3: 签名确认
在 RELEASE_CHECKLIST.md 中添加签名：

```markdown
## 验证签名

| 角色 | 姓名 | 日期 | 签名 |
|------|------|------|------|
| 开发者 | [你的名字] | YYYY-MM-DD | [签名] |
| 测试者 | [测试者名] | YYYY-MM-DD | [签名] |
```

---

### 🚀 第四阶段：正式发布

**触发条件**: 人工验证全部通过

**执行时间**: 发布日

#### 步骤 4.1: 创建发布说明
```bash
# 创建详细的发布说明
cat > releases/v1.2.1/RELEASE_NOTE.md << 'EOF'
# TeenC++ v1.2.1 发布说明

**发布时间**: YYYY-MM-DD
**版本类型**: 正式版/测试版
**上一个版本**: v1.2.0

## 主要变更

### 新增功能
- xxx

### 修复问题
- xxx

### 优化改进
- xxx

## 系统要求
- Windows 10/11 (64位)
- .NET Framework 4.8

## 下载安装
1. 下载 `releases/v1.2.1/` 整个目录
2. 运行 `TeenCppEdu.exe`

## 验证清单
- [x] 编译 0 error
- [x] 单元测试通过
- [x] 干净环境测试通过
- [x] 功能回归测试通过

---
*TeenC++ 教学系统*
EOF
```

#### 步骤 4.2: 生成压缩包
```bash
# Windows (PowerShell)
Compress-Archive -Path releases/v1.2.1/* -DestinationPath releases/TeenCppEdu-v1.2.1.zip

# 或使用 7z
7z a releases/TeenCppEdu-v1.2.1.zip releases/v1.2.1/*

# Linux/Mac
cd releases && zip -r TeenCppEdu-v1.2.1.zip v1.2.1/
```

#### 步骤 4.3: 生成校验和 (可选但推荐)
```bash
# 生成 SHA256 校验和
sha256sum releases/TeenCppEdu-v1.2.1.zip > releases/TeenCppEdu-v1.2.1.zip.sha256

# Windows
# Get-FileHash releases/TeenCppEdu-v1.2.1.zip -Algorithm SHA256
```

#### 步骤 4.4: Git 标签和合并
```bash
# 确保在发布分支上
git checkout release/v1.2.1

# 创建标签
git tag -a v1.2.1 -m "Release v1.2.1

主要变更:
- xxx

验证状态:
- 编译: 通过
- 测试: 通过
- 人工验证: 通过
"

# 推送标签
git push origin v1.2.1

# 合并到 main
git checkout main
git merge release/v1.2.1 --no-ff -m "Merge release v1.2.1

Co-Authored-By: Claude <noreply@anthropic.com>"

# 推送 main
git push origin main
```

#### 步骤 4.5: 创建 GitHub Release (如果有GitHub仓库)
```bash
# 使用 GitHub CLI
gh release create v1.2.1 \
  --title "TeenC++ v1.2.1" \
  --notes-file releases/v1.2.1/RELEASE_NOTE.md \
  releases/TeenCppEdu-v1.2.1.zip
```

---

## 版本号规范

### 语义化版本控制

格式: `主版本号.次版本号.修订号` (MAJOR.MINOR.PATCH)

| 版本变化 | 规则 | 示例 |
|----------|------|------|
| **MAJOR** | 不兼容的API更改 | 1.x.x → 2.0.0 |
| **MINOR** | 向后兼容的功能增加 | 1.1.x → 1.2.0 |
| **PATCH** | 向后兼容的问题修复 | 1.2.0 → 1.2.1 |

### TeenC++ 版本策略

| 版本类型 | 用途 | 示例 |
|----------|------|------|
| **正式版** | 稳定可用，面向用户 | v1.2.0, v1.2.1 |
| **Beta版** | 功能完成，待测试 | v1.2.1-beta.1 |
| **Alpha版** | 开发中，不稳定 | v1.3.0-alpha.1 |

---

## 发布频率建议

| 版本类型 | 频率 | 说明 |
|----------|------|------|
| **PATCH** | 按需，紧急修复可立即发布 | Bug修复、小优化 |
| **MINOR** | 每2-3个月 | 新功能、新课程 |
| **MAJOR** | 每6-12个月 | 架构升级、重大变更 |

---

## 快速命令速查

### 完整发布流程（一键执行）
```bash
#!/bin/bash
# release.sh - 一键发布脚本

VERSION="1.2.1"
BRANCH="release/v${VERSION}"

echo "🚀 开始发布流程 v${VERSION}"

# 1. 创建发布分支
git checkout -b ${BRANCH}

# 2. 更新版本号 (手动编辑 AssemblyInfo.cs)
echo "⚠️ 请手动更新 AssemblyInfo.cs 中的版本号"
read -p "按回车继续..."

# 3. 提交
git add .
git commit -m "Release v${VERSION}"
git push origin ${BRANCH}

# 4. 编译
cd src
dotnet build TeenCppEdu/TeenCppEdu.csproj -c Release
dotnet test TeenCppEdu.Tests/TeenCppEdu.Tests.csproj

# 5. 创建发布目录
cd ..
mkdir -p releases/v${VERSION}
cp -r src/TeenCppEdu/bin/Release/net48/* releases/v${VERSION}/
cp -r courses releases/v${VERSION}/
mkdir -p releases/v${VERSION}/output

# 6. 创建压缩包
zip -r releases/TeenCppEdu-v${VERSION}.zip releases/v${VERSION}/

# 7. 标签和合并
git tag -a v${VERSION} -m "Release v${VERSION}"
git push origin v${VERSION}

git checkout main
git merge ${BRANCH} --no-ff
git push origin main

echo "✅ 发布完成 v${VERSION}"
```

---

## 发布完成后的工作

### 立即执行
- [ ] 通知相关人员（测试、业务、用户）
- [ ] 更新 README.md 中的版本信息
- [ ] 备份发布包

### 本周内
- [ ] 收集用户反馈
- [ ] 创建问题跟踪工单

### 下次迭代前
- [ ] 回顾本次发布流程
- [ ] 更新发布流程文档（如有改进点）
- [ ] 规划下一个版本

---

*文档版本: v1.0*  
*最后更新: 2026-04-04*  
*配套脚本: tools/publish.ps1*
