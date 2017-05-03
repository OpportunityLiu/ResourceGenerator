using Opportunity.ResourceGenerator.Generator.ResourceProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Opportunity.ResourceGenerator.Generator
{
    class ResourceWriter : FileWriter
    {
        public Configuration Config => Configuration.Current;

        public ResourceWriter(string path) : base(path)
        {
        }

        public void Execute()
        {
            if (Config.SourceLanguagePath == null)
                Config.SourceLanguagePath = Directory.EnumerateDirectories(Path.Combine(Config.ProjectPath, Config.ResourcePath)).First();
            var stringsPath = Path.Combine(Config.ProjectPath, Config.ResourcePath, Config.SourceLanguagePath);
            string[] files;

            if (Directory.Exists(stringsPath))
            {
                files = Directory.GetFiles(stringsPath);
            }
            else
            {
                files = new string[0];
            }
            foreach (var item in files)
            {
                WriteHash(item);
            }
            WriteInfo();
            var tree = Provider.Analyze(files);
            WriteInterfaces(tree);
            WriteLine();
            WriteResources(tree);
        }

        private void Check(Node node)
        {
            if (!Helper.Keywords.Contains(node.Name) && node.Name != node.PName)
            {
                WriteLine($"#warning Resource has been renamed. ResourceName: \"{Helper.AsLiteral(node.Name)}\", PropertyName: \"{Helper.AsLiteral(node.PName)}\"");
            }
        }

        public void WriteAttributsForInterface(int indent)
        {
            WriteLine(indent, $@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Helper.ProductName}"", ""{Helper.ProductVersion}"")]");
        }

        public void WriteAttributsForClass(int indent)
        {
            WriteLine(indent, "[global::System.Diagnostics.DebuggerTypeProxy(typeof(global::Opportunity.ResourceGenerator.DebuggerDisplay))]");
            if (!Config.DebugGeneratedCode)
                WriteLine(indent, $@"[global::System.Diagnostics.DebuggerNonUserCodeAttribute]");
            WriteLine(indent, $@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Helper.ProductName}"", ""{Helper.ProductVersion}"")]");
        }

        public void WriteComment(int indent, string summary)
        {
            WriteLine(indent, "/// <summary>");
            var comments = new StringReader(summary);
            while (true)
            {
                var comment = HttpUtility.HtmlEncode(comments.ReadLine());
                if (comment == null)
                    break;
                WriteLine(indent, $@"/// <para>{comment}</para>");
            }
            WriteLine(indent, $@"/// </summary>");
        }

        public void WriteInterfaces(IList<RootNode> tree)
        {
            foreach (var item in tree)
            {
                WriteRootInterface(item);
            }
        }

        public void WriteInterface(int indent, string inhertFrom, Node node)
        {
            WriteLine();
            WriteLine(indent, $"namespace {node.INs}");
            WriteLine(indent, $"{{");
            WriteAttributsForInterface(indent + 1);
            WriteLine(indent, $"    {Config.Modifier} interface {node.IName} : {inhertFrom}");
            WriteLine(indent, $"    {{");
            foreach (var item in node.Childern)
            {
                if (item.IsLeaf)
                    WriteInterfaceProperty(indent + 2, item);
                else
                    WriteInterfaceIProperty(indent + 2, item);
            }
            WriteLine(indent, $"    }}");
            WriteLine(indent, $"}}");
            foreach (var item in node.Childern)
            {
                if (!item.IsLeaf)
                    WriteInnerInterface(indent, item);
            }
        }

        public void WriteRootInterface(RootNode node)
        {
            WriteInterface(0, Config.IRPFullName, node);
        }

        public void WriteInnerInterface(int indent, Node node)
        {
            WriteInterface(indent, Config.IRPFullName, node);
        }

        public void WriteInterfaceProperty(int indent, Node node)
        {
            WriteComment(indent, node.Value);
            Check(node);
            this.WriteLine(indent, $@"{Helper.IPropModifier(node.PName)}string {node.PName} {{ get; }}");
        }

        public void WriteInterfaceIProperty(int indent, Node node)
        {
            Check(node);
            WriteLine(indent, $@"{Helper.IPropModifier(node.PName)}{node.IFName} {node.PName} {{ get; }}");
        }

        public void WriteResources(IList<RootNode> tree)
        {
            var indent = 1;
            WriteLine($"namespace {Config.LocalizedStringsNamespace}");
            WriteLine($"{{");
            WriteAttributsForClass(indent);
            WriteLine(indent, $"{Config.Modifier} static class {Config.LocalizedStringsClassName}");
            WriteLine(indent, $"{{");
            foreach (var item in tree)
            {
                WriteRootResource(indent + 1, item);
            }
            WriteLine(indent, $"}}");
            WriteLine($"}}");
        }

        public void WriteRootResource(int indent, RootNode node)
        {
            WriteLine();
            WriteLine(indent, "[global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]");
            WriteLine(indent, $"private static {node.IFName} {node.FName};");
            WriteLine(indent, $"{Config.Modifier} static {node.IFName} {node.PName} ");
            WriteLine(indent, $"    => global::System.Threading.LazyInitializer.EnsureInitialized(ref {node.FName}, () => new {node.CFName}());");
            WriteLine();
            WriteAttributsForClass(indent);
            WriteLine(indent, $@"[global::System.Diagnostics.DebuggerDisplay(""[{Helper.AsLiteral(node.RName)}]"")]");
            WriteLine(indent, $"private sealed class {node.CName} : {node.IFName}");
            WriteLine(indent, $"{{");
            WriteLine(indent, $"    {Config.GRPFullName} {Config.IRPFullName}.this[string resourceKey]");
            WriteLine(indent, $"    {{");
            WriteLine(indent, $"        get");
            WriteLine(indent, $"        {{");
            WriteLine(indent, $"            if(resourceKey == null)");
            WriteLine(indent, $"                throw new global::System.ArgumentNullException();");
            WriteLine(indent, $"            return new {Config.GRPFullName}(\"{Helper.AsLiteral(node.RName)}/\" + resourceKey);");
            WriteLine(indent, $"        }}");
            WriteLine(indent, $"    }}");
            WriteLine();
            WriteLine(indent, $"    string {Config.IRPFullName}.GetValue(string resourceKey)");
            WriteLine(indent, $"    {{");
            WriteLine(indent, $"        return {Config.RLFullName}.GetValue(\"{Helper.AsLiteral(node.RName)}/\" + resourceKey);");
            WriteLine(indent, $"    }}");
            WriteLine();
            foreach (var item in node.Childern)
            {
                if (!item.IsLeaf)
                    WriteInnerResource(indent + 1, item);
            }
            WriteLine();
            foreach (var item in node.Childern)
            {
                if (item.IsLeaf)
                    WriteProperty(indent + 1, item);
            }
            WriteLine(indent, $"}}");
        }

        public void WriteInnerResource(int indent, Node node)
        {
            WriteLine();
            WriteLine(indent, "[global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]");
            WriteLine(indent, $"private {node.IFName} {node.FName};");
            WriteLine(indent, $"{node.IFName} {node.Parent.IFName}.{node.PName} ");
            WriteLine(indent, $"    => global::System.Threading.LazyInitializer.EnsureInitialized(ref {node.FName}, () => new {node.CFName}());");
            WriteLine();
            WriteAttributsForClass(indent);
            WriteLine(indent, $@"[global::System.Diagnostics.DebuggerDisplay(""[{Helper.AsLiteral($"{node.RName}")}]"", Name = ""{Helper.AsLiteral(node.PName)}"")]");
            WriteLine(indent, $@"private sealed class {node.CName} : {node.IFName}");
            WriteLine(indent, $@"{{");
            WriteLine(indent, $@"    {Config.GRPFullName} {Config.IRPFullName}.this[string resourceKey]");
            WriteLine(indent, $@"    {{");
            WriteLine(indent, $@"        get");
            WriteLine(indent, $@"        {{");
            WriteLine(indent, $@"            if(resourceKey == null)");
            WriteLine(indent, $@"                throw new global::System.ArgumentNullException();");
            WriteLine(indent, $@"            return new {Config.GRPFullName}(""{Helper.AsLiteral(node.RName)}/"" + resourceKey);");
            WriteLine(indent, $@"        }}");
            WriteLine(indent, $@"    }}");
            WriteLine();
            WriteLine(indent, $@"    string {Config.IRPFullName}.GetValue(string resourceKey)");
            WriteLine(indent, $@"    {{");
            WriteLine(indent, $@"        if(resourceKey == null)");
            WriteLine(indent, $@"            return {Config.RLFullName}.GetValue(""{Helper.AsLiteral(node.RName)}"");");
            WriteLine(indent, $@"        return {Config.RLFullName}.GetValue(""{Helper.AsLiteral(node.RName)}/"" + resourceKey);");
            WriteLine(indent, $@"    }}");
            foreach (var item in node.Childern)
            {
                if (!item.IsLeaf)
                    WriteInnerResource(indent + 1, item);
            }
            WriteLine();
            foreach (var item in node.Childern)
            {
                if (item.IsLeaf)
                    WriteProperty(indent + 1, item);
            }
            WriteLine(indent, $@"}}");
        }

        public void WriteProperty(int indent, Node node)
        {
            WriteLine(indent, $@"string {node.Parent.IFName}.{node.PName}");
            WriteLine(indent + 1, $@"=> {Config.RLFullName}.GetValue(""{Helper.AsLiteral(node.RName)}"");");
        }
    }
}
