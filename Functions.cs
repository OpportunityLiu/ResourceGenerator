using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Linq;
using System.Web;

partial class Functions
{
    private void Init()
    {
        Properties.ProjectAssemblyName = GetProjectAssemblyName();
        Properties.ProjectDefaultNamespace = GetProjectDefaultNamespace();
        Properties.ProjectPath = GetProjectPath();
    }

    public void Execute()
    {
        var names = new Dictionary<string, Dictionary<string, object>>();
        var stringsPath = Path.Combine(Properties.ProjectPath, Properties.ResourcePath, Properties.SourceLanguagePath);
        string[] reswPaths;
        string[] resJsonPaths;

        if(Directory.Exists(stringsPath))
        {
            reswPaths = Directory.GetFiles(stringsPath, "*.resw", SearchOption.AllDirectories);
            resJsonPaths = Directory.GetFiles(stringsPath, "*.resJson", SearchOption.AllDirectories);
        }
        else
        {
            reswPaths = new string[0];
            resJsonPaths = new string[0];
        }
        foreach(var reswPath in reswPaths)
        {
            AnalyzeResw(names, reswPath);
        }
        foreach(var resJsonPath in resJsonPaths)
        {
            AnalyzeResJson(names, resJsonPath);
        }
        WriteInterfaces(names);
        WriteLine("");
        WriteResources(names);
    }

    public PropertiesClass Properties = new PropertiesClass();

    public class PropertiesClass
    {
        private void setIdentity(ref string field, string value, string def, bool allowDots)
        {
            if(!string.IsNullOrWhiteSpace(value))
                field = Helper.Refine(value.Trim(), allowDots);
            else
                field = def;
        }

        public string ProjectPath { get; set; }
        public string ProjectDefaultNamespace { get; set; }
        public string ProjectAssemblyName { get; set; }

        public bool DebugGeneratedCode { get; set; }
        public string ResourcePath { get; set; } = "Strings";
        public string SourceLanguagePath { get; set; } = "en";
        public bool IsDefaultProject { get; set; } = true;

        private string rns, rc, ins, mf = "public";
        public string LocalizedStringsNamespace
        {
            get { return rns; }
            set { setIdentity(ref rns, value, ProjectDefaultNamespace, true); }
        }
        public string LocalizedStringsClassName
        {
            get { return rc; }
            set { setIdentity(ref rc, value, "LocalizedStrings", false); }
        }
        public string InterfacesNamespace
        {
            get { return ins; }
            set { setIdentity(ref ins, value, $"{ProjectDefaultNamespace}.{ProjectDefaultNamespace}_ResourceInfo", true); }
        }
        public string Modifier
        {
            get { return mf; }
            set
            {
                value = (value ?? "").Trim();
                mf = string.IsNullOrEmpty(value) ? "" : value + " ";
            }
        }

        public string LocalizedStringsFullName
            => $"global::{LocalizedStringsNamespace}.{LocalizedStringsClassName}";
        public string InterfaceFullName(string interfaceName, string ins)
        {
            if(string.IsNullOrEmpty(ins))
                return $"global::{InterfacesNamespace}.{interfaceName}";
            else if(ins.StartsWith("global::"))
                return $"{ins}.{interfaceName}";
            else
                return $"global::{ins}.{interfaceName}";
        }
        public string InterfaceFullName(string propertyName)
        {
            if(!propertyName.StartsWith(LocalizedStringsFullName + "."))
                throw new Exception();
            propertyName = propertyName.Substring(LocalizedStringsFullName.Length + 1);
            var names = propertyName.Split('.');
            names[names.Length - 1] = "I" + names[names.Length - 1];
            return $"global::{InterfacesNamespace}.{string.Join(".", names)}";
        }
    }

    public void WriteAttributsForInterface(string indent)
    {
        WriteLine($@"{indent}[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{ProductName}"", ""{ProductVersion}"")]");
    }

    public void WriteAttributsForClass(string indent)
    {
        if(!Properties.DebugGeneratedCode)
            WriteLine($@"{indent}[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]");
        WriteLine($@"{indent}[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{ProductName}"", ""{ProductVersion}"")]");
    }

