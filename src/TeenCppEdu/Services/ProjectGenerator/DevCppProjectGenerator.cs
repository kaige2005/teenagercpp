using System;
using System.IO;
using System.Text;

namespace TeenCppEdu.Services.ProjectGenerator
{
    /// <summary>
    /// Dev-C++ 项目生成器
    /// </summary>
    public class DevCppProjectGenerator
    {
        /// <summary>
        /// 生成完整的 Dev-C++ 项目
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="outputPath">输出目录</param>
        /// <param name="sourceCode">源代码内容（可为空，使用模板）</param>
        /// <returns>生成的项目路径</returns>
        public string GenerateProject(string projectName, string outputPath, string sourceCode = null)
        {
            // 创建项目目录
            string projectDir = Path.Combine(outputPath, projectName);
            Directory.CreateDirectory(projectDir);

            // 使用 GB2312 (ANSI) 编码以兼容 Dev-C++ 5.11
            var ansiEncoding = Encoding.GetEncoding("gb2312");

            // 关键修复：使用StreamWriter明确控制编码，确保输出纯ANSI无BOM
            string devFilePath = Path.Combine(projectDir, $"{projectName}.dev");
            using (var writer = new StreamWriter(devFilePath, false, ansiEncoding))
            {
                writer.Write(GenerateDevFile(projectName));
            }

            // 生成 .cpp 源文件
            string cppCode = sourceCode ?? GenerateDefaultCppCode(projectName);
            string cppFilePath = Path.Combine(projectDir, "main.cpp");
            using (var writer = new StreamWriter(cppFilePath, false, ansiEncoding))
            {
                writer.Write(cppCode);
            }

            // 生成 private 子目录和 resources.rc
            string privateDir = Path.Combine(projectDir, "private");
            Directory.CreateDirectory(privateDir);
            string rcFilePath = Path.Combine(privateDir, "resources.rc");
            using (var writer = new StreamWriter(rcFilePath, false, ansiEncoding))
            {
                writer.Write(GenerateRcFile());
            }

            // 生成 Makefile.win
            string makefilePath = Path.Combine(projectDir, "Makefile.win");
            using (var writer = new StreamWriter(makefilePath, false, ansiEncoding))
            {
                writer.Write(GenerateMakefile(projectName));
            }

            return projectDir;
        }

        /// <summary>
        /// 使用模板生成项目（从课程数据）
        /// </summary>
        public string GenerateFromTemplate(string projectName, string outputPath, string templatePath)
        {
            // 关键修复：使用无BOM的UTF-8读取模板，确保正确转换到ANSI
            string templateCode;
            using (var reader = new StreamReader(templatePath, new UTF8Encoding(false)))
            {
                templateCode = reader.ReadToEnd();
            }
            return GenerateProject(projectName, outputPath, templateCode);
        }

        private string GenerateDevFile(string projectName)
        {
            return $@"
[Project]
FileName={projectName}.dev
Name={projectName}
Type=1
Ver=2
ObjFiles=
Includes=
Libs=
PrivateResource=private\\resources.rc
ResourceIncludes=
MakeIncludes=
Compiler=
CppCompiler=
Linker=
IsCpp=1
Icon=
ExeOutput=
ObjectOutput=
LogOutput=
LogOutputEnabled=0
OverrideOutput=0
OverrideOutputName={projectName}.exe
HostApplication=
UseCustomMakefile=0
CustomMakefile=
CommandLine=
Folders=
IncludeVersionInfo=0
SupportXPThemes=0
CompilerSet=0
CompilerSettings=0000000000000000001000000
UnitCount=1

[VersionInfo]
Major=1
Minor=0
Release=0
Build=0
LanguageID=1033
CharsetID=1252
CompanyName=
FileVersion=1.0.0.0
FileDescription=Developed using the Dev-C++ IDE
InternalName=
LegalCopyright=
LegalTrademarks=
OriginalFilename=
ProductName={projectName}
ProductVersion=1.0.0.0
AutoIncBuildNr=0
SyncProduct=1

[Unit1]
FileName=main.cpp
CompileCpp=1
Folder={projectName}
Compile=1
Link=1
Priority=1000
OverrideBuildCmd=0
BuildCmd=
";
        }

        private string GenerateDefaultCppCode(string projectName)
        {
            return $@"
#include <iostream>

// {projectName} - C++ 练习项目
// 开始编写你的代码吧！

int main()
{{
    std::cout << ""Hello, C++!"" << std::endl;

    return 0;
}}
";
        }

        private string GenerateRcFile()
        {
            return @"
/*
 *  Resources for Dev-C++ Project
 */

#include <windows.h>

/*  应用程序图标可以在这里定义  */
";
        }

        private string GenerateMakefile(string projectName)
        {
            // 这是一个简化版的 Makefile，实际 Dev-C++ 生成的会更复杂
            // 但对于教学用途，Dev-C++ 会重新生成它
            return $@"
# Project: {projectName}
# Makefile created by TeenCppEdu

CPP      = g++.exe
CC       = gcc.exe
OBJ      = main.o
LINKOBJ  = main.o
BIN      = {projectName}.exe
CXXFLAGS = -g3 -Wall -std=c++11
RM       = rm.exe -f

.PHONY: clean all

all: $(BIN)

clean:
	$(RM) $(OBJ) $(BIN)

$(BIN): $(OBJ)
	$(CPP) $(LINKOBJ) -o $(BIN)

main.o: main.cpp
	$(CPP) -c main.cpp -o main.o $(CXXFLAGS)
";
        }
    }
}
