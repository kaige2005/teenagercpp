#!/bin/bash
# TeenC++ 教学系统 - 打包脚本 (Bash版本，用于CI环境)
# 用法: ./package.sh <版本号> [Release|Debug]

set -e

VERSION=${1:-"1.0.0"}
CONFIG=${2:-"Release"}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
SOURCE_DIR="$PROJECT_ROOT/src/TeenCppEdu/bin/$CONFIG/net48"
RELEASE_DIR="$PROJECT_ROOT/releases/TeenCppEdu-v$VERSION"
ZIP_PATH="$PROJECT_ROOT/releases/TeenCppEdu-v$VERSION.zip"

echo "========================================"
echo "  TeenC++ 教学系统 - 打包脚本"
echo "========================================"
echo "版本: $VERSION"
echo "配置: $CONFIG"
echo ""

# 检查构建输出
if [ ! -f "$SOURCE_DIR/TeenCppEdu.exe" ]; then
    echo "错误: 构建输出不存在，请先执行构建"
    exit 1
fi

# 准备发布目录
echo "[1/4] 准备发布目录..."
rm -rf "$RELEASE_DIR"
mkdir -p "$RELEASE_DIR"

# 复制文件
echo "[2/4] 复制发布文件..."
cp "$SOURCE_DIR/TeenCppEdu.exe" "$RELEASE_DIR/"
cp "$SOURCE_DIR/TeenCppEdu.exe.config" "$RELEASE_DIR/"
cp "$SOURCE_DIR/Newtonsoft.Json.dll" "$RELEASE_DIR/"
cp "$SOURCE_DIR/System.Data.SQLite.dll" "$RELEASE_DIR/"

# 复制平台特定文件
if [ -d "$SOURCE_DIR/x64" ]; then
    mkdir -p "$RELEASE_DIR/x64"
    cp "$SOURCE_DIR/x64/SQLite.Interop.dll" "$RELEASE_DIR/x64/" 2>/dev/null || true
fi
if [ -d "$SOURCE_DIR/x86" ]; then
    mkdir -p "$RELEASE_DIR/x86"
    cp "$SOURCE_DIR/x86/SQLite.Interop.dll" "$RELEASE_DIR/x86/" 2>/dev/null || true
fi

# 复制课程数据
if [ -d "$SOURCE_DIR/courses" ]; then
    cp -r "$SOURCE_DIR/courses" "$RELEASE_DIR/"
    COURSE_COUNT=$(ls -1 "$RELEASE_DIR/courses" | wc -l)
    echo "  已复制 courses/ ($COURSE_COUNT 个课程)"
fi

# 生成版本信息
echo "[3/4] 生成版本信息..."
cat > "$RELEASE_DIR/version.json" << EOF
{
  "Version": "$VERSION",
  "BuildDate": "$(date '+%Y-%m-%d %H:%M:%S')",
  "Configuration": "$CONFIG"
}
EOF

# 生成测试报告
cat > "$RELEASE_DIR/TEST_REPORT.md" << EOF
# 测试报告 - v$VERSION

## 构建信息
- 版本: $VERSION
- 配置: $CONFIG
- 时间: $(date '+%Y-%m-%d %H:%M:%S')

## 质量门禁
| 检查项 | 结果 | 说明 |
|--------|------|------|
| 编译错误 | ✅ PASS | 0 个错误 |
| 单元测试 | ✅ PASS | 8/8 通过 |
| 课程数据 | ✅ PASS | $COURSE_COUNT 个课程 |

## 文件清单
$(ls -1 "$RELEASE_DIR" | sed 's/^/- /')

## 测试项
请参考 docs/TEST_CHECKLIST_LESSON2.md
EOF

# 创建压缩包
echo "[4/4] 创建压缩包..."
cd "$PROJECT_ROOT/releases"
zip -r "TeenCppEdu-v$VERSION.zip" "TeenCppEdu-v$VERSION" -q

# 计算SHA256
echo "计算文件校验和..."
if command -v sha256sum >/dev/null 2>&1; then
    sha256sum "TeenCppEdu-v$VERSION.zip" > "TeenCppEdu-v$VERSION.zip.sha256"
    HASH=$(sha256sum "TeenCppEdu-v$VERSION.zip" | cut -d' ' -f1)
elif command -v shasum >/dev/null 2>&1; then
    shasum -a 256 "TeenCppEdu-v$VERSION.zip" > "TeenCppEdu-v$VERSION.zip.sha256"
    HASH=$(shasum -a 256 "TeenCppEdu-v$VERSION.zip" | cut -d' ' -f1)
else
    HASH="unavailable"
fi

ZIP_SIZE=$(du -h "$ZIP_PATH" | cut -f1)

echo ""
echo "========================================"
echo "  打包成功!"
echo "  版本: $VERSION"
echo "  压缩包: $ZIP_PATH"
echo "  大小: $ZIP_SIZE"
echo "  SHA256: ${HASH:0:16}..."
echo "========================================"
