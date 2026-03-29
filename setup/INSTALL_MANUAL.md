# .NET 8 SDK 手动安装指南

## 快速安装（推荐）

### 方法一：官方安装程序（最简单）

1. **下载**
   - 访问：https://dotnet.microsoft.com/download/dotnet/8.0
   - 点击 **"Download .NET SDK x64"**
   - 保存文件（约 200MB）

2. **安装**
   - 双击下载的 `.exe` 文件
   - 点击 "Install"
   - 等待进度条完成
   - 点击 "Close"

3. **验证**
   ```bash
   dotnet --version
   # 应显示 8.0.xxx
   ```

### 方法二：使用 PowerShell 脚本（自动化）

以**管理员身份**打开 PowerShell，执行：

```powershell
# 下载安装脚本
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile "$env:TEMP\dotnet-install.ps1"

# 执行安装（安装到用户目录）
& "$env:TEMP\dotnet-install.ps1" -Channel 8.0 -InstallDir "$env:LOCALAPPDATA\Microsoft\dotnet"

# 添加到 PATH
$userPath = [Environment]::GetEnvironmentVariable("Path", "User")
$newPath = "$env:LOCALAPPDATA\Microsoft\dotnet"
if (-not $userPath.Contains($newPath)) {
    [Environment]::SetEnvironmentVariable("Path", "$userPath;$newPath", "User")
}

# 验证
& "$env:LOCALAPPDATA\Microsoft\dotnet\dotnet.exe" --version
```

安装完成后，**重新打开终端**。

---

## 企业环境/离线安装

### 方法三：ZIP 包手动解压

1. **下载 ZIP**
   ```
   https://dotnetcli.azureedge.net/dotnet/Sdk/8.0.100/dotnet-sdk-8.0.100-win-x64.zip
   ```

2. **解压**
   - 解压到：`C:\Program Files\dotnet` 或 `C:\Users\[用户名]\.dotnet`

3. **添加 PATH**
   - 右键"此电脑" → 属性 → 高级系统设置
   - 环境变量 → 用户变量 → 编辑 PATH
   - 添加解压目录路径

---

## 验证安装

```bash
dotnet --version
dotnet --list-sdks
dotnet --info
```

---

## 编译 TeenC++ 项目

安装完成后：

```bash
cd TeenagerCPlusPlusEduSystem/src

dotnet restore  # 恢复 NuGet 包
dotnet build    # 编译项目
```

---

## 常见问题

### Q: 提示 "dotnet 不是内部或外部命令"
**A**: 需要添加 dotnet 到 PATH，或重新打开终端。

### Q: 安装后版本不对
**A**: 检查是否同时安装了多个版本：
```bash
dotnet --list-sdks
```

### Q: 需要管理员权限吗？
**A**: 使用用户目录安装（方法二的 `$env:LOCALAPPDATA`）不需要管理员权限。

---

## 下一步

安装完成后，回到 Claude Code 继续：
```
继续 .NET 8 迁移
```
