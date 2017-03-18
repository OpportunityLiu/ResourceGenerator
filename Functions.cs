﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Linq;
using System.Web;

namespace ResourceGenerator
{
    public partial class Functions
    {
        private void Init()
        {
            Properties.ProjectAssemblyName = GetProjectAssemblyName();
            Properties.ProjectDefaultNamespace = GetProjectDefaultNamespace();
            Properties.ProjectPath = GetProjectPath();
            Properties.InterfacesNamespace = null;
            Properties.LocalizedStringsNamespace = null;
            Properties.LocalizedStringsClassName = null;
        }

        public void Execute()
        {
            if(Properties.SourceLanguagePath == null)
                Properties.SourceLanguagePath = Directory.EnumerateDirectories(Path.Combine(Properties.ProjectPath, Properties.ResourcePath)).First();
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
            var tree = ResourceRootNode.GetTree(names);
            WriteInterfaces(tree);
            WriteLine();
            WriteResources(tree);
        }

        public PropertiesClass Properties => PropertiesClass.Current;

        public class PropertiesClass
        {
            public static PropertiesClass Current { get; } = new PropertiesClass();

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
            public string SourceLanguagePath { get; set; }
            public bool IsDefaultProject { get; set; } = true;

            public string CacheActivator
            {
                get => ca;
                set
                {
                    if(!string.IsNullOrWhiteSpace(value))
                        this.ca = value;
                }
            }

            private string rns;
            private string rc;
            private string ins;
            private string mf = "internal";
            private string ca = "new global::System.Collections.Generic.Dictionary<string, string>()";
            public string LocalizedStringsNamespace
            {
                get => this.rns;
                set => setIdentity(ref this.rns, value, ProjectDefaultNamespace, true);
            }
            public string LocalizedStringsClassName
            {
                get => this.rc;
                set => setIdentity(ref this.rc, value, "LocalizedStrings", false);
            }
            public string InterfacesNamespace
            {
                get => this.ins;
                set => setIdentity(ref this.ins, value, $"{ProjectDefaultNamespace}.{ProjectDefaultNamespace}_ResourceInfo", true);
            }
            public string Modifier
            {
                get => this.mf; set
                {
                    value = (value ?? "").Trim();
                    this.mf = string.IsNullOrEmpty(value) ? "internal" : value;
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

            public string IRPName => "IResourceProvider";
            public string IGRPName => "IGeneratedResourceProvider";
            public string GRPName => "GeneratedResourceProvider";

            public string IRPFullName => InterfaceFullName(IRPName, null);
            public string IGRPFullName => InterfaceFullName(IGRPName, null);
            public string GRPFullName => InterfaceFullName(GRPName, null);
        }

        public class ResourceRootNode : ResourceNode
        {
            public static List<ResourceRootNode> GetTree(Dictionary<string, Dictionary<string, object>> raw)
            {
                var l = new List<ResourceRootNode>();
                foreach(var item in raw)
                {
                    l.Add(getRootNode(item));
                }
                return l;
            }

            private static ResourceRootNode getRootNode(KeyValuePair<string, Dictionary<string, object>> item)
            {
                var node = new ResourceRootNode(item.Key);
                foreach(var i in item.Value)
                {
                    if(i.Value is string v)
                    {
                        node.Childern.Add(GetNode(i.Key, v));
                    }
                    else if(i.Value is Dictionary<string, object> v2)
                    {
                        node.Childern.Add(GetNode(i.Key, v2));
                    }
                }
                node.Refine();
                return node;
            }

            public ResourceRootNode(string name)
                : base(name)
            {
                this.Root = this;
                this.Parent = null;
            }

            public override string RName
            {
                get
                {
                    if(PropertiesClass.Current.IsDefaultProject)
                        return $"ms-resource:///{Name}";
                    else
                        return $"ms-resource:///{PropertiesClass.Current.ProjectAssemblyName}/{Name}";
                }
            }
            public override string INs => PropertiesClass.Current.InterfacesNamespace;
            public override string CFName => $"{PropertiesClass.Current.LocalizedStringsFullName}.{CName}";
            public override string PFName => $"{PropertiesClass.Current.LocalizedStringsFullName}.{PName}";
        }

        public class ResourceNode
        {
            public static ResourceNode GetNode(string name, string value)
            {
                return new ResourceNode(name, value);
            }

            public static ResourceNode GetNode(string name, Dictionary<string, object> values)
            {
                var r = new ResourceNode(name);
                foreach(var i in values)
                {
                    if(i.Value is string v)
                    {
                        r.Childern.Add(GetNode(i.Key, v));
                    }
                    else if(i.Value is Dictionary<string, object> v2)
                    {
                        r.Childern.Add(GetNode(i.Key, v2));
                    }
                }
                return r;
            }

            public ResourceNode(string name, string value)
            {
                this.Name = name;
                this.Value = value;
                setNames();
            }

            public ResourceNode(string name)
            {
                this.Name = name;
                this.Childern = new List<ResourceNode>();
                setNames();
            }

            protected void Refine()
            {
                if(Childern == null)
                    return;
                foreach(var item in Childern)
                {
                    item.Parent = this;
                    item.Root = this.Root;
                    item.Refine();
                }
            }

            private void setNames()
            {
                var r = Helper.Refine(Name);
                if(r.StartsWith("@"))
                    this.IName = $"I{Name}";
                else
                    this.IName = $"I{r}";
                this.CName = Helper.Refine(Helper.GetRandomName(Name));
                this.PName = r;
            }

            public IList<ResourceNode> Childern { get; }

            public ResourceNode Parent { get; protected set; }

            public ResourceRootNode Root { get; protected set; }

            public bool IsLeaf => Value != null;

            public string Name { get; }
            public string Value { get; }

            public virtual string RName => Helper.CombineResourcePath(Parent?.RName, Name);

            public string IName { get; private set; }
            public virtual string INs => $"{Parent.INs}.{Parent.PName}";

            public string CName { get; private set; }

            public string PName { get; private set; }

            public string IFName => PropertiesClass.Current.InterfaceFullName(IName, INs);

            public virtual string CFName => $"{Parent.CFName}.{CName}";

            public virtual string PFName => $"{Parent.CFName}.{PName}";
        }

        private void Check(ResourceNode node)
        {
            if(!Helper.Keywords.Contains(node.Name) && node.Name != node.PName)
            {
                WriteLine($"#warning Resource has been renamed. ResourceName: \"{Helper.AsLiteral(node.Name)}\", PropertyName: \"{Helper.AsLiteral(node.PName)}\"");
            }
        }

        public void WriteLine(int indent, string value)
        {
            for(var i = 0; i < indent; i++)
                Write("    ");
            WriteLine(value);
        }

        public void WriteLine()
        {
            WriteLine("");
        }

        public void WriteAttributsForInterface(int indent)
        {
            WriteLine(indent, $@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{ProductName}"", ""{ProductVersion}"")]");
        }

