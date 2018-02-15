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
                Config.SourceLanguagePath = Directory.EnumerateDirectories(Path.Combine(Config.ProjectDirectory, Config.ResourcePath)).First();
            var stringsPath = Path.Combine(Config.ProjectDirectory, Config.ResourcePath, Config.SourceLanguagePath);
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
            if (!Helper.Keywords.Contains(node.Name) && node.Name != node.PropertyName)
            {
                WriteLine($"#warning Resource has been renamed. ResourceName: \"{Helper.AsLiteral(node.Name)}\", PropertyName: \"{Helper.AsLiteral(node.PropertyName)}\"");
            }
        }

        public void WriteAttributsForInterface(int indent)
        {
            WriteLine(indent, Strings.GeneratedCode);
        }

        public void WriteAttributsForClass(int indent)
        {
            WriteLine(indent, "[global::System.Diagnostics.DebuggerTypeProxyAttribute(typeof(global::Opportunity.ResourceGenerator.DebuggerDisplay))]");
            if (!Config.DebugGeneratedCode)
                WriteLine(indent, Strings.DebuggerNonUserCode);
            WriteLine(indent, Strings.GeneratedCode);
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
            WriteLine(indent, $"namespace {node.InterfaceNamespace}");
            WriteLine(indent, $"{{");
            WriteAttributsForInterface(indent + 1);
            WriteLine(indent, $"    {Config.Modifier} interface {node.InterfaceName} : {inhertFrom}");
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
            WriteInterface(0, Strings.IResourceProvider, node);
        }

        public void WriteInnerInterface(int indent, Node node)
        {
            WriteInterface(indent, Strings.IResourceProvider, node);
        }

        public void WriteInterfaceProperty(int indent, Node node)
        {
            WriteComment(indent, node.Value);
            Check(node);
            this.WriteLine(indent, $@"{Helper.IPropModifier(node.PropertyName)}string {node.PropertyName} {{ get; }}");
        }

        public void WriteInterfaceIProperty(int indent, Node node)
        {
            Check(node);
            WriteLine(indent, $@"{Helper.IPropModifier(node.PropertyName)}{node.InterfaceFullName} {node.PropertyName} {{ get; }}");
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
            WriteLine(indent, Strings.DebuggerNeverBrowse);
            WriteLine(indent, $@"private static {node.InterfaceFullName} {node.FieldName};");
            WriteLine(indent, $@"{Config.Modifier} static {node.InterfaceFullName} {node.PropertyName} ");
            WriteLine(indent, $@"    => global::System.Threading.LazyInitializer.EnsureInitialized(ref {node.FieldName}, () => new {node.ClassFullName}());");
            WriteLine();
            WriteAttributsForClass(indent);
            WriteLine(indent, $@"private sealed class {node.ClassName} : {Strings.ResourceProviderBase}, {node.InterfaceFullName}");
            WriteLine(indent, $@"{{");
            WriteLine(indent, $@"    public {node.ClassName}() : base(""{Helper.AsLiteral(node.ResourceName)}/"") {{ }}");
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
            WriteLine(indent, $@"}}");
        }

        public void WriteInnerResource(int indent, Node node)
        {
            WriteLine();
            WriteLine(indent, Strings.DebuggerNeverBrowse);
            WriteLine(indent, $@"private {node.InterfaceFullName} {node.FieldName};");
            WriteLine(indent, Strings.ResourcePath(node.ResourceName));
            WriteLine(indent, $@"{node.InterfaceFullName} {node.Parent.InterfaceFullName}.{node.PropertyName} ");
            WriteLine(indent, $@"    => global::System.Threading.LazyInitializer.EnsureInitialized(ref this.{node.FieldName}, () => new {node.ClassFullName}());");
            WriteLine();
            WriteAttributsForClass(indent);
            WriteLine(indent, $@"private sealed class {node.ClassName} : {Strings.ResourceProviderBase}, {node.InterfaceFullName}");
            WriteLine(indent, $@"{{");
            WriteLine(indent, $@"    public {node.ClassName}() : base(""{Helper.AsLiteral(node.ResourceName)}/"") {{ }}");
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
            var pathlit = Helper.AsLiteral(node.ResourceName);
            WriteLine(indent, Strings.ResourcePath(node.ResourceName));
            WriteLine(indent, $@"string {node.Parent.InterfaceFullName}.{node.PropertyName}");
            WriteLine(indent + 1, $@"=> {Strings.LocalizedStrings}.GetValue(""{pathlit}"");");
        }
    }
}