    public void WriteComment(string indent, string summary)
    {
        WriteLine($"{indent}/// <summary>");
        var comments = new StringReader(summary);
        while(true)
        {
            var comment = HttpUtility.HtmlEncode(comments.ReadLine());
            if(comment == null)
                break;
            WriteLine($"{indent}/// <para>{comment}</para>");
        }
        WriteLine($@"{indent}/// </summary>");
    }

    public void WriteInterfaces(Dictionary<string, Dictionary<string, object>> names)
    {
        var indent = "    ";
        WriteLine($"namespace {Properties.InterfacesNamespace}");
        WriteLine($"{{");
        WriteAttributsForInterface(indent);
        WriteLine($"{indent}{Properties.Modifier}interface IResourceProvider");
        WriteLine($"{indent}{{");
        WriteLine($"{indent}    string this[string key] {{ get; }}");
        WriteLine($"{indent}}}");
        WriteLine($"");
        WriteAttributsForInterface(indent);
        WriteLine($"{indent}{Properties.Modifier}interface IRootResourceProvider : {Properties.InterfaceFullName("IResourceProvider", null)}");
        WriteLine($"{indent}{{");
        WriteLine($"{indent}    void Reset();");
        WriteLine($"{indent}}}");
        WriteLine($"}}");

        foreach(var item in names)
        {
            WriteRootInterface("", Properties.InterfacesNamespace, Helper.Refine(item.Key), item.Value);
        }
    }

    public void WriteInterface(string indent, string ns, string interfaceName, string inhertFrom, Dictionary<string, object> names)
    {
        WriteLine($"");
        WriteLine($"{indent}namespace {ns}");
        WriteLine($"{indent}{{");
        WriteAttributsForInterface(indent + "    ");
        WriteLine($"{indent}    {Properties.Modifier}interface I{interfaceName} : {inhertFrom}");
        WriteLine($"{indent}    {{");
        foreach(var item in names)
        {
            var v = item.Value as string;
            if(v != null)
                WriteInterfaceProperty(indent + "        ", item.Key, v);
            else
                WriteInterfaceIProperty(indent + "        ", item.Key, $"{ns}.{interfaceName}");
        }
        WriteLine($"{indent}    }}");
        WriteLine($"{indent}}}");
        foreach(var item in names)
        {
            var v = item.Value as Dictionary<string, object>;
            if(v != null)
                WriteInnerInterface(indent, $"{ns}.{interfaceName}", Helper.Refine(item.Key), v);
        }
    }

    public void WriteRootInterface(string indent, string ns, string interfaceName, Dictionary<string, object> names)
    {
        WriteInterface(indent, ns, interfaceName, Properties.InterfaceFullName("IRootResourceProvider", ""), names);
    }

    public void WriteInnerInterface(string indent, string ns, string interfaceName, Dictionary<string, object> names)
    {
        WriteInterface(indent, ns, interfaceName, Properties.InterfaceFullName("IResourceProvider", ""), names);
    }

    public void WriteInterfaceProperty(string indent, string key, string value)
    {
        WriteLine("");
        WriteComment(indent, value);
        WriteLine($@"{indent}string {Helper.Refine(key)} {{ get; }}");
    }

    public void WriteInterfaceIProperty(string indent, string key, string ins)
    {
        var p = Helper.Refine(key);
        WriteLine("");
        WriteLine($@"{indent}{Properties.InterfaceFullName("I" + p, ins)} {p} {{ get; }}");
    }

    public void WriteResources(Dictionary<string, Dictionary<string, object>> names)
    {
        var indent = Properties.LocalizedStringsNamespace == null ? "" : "    ";
        if(Properties.LocalizedStringsNamespace != null)
        {
            WriteLine($"namespace {Properties.LocalizedStringsNamespace}");
            WriteLine($"{{");
        }
        WriteAttributsForClass(indent);
        WriteLine($"{indent}{Properties.Modifier}static class {Properties.LocalizedStringsClassName}");
        WriteLine($"{indent}{{");
        foreach(var item in names)
        {
            WriteRootResource(indent + "    ", item.Key, item.Value);
        }
        WriteLine($"{indent}}}");
        if(Properties.LocalizedStringsNamespace != null)
        {
            WriteLine($"}}");
        }
    }

