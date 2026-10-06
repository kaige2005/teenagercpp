# TeenC++ 版本管理规范

> 版本命名、管理策略与发布流程的完整规范

---

## 一、版本号命名规范

### 1.1 正式版本 (Release)

**格式**: `v主版本号.次版本号.修订号`

| 版本类型 | 格式 | 示例 | 说明 |
|----------|------|------|------|
| **主版本** (Major) | vX.0.0 | v2.0.0 | 重大架构变更，可能不兼容 |
| **次版本** (Minor) | vX.Y.0 | v1.3.0 | 新增功能，向后兼容 |
| **修订版本** (Patch) | vX.Y.Z | v1.2.1 | Bug修复，向后兼容 |

**规则**:
```
v1.2.0  ✓ 正确 (次版本发布)
v1.2.1  ✓ 正确 (补丁版本)
v2.0.0  ✓ 正确 (主版本发布)
1.2.0   ✗ 错误 (缺少 v 前缀)
V1.2.0  ✗ 错误 (大写 V)
v01.02.03 ✗ 错误 (多余前导零)
```

### 1.2 开发版本 (本地测试，禁止推送到 GitHub)

**格式禁止**:
- ❌ `v1.2.0-beta.1` (禁止)
- ❌ `v1.2.0-alpha.1` (禁止)
- ❌ `v1.2.0-rc.1` (禁止)
- ❌ `v1.2.0-preview` (禁止)

**替代方案**:
- ✅ 本地构建测试，不创建 Tag
- ✅ 使用分支: `dev/v1.2.x`
- ✅ 使用 commit: 直接本地验证

---

## 二、版本管理策略

### 2.1 版本生命周期

```
开发中 → 测试通过 → 正式发布 → 维护期 → 废弃
   │         │          │          │        │
  dev      local       GitHub      patches  EOL
 branch     test       Release
```

### 2.2 版本保留策略

| 版本类型 | 保留数量 | 保留时间 | 操作 |
|----------|----------|----------|------|
| **正式版** | 全部 | 永久 | 保留 |
| **最新正式版** | 1个 | 永久 | 设为 Latest |
| **补丁版** | 最新2个 | 维护期内 | 保留 |
| **Beta/测试** | 0个 | 不保留 | ❌ 发布后立即删除 |

### 2.3 清理历史 Beta 版本

**已清理**:
- ✅ v1.1.0-beta.2 ~ v1.1.0-beta.11 (10个)
- ✅ v1.2.0-beta.1 ~ v1.2.0-beta.7 (7个)

**清理命令**:
```bash
# 本地 releases/ 目录
rm -rf releases/v1.1.0-beta.*
rm -rf releases/v1.2.0-beta.*

# GitHub Release (手动或 CLI)
gh release delete <beta-version> --repo kaige2005/teenagercpp --yes

# Git Tag (如果误推送了)
git push origin --delete v1.2.0-beta.x
git tag -d v1.2.0-beta.x
```

---

## 三、发布流程 (标准化)

### 3.1 发布前准备清单

#### 1. 代码准备
```bash
# 切换到 main 分支
git checkout main
git pull origin main

# 确保工作区干净
git status  # 应该是干净的

# 运行测试
./tools/release.sh v1.X.Y  # 会自动测试
```

#### 2. 版本号确认
- [ ] 版本号格式: `vX.Y.Z` (无beta/alpha/rc)
- [ ] CHANGELOG.md 已更新
- [ ] README.md 版本信息已更新

#### 3. 质量检查
- [ ] 编译: 0 errors
- [ ] 单元测试: 100% 通过
- [ ] 用户测试: 完成并通过
- [ ] 文件完整性: 检查清单通过

### 3.2 发布步骤

```bash
# 使用发布脚本 (推荐)
./tools/release.sh v1.3.0

# 或手动步骤
# 1. 编译测试
cd src
dotnet build TeenCppEdu/TeenCppEdu.csproj -c Release
dotnet test TeenCppEdu.Tests/TeenCppEdu.Tests.csproj

# 2. 创建发布包
# ... (详见 RELEASE_PROCESS.md)

# 3. 创建 Git 标签
git tag -a v1.3.0 -m "Release v1.3.0"
git push origin v1.3.0

# 4. 创建 GitHub Release
gh release create v1.3.0 \
  --title "TeenC++ v1.3.0 - xxxx" \
  --notes-file releases/v1.3.0/RELEASE_NOTE.md \
  releases/TeenCppEdu-v1.3.0.tar.gz
```

### 3.3 发布后验证

| 检查项 | 方法 | 预期 |
|--------|------|------|
| GitHub Release 页面 | 访问链接 | 显示新版本 |
| 文件可下载 | 点击下载 | 正常下载 |
| 版本号正确 | 查看页面 | 显示 vX.Y.Z |
| 标记为 Latest | 查看标签 | 绿色 "Latest" |

