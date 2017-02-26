using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

partial class Functions
{
    public void Run()
    {
        Init();

        // TODO: 修改相关配置。
        // 检索 resw 的路径。
        Properties.ResourcePath = "Strings";
        // 检索 resw 并生成注释时使用的语言相对 ResourcePath 的路径。
        Properties.SourceLanguagePath = "en";
        // 生成辅助类的命名空间，默认使用 "<ProjectDefaultNamespace>"。
        Properties.LocalizedStringsNamespace = default(string);
        // 生成辅助类的名称，默认使用 "LocalizedStrings"。
        Properties.LocalizedStringsClassName = default(string);
        // 生成辅助类的相关接口的命名空间，默认使用 "<ProjectDefaultNamespace>.<ProjectDefaultNamespace>_ResourceInfo"。
        Properties.InterfacesNamespace = default(string);
        // 生成辅助类的修饰符
        Properties.Modifier = "public";
        // 是否为默认工程，决定是否需要显式定义资源路径。
        Properties.IsDefaultProject = true;
        // 是否调试生成的代码。
        Properties.DebugGeneratedCode = false;

        Execute();
    }
}