    public void WriteRootResource(string indent, string resourceName, Dictionary<string, object> names)
    {
        var resourcePath = (Properties.IsDefaultProject ? "" : GetProjectAssemblyName()) + "/" + resourceName;
        var cache = Helper.GetRandomName("_cache");
        var loader = Helper.GetRandomName("_loader");

        var propertyName = Helper.Refine(resourceName);
        var propertyFullName = $"{Properties.LocalizedStringsFullName}.{propertyName}";
        var className = Helper.GetRandomName(propertyName);
        var interfaceName = "I" + propertyName;
        var interfaceFullName = Properties.InterfaceFullName(propertyFullName);
        var classFullName = $"{Properties.LocalizedStringsFullName}.{className}";
        WriteLine($"");
        WriteLine($"{indent}{Properties.Modifier}static {interfaceFullName} {propertyName} {{ get; }} = new {classFullName}();");
        WriteLine($"");
        WriteLine($@"{indent}[System.Diagnostics.DebuggerDisplay(""\\{{{Helper.AsLiterital(resourceName)}\\}}"")]");
        WriteLine($"{indent}private sealed class {className} : {interfaceFullName}");
        WriteLine($"{indent}{{");
        WriteLine($"{indent}    private global::System.Collections.Generic.Dictionary<string, string> {cache};");
        WriteLine($"{indent}    private global::Windows.ApplicationModel.Resources.ResourceLoader {loader};");
        WriteLine($"");
        WriteLine($"{indent}    public string this[string resourceKey]");
        WriteLine($"{indent}    {{");
        WriteLine($"{indent}        get");
        WriteLine($"{indent}        {{");
        WriteLine($"{indent}            string value;");
        WriteLine($"{indent}            if(this.{cache}.TryGetValue(resourceKey, out value))");
        WriteLine($"{indent}                return value;");
        WriteLine($"{indent}            else");
        WriteLine($"{indent}                return this.{cache}[resourceKey] = this.{loader}.GetString(resourceKey);");
        WriteLine($"{indent}        }}");
        WriteLine($"{indent}    }}");
        WriteLine($"");
        WriteLine($"{indent}    public {className}()");
        WriteLine($"{indent}    {{");
        WriteLine($"{indent}        this.Reset();");
        WriteLine($"{indent}    }}");
        WriteLine($"");
        WriteLine($"{indent}    public void Reset()");
        WriteLine($"{indent}    {{");
        WriteLine($"{indent}        this.{cache} = new global::System.Collections.Generic.Dictionary<string, string>();");
        WriteLine($"{indent}        this.{loader} = global::Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse(\"{resourcePath}\");");
        WriteLine($"{indent}    }}");
        foreach(var item in names)
        {
            var v = item.Value as string;
            if(v != null)
                WriteProperty(indent + "    ", propertyFullName, "", item.Key, v);
            else
                WriteInnerResource(indent + "    ", propertyFullName, "", propertyFullName, classFullName, item.Key, (Dictionary<string, object>)item.Value);
        }
        WriteLine($"{indent}}}");
    }