        public void WriteAttributsForClass(int indent)
        {
            if(!Properties.DebugGeneratedCode)
                WriteLine(indent, $@"[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]");
            WriteLine(indent, $@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{ProductName}"", ""{ProductVersion}"")]");
        }

        public void WriteComment(int indent, string summary)
        {
            WriteLine(indent, "/// <summary>");
            var comments = new StringReader(summary);
            while(true)
            {
                var comment = HttpUtility.HtmlEncode(comments.ReadLine());
                if(comment == null)
                    break;
                WriteLine(indent, $@"/// <para>{comment}</para>");
            }
            WriteLine(indent, $@"/// </summary>");
        }

        public void WriteInterfaces(IList<ResourceRootNode> tree)
        {
            var indent = 1;
            WriteLine($"namespace {Properties.InterfacesNamespace}");
            WriteLine($"{{");
            WriteAttributsForInterface(indent);
            WriteLine(indent, $@"{Properties.Modifier} interface {Properties.IRPName}");
            WriteLine(indent, $"{{");
            WriteLine(indent, $"    {Properties.GRPFullName} this[string resourceKey] {{ get; }}");
            WriteLine(indent, $"    string GetValue(string resourceKey);");
            WriteLine(indent, $"}}");
            WriteLine();
            WriteAttributsForInterface(indent);
            WriteLine(indent, $"{Properties.Modifier} interface {Properties.IGRPName} : {Properties.IRPFullName}");
            WriteLine(indent, $"{{");
            WriteLine(indent, $"    string Value {{ get; }}");
            WriteLine(indent, $"}}");
            WriteLine();
            WriteAttributsForClass(indent);
            WriteLine(indent, $@"[System.Diagnostics.DebuggerDisplay(""\\{{{{key,nq}}\\}}"")]");
            WriteLine(indent, $"{Properties.Modifier} struct {Properties.GRPName} : {Properties.IGRPFullName}");
            WriteLine(indent, $"{{");
            WriteLine(indent, $"    {Properties.Modifier} {Properties.GRPName}(string key)");
            WriteLine(indent, $"    {{");
            WriteLine(indent, $"        this.key = key;");
            WriteLine(indent, $"    }}");
            WriteLine();
            WriteLine(indent, $"    private readonly string key;");
            WriteLine();
            WriteLine(indent, $"    public string Value => {Properties.LocalizedStringsFullName}.GetValue(key);");
            WriteLine();
            WriteLine(indent, $"    public {Properties.GRPName} this[string resourceKey]");
            WriteLine(indent, $"    {{");
            WriteLine(indent, $"        get");
            WriteLine(indent, $"        {{");
            WriteLine(indent, $"            if(resourceKey == null)");
            WriteLine(indent, $"                throw new global::System.ArgumentNullException();");
            WriteLine(indent, $"            return new {Properties.GRPFullName}($\"{{key}}/{{resourceKey}}\");");
            WriteLine(indent, $"        }}");
            WriteLine(indent, $"    }}");
            WriteLine();
            WriteLine(indent, $"    public string GetValue(string resourceKey)");
            WriteLine(indent, $"    {{");
            WriteLine(indent, $"        if(resourceKey == null)");
            WriteLine(indent, $"            return this.Value;");
            WriteLine(indent, $"        return {Properties.LocalizedStringsFullName}.GetValue($\"{{key}}/{{resourceKey}}\");");
            WriteLine(indent, $"    }}");
            WriteLine(indent, $"}}");
            WriteLine($"}}");

            foreach(var item in tree)
            {
                WriteRootInterface(item);
            }
        }

