namespace ResourceGenerator
{
    partial class Functions
    {
        public void Run()
        {
            Init();

            // TODO: 修改相关配置。

            // 检索 resw 的路径，默认为 "Strings"。
            Properties.ResourcePath = "Strings";

            // 检索 resw 并生成注释时使用的语言相对 ResourcePath 的路径。
            // Properties.SourceLanguagePath = "en-Us";

            // 生成辅助类的命名空间，默认使用 "<ProjectDefaultNamespace>"。
            // Properties.LocalizedStringsNamespace = "MyNamespace";

            // 生成辅助类的名称，默认使用 "LocalizedStrings"。
            Properties.LocalizedStringsClassName = "LocalizedStrings";

            // 生成辅助类的相关接口的命名空间，默认使用 "<ProjectDefaultNamespace>.<ProjectDefaultNamespace>_ResourceInfo"。
            // Properties.InterfacesNamespace = "MyNamespace.MyNamespace_ResourceInfo";

            // 生成辅助类的修饰符
            Properties.Modifier = "internal";

            // 是否为默认工程，决定是否需要显式定义资源路径。
            Properties.IsDefaultProject = true;

            // 是否调试生成的代码。
            Properties.DebugGeneratedCode = false;

            // 生成 Cache 存储的方法，默认为 "new global::System.Collections.Generic.Dictionary<string, string>()"，必须实现 "IDictionary<string, string>"
            // Properties.CacheActivator = "new MyNamespace.MyCacheDictionaty()";

            Execute();
        }
    }
}