---

## 四、自动化工具

### 4.1 release.sh 脚本 (强制检查)

位置: `tools/release.sh`

**功能**:
- ✅ 强制版本号格式 `vX.Y.Z` (无beta)
- ✅ 自动编译检查
- ✅ 自动单元测试
- ✅ 强制干净工作区
- ✅ 只在 main 分支发布

**使用**:
```bash
# 正确 - 执行发布
./tools/release.sh v1.3.0

# 错误 - 被拒绝
./tools/release.sh v1.3.0-beta.1
# 输出: 错误: 禁止使用 beta/alpha/rc 版本
```

### 4.2 版本号验证正则

```regex
^v[0-9]+\.[0-9]+\.[0-9]+$

解释:
^v          - 以 "v" 开头
[0-9]+      - 主版本号 (数字)
\.          - 点号
[0-9]+      - 次版本号 (数字)
\.          - 点号
[0-9]+      - 修订号 (数字)
$           - 结束
```

---

## 五、版本演进路线图

### 5.1 版本规划

```
2026 Q1      2026 Q2      2026 Q3      2026 Q4      2027 Q1
  │            │            │            │            │
v1.2.0       v1.3.0       v1.4.0       v2.0.0       v2.1.0
 (当前)        ↓            ↓            ↓            ↓
  │        L4-L6课程    L7-L10课程   竞赛版本      AI辅导
  │          发布          发布         发布         测试
  │
  └── 已发布 ──┘
```

### 5.2 版本时间表

| 版本 | 计划时间 | 主要内容 | 状态 |
|------|----------|----------|------|
| v1.2.0 | 2026-04-04 | L03 + 三阶段结构 | ✅ 已发布 |
| v1.2.1 | 按需 | Bug修复 | ⏳ 待定 |
| v1.3.0 | 2026-04 | L04-L06课程 | 🔵 规划中 |
| v1.4.0 | 2026-Q3 | L07-L10课程 | 🟡 规划中 |
| v2.0.0 | 2026-Q4 | 竞赛版+在线评测 | 🟡 规划中 |

---

## 六、文档关联

| 文档 | 用途 | 与本规范关系 |
|------|------|-------------|
| `VERSION_MANAGEMENT.md` | 本文件 | 主规范文档 |
| `RELEASE_PROCESS.md` | 详细发布步骤 | 执行指引 |
| `RELEASE_POLICY.md` | 发布政策 | 约束说明 |
| `ROADMAP.md` | 产品路线图 | 版本规划来源 |
| `tools/release.sh` | 发布脚本 | 自动化执行 |

---

## 七、违规处理

### 7.1 常见违规

**误发布 beta 版本**:
```bash
# 发现误发了 v1.3.0-beta.1

# 1. 删除 GitHub Release
gh release delete v1.3.0-beta.1 --repo kaige2005/teenagercpp --yes

# 2. 删除 Git 标签
git push origin --delete v1.3.0-beta.1
git tag -d v1.3.0-beta.1

# 3. 如果已通知用户，发布说明澄清
```

### 7.2 预防措施

1. **使用 release.sh 脚本**: 强制检查版本号
2. **分支保护**: 只有 main 分支可以打 release 标签
3. **代码审查**: 发布前必须有人确认无误
4. **发布检查清单**: 严格按照 RELEASE_CHECKLIST 执行

---

## 八、快速参考

### 版本命名速查表

```
✅ 允许发布:
v1.0.0
v1.1.0
v1.2.0
v1.2.1
v2.0.0

❌ 禁止发布:
v1.2.0-beta.1
v1.2.0-alpha.2
v1.2.0-rc.3
v1.2.0-SNAPSHOT
v1.2.0-preview
```

### 发布命令速查

```bash
# 完整发布
./tools/release.sh v1.3.0

# 仅创建标签 (不推荐手动)
git tag -a v1.3.0 -m "Release v1.3.0"
git push origin v1.3.0

# 仅创建 GitHub Release
gh release create v1.3.0 \
  --title "TeenC++ v1.3.0" \
  --notes "Release notes" \
  TeenCppEdu-v1.3.0.tar.gz

# 删除误发的 beta
gh release delete v1.3.0-beta.1 --yes
git push origin --delete v1.3.0-beta.1
```

---

## 九、版本管理检查清单

### 每次发布前必须检查

- [ ] 版本号为正式版格式 `vX.Y.Z` (无beta/alpha/rc)
- [ ] 在 main 分支
- [ ] 工作区干净 (无未提交更改)
- [ ] 编译 0 error
- [ ] 单元测试 100% 通过
- [ ] CHANGELOG.md 已更新
- [ ] GitHub Release 描述已准备
- [ ] 发布包已创建并测试

---

*规范版本: v1.0*  
*生效日期: 2026-04-04*  
*关联文档: RELEASE_PROCESS.md, RELEASE_POLICY.md*
