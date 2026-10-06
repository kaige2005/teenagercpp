# Release Skill - 标准化发布流程

自动化TeenC++项目的发布流程，确保每次发布都经过完整的验证和标准化的步骤。

## 用法

```
/release <version> [--beta] [--skip-tests]
```

## 参数

- `version`: 版本号 (如 v1.2.0, v1.2.0-beta.8)
- `--beta`: 标记为测试版本
- `--skip-tests`: 跳过测试执行 (不推荐)

## 发布流程

### Phase 1: 预检查 (无法跳过)
1. 验证GitHub CLI可用且已认证
2. 验证当前分支状态
3. 检查工作目录干净
4. 验证版本号格式正确

### Phase 2: 构建验证
5. 清理bin/obj目录
6. 执行完整编译
7. 验证关键DLL存在 (基准目录 `src/TeenCppEdu/bin/Release/net48/`):
   - SQLite.Interop.dll (x86/x64)
   - System.Data.SQLite.dll
   - Newtonsoft.Json.dll
8. 验证主程序exe生成 (TeenCppEdu.exe)

### Phase 3: 功能验证 (可--skip-tests跳过)
9. 执行单元测试
10. 验证L01旧格式兼容性
11. 验证L03新格式功能
12. 检查代码功能空引用测试

### Phase 4: 版本更新
13. 更新AssemblyInfo.cs版本号
14. 更新VERSION文件
15. 更新CHANGELOG.md
16. 提交版本更新commit

### Phase 5: 打包发布
17. 清理发布目录
18. 复制必要文件到releases/目录
19. 创建zip包
20. 计算SHA256校验和

### Phase 6: GitHub Release
21. 创建GitHub Release
22. 上传发布包和校验和
23. 生成发布说明
24. 标记为pre-release (如果是beta)

## 回滚机制

如果任何阶段失败:
- Phase 1-2: 中止发布，报告错误
- Phase 3-4: 可选择回滚版本提交
- Phase 5-6: 可选择删除已创建的tag/release

## 发布检查清单

Release Skill会自动验证以下内容:

### DLL依赖检查 (基准目录 `src/TeenCppEdu/bin/Release/net48/`)
- [ ] x86/SQLite.Interop.dll 存在
- [ ] x64/SQLite.Interop.dll 存在
- [ ] System.Data.SQLite.dll 存在
- [ ] Newtonsoft.Json.dll 存在
- [ ] courses/ 资源目录存在，且所有 `lesson.json` 均为合法 JSON
      (L03 曾因字符串内未转义 `"` 导致整课无法解析)
- [ ] L01/L02/L03 XP 合计一致 (设计 190；当前 L01/L02 实为 200，见记忆 teencpp-xp-discrepancy)

### 功能检查
- [ ] L01课程代码编辑器正常
- [ ] 检查代码按钮不崩溃
- [ ] L03三阶段UI正常

### 文件检查
- [ ] 配置文件完整
- [ ] 数据库文件存在
- [ ] 示例代码完整

## 示例

```bash
# 创建正式版本
/release v1.2.0

# 创建测试版本
/release v1.2.0-beta.8 --beta

# 紧急修复 (跳过测试)
/release v1.2.1 --skip-tests
```

## 故障处理

### GitHub CLI 认证失败
如果`gh auth status`失败，Skill将:
1. 提示用户运行 `gh auth login`
2. 或改为手动 Web 上传到 https://github.com/kaige2005/teenagercpp/releases

### 编译失败
如果编译失败，Skill将:
1. 显示编译错误摘要
2. 建议修复方案
3. 中止发布流程

### 测试失败
如果测试失败，Skill将:
1. 显示失败的测试
2. 询问是否继续发布
3. 记录警告到发布说明

## GitHub 通路说明

**不再有 MCP 回退。** 历史文档建议的 `npx @anthropics/mcp-github-server` 在 npm 上**不存在（404）**，
且 `.claude/mcp/github-server.json` 这个位置 Claude Code **根本不读**（项目 MCP 必须是项目根的 `.mcp.json`），
`GITHUB_TOKEN` 也从未设置 —— 该方案从未生效，已于 2026-09-25 删除。

仓库: **`kaige2005/teenagercpp`**
（不是历史文档里的 `Carl/TeenagerCPlusPlusEduSystem` —— 该仓库不存在，会 404）

若确实需要 MCP：先建项目根的 `.mcp.json`，并**先用 `npm view <包名>` 确认包名可用**再写入。