    public void WriteInnerResource(string indent, string rootResource, string parentResource, string parentProperty, string parentClass, string key, Dictionary<string, object> value)
    {
        var pName = Helper.Refine(key);
        var fullPName = $"{parentProperty}.{pName}";
        var cName = Helper.GetRandomName(pName);
        var fullPath = Helper.CombineResourcePath(parentResource, key);
        var fullCName = $"{parentClass}.{cName}";
        var iName = "I" + pName;
        var fullIName = Properties.InterfaceFullName(fullPName);
        WriteLine($@"");
        WriteLine($@"{indent}public {fullIName} {pName} {{ get; }} = new {fullCName}();");
        WriteLine($@"");
        WriteLine($@"{indent}[System.Diagnostics.DebuggerDisplay(""\\{{{Helper.AsLiterital(key)}\\}}"")]");
        WriteLine($@"{indent}private sealed class {cName} : {fullIName}");
        WriteLine($@"{indent}{{");
        WriteLine($@"{indent}    public string this[string resourceKey]");
        WriteLine($@"{indent}    {{");
        WriteLine($@"{indent}        get");
        WriteLine($@"{indent}        {{");
        WriteLine($@"{indent}            if(resourceKey == null)");
        WriteLine($@"{indent}                return {rootResource}[""{Helper.AsLiterital(fullPath)}""];");
        WriteLine($@"{indent}            return {rootResource}[""{Helper.AsLiterital(fullPath)}/"" + resourceKey];");
        WriteLine($@"{indent}        }}");
        WriteLine($@"{indent}    }}");
        foreach(var item in value)
        {
            var v = item.Value as string;
            if(v != null)
                WriteProperty(indent + "    ", rootResource, fullPath, item.Key, v);
            else
                WriteInnerResource(indent + "    ", rootResource, fullPath, fullPName, fullCName, item.Key, (Dictionary<string, object>)item.Value);
        }
        WriteLine($@"{indent}}}");
    }

    public void WriteProperty(string indent, string rootResource, string parent, string key, string value)
    {
        var fullPath = Helper.CombineResourcePath(parent, key);
        WriteLine("");
        WriteLine($@"{indent}public string {Helper.Refine(key)} => {rootResource}[""{Helper.AsLiterital(fullPath)}""];");
    }

    public void SetValue(Dictionary<string, Dictionary<string, object>> output, string root, IList<string> path, string value)
    {
        Dictionary<string, object> o;
        output.TryGetValue(root, out o);
        if(o == null)
            output[root] = o = new Dictionary<string, object>();
        SetValueCore(o, path, 0, value);
    }

    public void SetValueCore(Dictionary<string, object> output, IList<string> path, int index, string value)
    {
        if(index == path.Count - 1)
            SetValueCore(output, path[index], value);
        else
        {
            object o;
            output.TryGetValue(path[index], out o);
            var dic = o as Dictionary<string, object>;
            if(dic == null)
                output[path[index]] = dic = new Dictionary<string, object>();
            SetValueCore(dic, path, index + 1, value);
        }
    }

    public void SetValueCore(Dictionary<string, object> output, string path, string value)
    {
        output[path] = value;
    }

    public void AnalyzeResw(Dictionary<string, Dictionary<string, object>> output, string path)
    {
        var className = Path.GetFileNameWithoutExtension(path);
        var document = new XmlDocument();
        document.Load(path);

        var dataNodes = document.GetElementsByTagName("data");
        foreach(XmlElement dataNode in dataNodes)
        {
            if(dataNode != null)
            {
                var value = dataNode.GetAttribute("name");
                SetValue(output, className, value.Split('.', '/'), dataNode["value"].InnerText);
            }
        }
    }

    public void AnalyzeResJson(Dictionary<string, Dictionary<string, object>> output, string path)
    {
        var className = Path.GetFileNameWithoutExtension(path);
        using(var reader = new StreamReader(path, true))
        using(var ms = new MemoryStream())
        using(var writer = new StreamWriter(ms, new System.Text.UTF8Encoding(false)))
        {
            var line = default(string);
            while((line = reader.ReadLine()) != null)
            {
                writer.WriteLine(line);
            }
            writer.Flush();
            ms.Position = 0;
            var xmlreader = JsonReaderWriterFactory.CreateJsonReader(ms, new XmlDictionaryReaderQuotas());
            var doc = new XmlDocument();
            doc.Load(xmlreader);
            var root = doc.FirstChild;
            var p = new List<string>();
            foreach(XmlElement item in root.ChildNodes)
            {
                AnalyzeNode(output, className, p, item);
            }

        }
    }

