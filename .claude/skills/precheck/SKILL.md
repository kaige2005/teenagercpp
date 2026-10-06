# PreCheck Skill - 预提交检查

在提交代码或标记任务完成前运行必要的验证检查。

## 用法

```
/precheck [level]
```

- `level`: 检查级别 (quick|standard|release)
  - `quick`: 快速检查，仅验证文件存在
  - `standard`: 标准检查，包含编译验证 (默认)
  - `release`: 发布检查，完整验证所有依赖和测试

## 检查项目

### 所有级别
1. 项目文件存在性检查
2. 关键DLL存在性检查 (基准目录 `src/TeenCppEdu/bin/Release/net48/`):
   - System.Data.SQLite.dll
   - Newtonsoft.Json.dll
   - TeenCppEdu.exe
3. SQLite.Interop.dll 平台检查 (x86/x64)
3b. courses/ 课程资源目录检查

> ⚠️ `WeifenLuo.WinFormsUI.Docking.dll` **已不再使用**（全库 0 处引用，csproj 仅有
> Newtonsoft.Json + System.Data.SQLite.Core）。不要再把它列为检查项。

### standard 级别额外检查
4. 编译验证 (如果msbuild可用)
5. 输出目录结构检查

### release 级别额外检查
6. 运行单元测试 (如果存在)
7. 检查是否有未提交的更改
8. 验证版本号一致性

## 退出代码

- 0: 所有检查通过
- 1: 发现错误，阻止提交/发布

## 示例

```
/precheck           # 运行标准检查
/precheck quick     # 快速检查
/precheck release   # 发布前完整检查
```