        public void WriteInterface(int indent, string inhertFrom, ResourceNode node)
        {
            WriteLine();
            WriteLine(indent, $"namespace {node.INs}");
            WriteLine(indent, $"{{");
            WriteAttributsForInterface(indent + 1);
            WriteLine(indent, $"    {Properties.Modifier} interface {node.IName} : {inhertFrom}");
            WriteLine(indent, $"    {{");
            foreach(var item in node.Childern)
            {
                if(item.IsLeaf)
                    WriteInterfaceProperty(indent + 2, item);
                else
                    WriteInterfaceIProperty(indent + 2, item);
            }
            WriteLine(indent, $"    }}");
            WriteLine(indent, $"}}");
            foreach(var item in node.Childern)
            {
                if(!item.IsLeaf)
                    WriteInnerInterface(indent, item);
            }
        }

        public void WriteRootInterface(ResourceRootNode node)
        {
            WriteInterface(0, Properties.IRPFullName, node);
        }

        public void WriteInnerInterface(int indent, ResourceNode node)
        {
            WriteInterface(indent, Properties.IRPFullName, node);
        }

        public void WriteInterfaceProperty(int indent, ResourceNode node)
        {
            WriteComment(indent, node.Value);
            Check(node);
            this.WriteLine(indent, $@"{Helper.IPropModifier(node.PName)}string {node.PName} {{ get; }}");
        }

        public void WriteInterfaceIProperty(int indent, ResourceNode node)
        {
            Check(node);
            WriteLine(indent, $@"{Helper.IPropModifier(node.PName)}{node.IFName} {node.PName} {{ get; }}");
        }

        public void WriteResources(IList<ResourceRootNode> tree)
        {
            var indent = 1;
            var loaderName = Helper.GetRandomName("__loader");
            var cacheName = Helper.GetRandomName("__cache");
            WriteLine($"namespace {Properties.LocalizedStringsNamespace}");
            WriteLine($"{{");
            WriteAttributsForClass(indent);
            WriteLine(indent, $"{Properties.Modifier} static class {Properties.LocalizedStringsClassName}");
            WriteLine(indent, $"{{");
            WriteLine(indent, $"    private static readonly global::System.Collections.Generic.IDictionary<string, string> {cacheName}");
            WriteLine(indent, $"        = {Properties.CacheActivator};");
            WriteLine(indent, $"    private static readonly global::Windows.ApplicationModel.Resources.ResourceLoader {loaderName}");
            WriteLine(indent, $"        = global::Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();");
            WriteLine();
            WriteLine(indent, $"    {Properties.Modifier} static string GetValue(string resourceKey)");
            WriteLine(indent, $"    {{");
            WriteLine(indent, $"        string value;");
            WriteLine(indent, $"        if({Properties.LocalizedStringsFullName}.{cacheName}.TryGetValue(resourceKey, out value))");
            WriteLine(indent, $"            return value;");
            WriteLine(indent, $"        return {Properties.LocalizedStringsFullName}.{cacheName}[resourceKey] = {Properties.LocalizedStringsFullName}.{loaderName}.GetString(resourceKey);");
            WriteLine(indent, $"    }}");
            WriteLine();
            foreach(var item in tree)
            {
                WriteRootResource(indent + 1, item);
            }
            WriteLine(indent, $"}}");
            WriteLine($"}}");
        }