    public void AnalyzeNode(Dictionary<string, Dictionary<string, object>> output, string root, List<string> path, XmlElement node)
    {
        var nodeName = GetNodeName(node);
        if(string.IsNullOrWhiteSpace(nodeName) || nodeName.StartsWith("_"))
            return;
        var currentName = GetNodeName(node).Split('/');
        path.AddRange(currentName);
        if(node.FirstChild.NodeType == XmlNodeType.Text)
        {
            var text = default(string);
            foreach(XmlText item in node.ChildNodes)
            {
                text += item.InnerText;
            }
            SetValue(output, root, path, text);
        }
        else
        {
            foreach(XmlElement item in node.ChildNodes)
            {
                AnalyzeNode(output, root, path, item);
            }
        }
        var currentNameLength = currentName.Length;
        path.RemoveRange(path.Count - currentNameLength, currentNameLength);
    }

    public string GetNodeName(XmlElement node)
    {
        var name = node.GetAttribute("item");
        if(string.IsNullOrEmpty(name))
            return node.Name;
        return name;
    }

    public static class Helper
    {
        private static HashSet<string> keywords = new HashSet<string>() { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while" };

        private static HashSet<string> used = new HashSet<string>() { "Reset", "MemberwiseClone", "ReferenceEquals", "Equals", "GetType", "GetHashCode" };

        public static string CombineResourcePath(string root, string reletive) => string.IsNullOrEmpty(root) ? reletive : $"{root}/{reletive}";

        public static string AsLiterital(string value)
        {
            if(string.IsNullOrEmpty(value))
                return "";
            var sb = new System.Text.StringBuilder(value.Length);
            foreach(var item in value)
            {
                switch(item)
                {
                case '"':
                case '\\':
                case '\'':
                case '\r':
                case '\t':
                case '\n':
                case '\0':
                case '\v':
                case '\a':
                case '\b':
                    sb.Append(@"\u");
                    sb.Append(((ushort)item).ToString("X4"));
                    break;
                default:
                    sb.Append(item);
                    break;
                }
            }
            return sb.ToString();
        }

        public static string Refine(string name, bool allowDot = false)
        {
            if(allowDot)
                return string.Join(".", name.Split('.').Select(n => Refine(n)));
            if(keywords.Contains(name))
                return "@" + name;
            if(used.Contains(name))
                return name + "_";
            if(string.IsNullOrWhiteSpace(name))
                return GetRandomName("__");
            if(!isValidStartChar(name[0]))
                name = "_" + name;
            for(var i = 1; i < name.Length; i++)
            {
                if(!isValidPartChar(name[i]))
                    name = name.Replace(name[i], '_');
            }
            return name;
        }

        private static bool isValidStartChar(char ch) => ch == '_' || isLetter(ch);

        private static bool isValidPartChar(char ch)
        {
            var c = (int)char.GetUnicodeCategory(ch);
            return c == 5 || c == 6 || c == 8 || c == 18 || c == 15 || isLetter(ch);
        }

        private static bool isLetter(char ch)
        {
            var c = (int)char.GetUnicodeCategory(ch);
            return c < 5 || c == 9;
        }

        private static Random ran = new Random();

        public static string GetRandomName(string head) => $"{head}__{ran.Next():X8}";
    }

    public string GetProjectPath()
    {
        return Host.ResolveAssemblyReference("$(ProjectDir)");
    }

    public string GetProjectDefaultNamespace()
    {
        var serviceProvider = (IServiceProvider)this.Host;
        var dte = (EnvDTE.DTE)serviceProvider.GetService(typeof(EnvDTE.DTE));
        var project = (EnvDTE.Project)dte.Solution.FindProjectItem(this.Host.TemplateFile).ContainingProject;
        return project.Properties.Item("DefaultNamespace").Value.ToString();
    }

    public string GetProjectAssemblyName()
    {
        var serviceProvider = (IServiceProvider)this.Host;
        var dte = (EnvDTE.DTE)serviceProvider.GetService(typeof(EnvDTE.DTE));
        var project = (EnvDTE.Project)dte.Solution.FindProjectItem(this.Host.TemplateFile).ContainingProject;
        return project.Properties.Item("AssemblyName").Value.ToString();
    }
}
