#!/bin/bash
# .NET 8 SDK 安装脚本 (适用于MSYS/Git Bash)

set -e

INSTALL_DIR="$LOCALAPPDATA/Microsoft/dotnet"
INSTALL_SCRIPT_URL="https://dot.net/v1/dotnet-install.sh"

echo "======================================"
echo ".NET 8 SDK 安装脚本"
echo "======================================"
echo ""

# 检查是否已安装
if command -v dotnet &> /dev/null; then
    echo ".NET SDK 已安装:"
    dotnet --version
    echo ""
    read -p "是否重新安装? (y/N): " -n 1 -r
    echo ""
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "跳过安装"
        exit 0
    fi
fi

# 创建安装目录
echo "创建安装目录: $INSTALL_DIR"
mkdir -p "$INSTALL_DIR"

# 下载安装脚本
echo ""
echo "正在下载 .NET 安装脚本..."
echo "来源: $INSTALL_SCRIPT_URL"
SCRIPT_PATH="/tmp/dotnet-install.sh"

if command -v curl &> /dev/null; then
    curl -fsSL "$INSTALL_SCRIPT_URL" -o "$SCRIPT_PATH"
elif command -v wget &> /dev/null; then
    wget -q "$INSTALL_SCRIPT_URL" -O "$SCRIPT_PATH"
else
    echo "错误: 需要 curl 或 wget"
    exit 1
fi

echo "安装脚本下载成功!"

# 设置执行权限
chmod +x "$SCRIPT_PATH"

# 执行安装
echo ""
echo "======================================"
echo "正在安装 .NET 8 SDK..."
echo "安装路径: $INSTALL_DIR"
echo "这可能需要几分钟，请耐心等待..."
echo "======================================"
echo ""

"$SCRIPT_PATH" --channel 8.0 --install-dir "$INSTALL_DIR" --no-path

echo ""
echo "======================================"
echo "安装完成!"
echo "======================================"
echo ""

# 添加到 PATH
USER_PROFILE="$USERPROFILE/.bash_profile"
if [[ -f "$USER_PROFILE" ]]; then
    if ! grep -q "$INSTALL_DIR" "$USER_PROFILE" 2>/dev/null; then
        echo "" >> "$USER_PROFILE"
        echo "# .NET SDK" >> "$USER_PROFILE"
        echo "export PATH=\"\$PATH:$INSTALL_DIR\"" >> "$USER_PROFILE"
        echo "已添加 $INSTALL_DIR 到 PATH ($USER_PROFILE)"
    fi
fi

# 当前会话添加
echo ""
echo "可用命令:"
echo "  查看版本: $INSTALL_DIR/dotnet --version"
echo "  列出SDK:  $INSTALL_DIR/dotnet --list-sdks"
echo "  编译项目: $INSTALL_DIR/dotnet build"
echo ""
echo "注意: 请重新打开终端或使用以下命令使PATH生效:"
echo "  export PATH=\"\$PATH:$INSTALL_DIR\""
echo ""

# 验证
if "$INSTALL_DIR/dotnet" --version; then
    echo ""
    echo ".NET 8 SDK 已成功安装并可用!"
else
    echo ""
    echo "安装可能已完成，但验证失败"
    echo "请尝试手动运行: $INSTALL_DIR/dotnet --version"
fi