        public void WriteRootResource(int indent, ResourceRootNode node)
        {
            WriteLine();
            WriteLine(indent, $"{Properties.Modifier} static {node.IFName} {node.PName} {{ get; }} = new {node.CFName}();");
            WriteLine();
            WriteAttributsForClass(indent);
            WriteLine(indent, $@"[System.Diagnostics.DebuggerDisplay(""\\{{{Helper.AsLiteral(node.RName)}\\}}"")]");
            WriteLine(indent, $"private sealed class {node.CName} : {node.IFName}");
            WriteLine(indent, $"{{");
            WriteLine(indent, $"    {Properties.GRPFullName} {Properties.IRPFullName}.this[string resourceKey]");
            WriteLine(indent, $"    {{");
            WriteLine(indent, $"        get");
            WriteLine(indent, $"        {{");
            WriteLine(indent, $"            if(resourceKey == null)");
            WriteLine(indent, $"                throw new global::System.ArgumentNullException();");
            WriteLine(indent, $"            return new {Properties.GRPFullName}(\"{Helper.AsLiteral(node.RName)}/\" + resourceKey);");
            WriteLine(indent, $"        }}");
            WriteLine(indent, $"    }}");
            WriteLine();
            WriteLine(indent, $"    string {Properties.IRPFullName}.GetValue(string resourceKey)");
            WriteLine(indent, $"    {{");
            WriteLine(indent, $"        return {Properties.LocalizedStringsFullName}.GetValue(\"{Helper.AsLiteral(node.RName)}/\" + resourceKey);");
            WriteLine(indent, $"    }}");
            WriteLine();
            foreach(var item in node.Childern)
            {
                if(!item.IsLeaf)
                    WriteInnerResource(indent + 1, item);
            }
            WriteLine();
            foreach(var item in node.Childern)
            {
                if(item.IsLeaf)
                    WriteProperty(indent + 1, item);
            }
            WriteLine(indent, $"}}");
        }

        public void WriteInnerResource(int indent, ResourceNode node)
        {
            WriteLine();
            WriteLine(indent, $@"{node.IFName} {node.Parent.IFName}.{node.PName} {{ get; }} = new {node.CFName}();");
            WriteLine();
            WriteAttributsForClass(indent);
            WriteLine(indent, $@"[System.Diagnostics.DebuggerDisplay(""\\{{{Helper.AsLiteral($"{node.RName}")}\\}}"")]");
            WriteLine(indent, $@"private sealed class {node.CName} : {node.IFName}");
            WriteLine(indent, $@"{{");
            WriteLine(indent, $@"    {Properties.GRPFullName} {Properties.IRPFullName}.this[string resourceKey]");
            WriteLine(indent, $@"    {{");
            WriteLine(indent, $@"        get");
            WriteLine(indent, $@"        {{");
            WriteLine(indent, $@"            if(resourceKey == null)");
            WriteLine(indent, $@"                throw new global::System.ArgumentNullException();");
            WriteLine(indent, $@"            return new {Properties.GRPFullName}(""{Helper.AsLiteral(node.RName)}/"" + resourceKey);");
            WriteLine(indent, $@"        }}");
            WriteLine(indent, $@"    }}");
            WriteLine();
            WriteLine(indent, $@"    string {Properties.IRPFullName}.GetValue(string resourceKey)");
            WriteLine(indent, $@"    {{");
            WriteLine(indent, $@"        if(resourceKey == null)");
            WriteLine(indent, $@"            return {Properties.LocalizedStringsFullName}.GetValue(""{Helper.AsLiteral(node.RName)}"");");
            WriteLine(indent, $@"        return {Properties.LocalizedStringsFullName}.GetValue(""{Helper.AsLiteral(node.RName)}/"" + resourceKey);");
            WriteLine(indent, $@"    }}");
            foreach(var item in node.Childern)
            {
                if(!item.IsLeaf)
                    WriteInnerResource(indent + 1, item);
            }
            WriteLine();
            foreach(var item in node.Childern)
            {
                if(item.IsLeaf)
                    WriteProperty(indent + 1, item);
            }
            WriteLine(indent, $@"}}");
        }

