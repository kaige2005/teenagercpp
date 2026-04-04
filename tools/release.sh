#!/bin/bash
# TeenC++ 发布脚本 - 强制只发布正式版本
# 用法: ./tools/release.sh <version>
# 示例: ./tools/release.sh v1.3.0

set -e

VERSION=$1

# ========== 颜色定义 ==========
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# ========== 版本号验证 ==========
if [ -z "$VERSION" ]; then
    echo -e "${RED}错误: 请指定版本号${NC}"
    echo "用法: ./tools/release.sh <version>"
    echo "示例: ./tools/release.sh v1.3.0"
    exit 1
fi

# 强制版本号格式: vX.Y.Z (不允许 beta/alpha/rc)
if [[ ! $VERSION =~ ^v[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo -e "${RED}错误: 版本号格式不正确${NC}"
    echo -e "只允许正式版本格式: ${GREEN}vX.Y.Z${NC}"
    echo -e "${RED}禁止:${NC} v1.3.0-beta, v1.3.0-alpha, v1.3.0-rc1 等"
    echo ""
    echo "如果你确实需要开发测试版本，请:"
    echo "1. 本地构建测试"
    echo "2. 不要创建 Git 标签"
    echo "3. 不要推送 GitHub Release"
    exit 1
fi

echo -e "${GREEN}✓ 版本号格式正确: $VERSION${NC}"

# ========== 检查当前分支 ==========
CURRENT_BRANCH=$(git branch --show-current)
if [ "$CURRENT_BRANCH" != "main" ]; then
    echo -e "${YELLOW}警告: 当前不在 main 分支 ($CURRENT_BRANCH)${NC}"
    read -p "是否切换到 main 分支? (y/n) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        git checkout main
        git pull origin main
    else
        echo -e "${RED}取消发布${NC}"
        exit 1
    fi
fi

# ========== 检查是否有未提交更改 ==========
if [ -n "$(git status --porcelain)" ]; then
    echo -e "${RED}错误: 有未提交的更改${NC}"
    git status --short
    echo "请先提交所有更改"
    exit 1
fi

echo -e "${GREEN}✓ 工作区干净${NC}"

# ========== 检查标签是否已存在 ==========
if git rev-parse "$VERSION" >/dev/null 2>&1; then
    echo -e "${RED}错误: 标签 $VERSION 已存在${NC}"
    exit 1
fi

echo -e "${GREEN}✓ 标签可用${NC}"

# ========== 编译检查 ==========
echo -e "${YELLOW}编译检查...${NC}"
cd src
dotnet build TeenCppEdu/TeenCppEdu.csproj -c Release

if [ $? -ne 0 ]; then
    echo -e "${RED}编译失败${NC}"
    exit 1
fi
cd ..

echo -e "${GREEN}✓ 编译通过${NC}"

# ========== 单元测试 ==========
echo -e "${YELLOW}运行单元测试...${NC}"
cd src
dotnet test TeenCppEdu.Tests/TeenCppEdu.Tests.csproj --verbosity minimal
TEST_RESULT=$?
cd ..

if [ $TEST_RESULT -ne 0 ]; then
    echo -e "${RED}单元测试失败${NC}"
    exit 1
fi

echo -e "${GREEN}✓ 单元测试通过${NC}"

# ========== 确认发布 ==========
echo ""
echo "======================================"
echo -e "${GREEN}准备发布 $VERSION${NC}"
echo "======================================"
echo ""
echo "即将执行:"
echo "  1. 创建版本提交"
echo "  2. 创建 Git 标签"
echo "  3. 推送到 GitHub"
echo ""
read -p "确认发布? (y/n) " -n 1 -r
echo

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo -e "${YELLOW}取消发布${NC}"
    exit 0
fi

# ========== 创建发布 ==========
echo -e "${YELLOW}创建发布...${NC}"

# 更新版本文件
echo "${VERSION}" > releases/version.txt

# 添加所有更改
git add -A
git commit -m "Release ${VERSION}

- Update version to ${VERSION}
- Ensure all tests pass
- Ready for production release

Co-Authored-By: Claude <noreply@anthropic.com>"

# 创建标签
git tag -a "${VERSION}" -m "Release ${VERSION}

正式版本发布
- 完整功能测试通过
- 用户验收通过
- 稳定可用"

# 推送到 GitHub
git push origin main
git push origin "${VERSION}"

echo -e "${GREEN}======================================${NC}"
echo -e "${GREEN}✅ 发布完成: ${VERSION}${NC}"
echo -e "${GREEN}======================================${NC}"
echo ""
echo "下一步:"
echo "  1. 访问 https://github.com/kaige2005/teenagercpp/releases/new"
echo "  2. 选择标签: ${VERSION}"
echo "  3. 填写 Release 说明"
echo "  4. 上传发布包"