        public void WriteProperty(int indent, ResourceNode node)
        {
            WriteLine(indent, $@"string {node.Parent.IFName}.{node.PName}");
            WriteLine(indent + 1, $@"=> {Properties.LocalizedStringsFullName}.GetValue(""{Helper.AsLiteral(node.RName)}"");");
        }

        public void SetValue(Dictionary<string, Dictionary<string, object>> output, string root, IList<string> path, string value)
        {
            output.TryGetValue(root, out var o);
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
                output.TryGetValue(path[index], out var o);
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
            var resourceName = Path.GetFileNameWithoutExtension(path);
            var document = new XmlDocument();
            document.Load(path);

            var dataNodes = document.GetElementsByTagName("data");
            foreach(XmlElement dataNode in dataNodes)
            {
                if(dataNode != null)
                {
                    var value = dataNode.GetAttribute("name");
                    SetValue(output, resourceName, value.Split('.', '/'), dataNode["value"].InnerText);
                }
            }
        }

        public void AnalyzeResJson(Dictionary<string, Dictionary<string, object>> output, string path)
        {
            var resourceName = Path.GetFileNameWithoutExtension(path);
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
                    AnalyzeNode(output, resourceName, p, item);
                }

            }
        }

        public void AnalyzeNode(Dictionary<string, Dictionary<string, object>> output, string root, List<string> path, XmlElement node)
        {
            var nodeName = GetNodeName(node);
            if(string.IsNullOrEmpty(nodeName) || nodeName.StartsWith("_"))
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
            public static HashSet<string> Keywords = new HashSet<string>() { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while" };

            private static HashSet<string> used = new HashSet<string>() { "Reset", "GetValue" };

            public static string CombineResourcePath(string root, string reletive) => string.IsNullOrEmpty(root) ? reletive : $"{root}/{reletive}";

            public static string AsLiteral(string value)
            {
                if(string.IsNullOrEmpty(value))
                    return "";
                var sb = new System.Text.StringBuilder(value.Length);
                foreach(var item in value)
                {
                    var chType = char.GetUnicodeCategory(item);
                    if(item == '"')
                        sb.Append(@"\""");
                    else if(item == '\\')
                        sb.Append(@"\\");
                    else if(item == '\0')
                        sb.Append(@"\0");
                    else if(item == '\a')
                        sb.Append(@"\a");
                    else if(item == '\b')
                        sb.Append(@"\b");
                    else if(item == '\f')
                        sb.Append(@"\f");
                    else if(item == '\r')
                        sb.Append(@"\r");
                    else if(item == '\n')
                        sb.Append(@"\n");
                    else if(item == '\t')
                        sb.Append(@"\t");
                    else if(item == '\v')
                        sb.Append(@"\v");
                    else if(chType == System.Globalization.UnicodeCategory.Control)
                    {
                        sb.Append(@"\u");
                        sb.Append(((ushort)item).ToString("X4"));
                    }
                    else
                    {
                        sb.Append(item);
                    }
                }
                return sb.ToString();
            }

            public static string IPropModifier(string name)
            {
                if(used.Contains(name))
                    return "new ";
                return "";
            }

            public static string Refine(string name, bool allowDot = false)
            {
                if(allowDot)
                    return string.Join(".", name.Split('.').Select(n => Refine(n)));
                if(string.IsNullOrEmpty(name))
                    return "__Empty__";
                if(Keywords.Contains(name))
                    return "@" + name;
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

            public static string GetRandomName(string head)
            {
                var buf = new byte[6];
                ran.NextBytes(buf);
                var r = Convert.ToBase64String(buf);
                return $"{head}__{r.Replace("+", "_").Replace("/", "_")}";
            }
        }

        public string GetProjectPath()
        {
            return this.Host.ResolveAssemblyReference("$(ProjectDir)");
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
}