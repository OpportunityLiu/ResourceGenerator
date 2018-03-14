using Opportunity.ResourceGenerator.Generator.Providers;
using Opportunity.ResourceGenerator.Generator.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Opportunity.ResourceGenerator.Generator.Configuration;

namespace Opportunity.ResourceGenerator.Generator
{
    class ResourceWriter : FileWriter
    {
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
            if (!Helper.Keywords.Contains(node.ResourceName) && node.ResourceName != node.MemberName)
            {
                if (Config.IsFormatStringEnabled && node.ResourceName.StartsWith("$"))
                    return;
                WriteLine($"#warning Resource has been renamed. ResourceName: \"{Helper.AsLiteral(node.ResourceName)}\", PropertyName: \"{Helper.AsLiteral(node.MemberName)}\"");
            }
        }

        public void WriteAttributesForInterface(int indent)
        {
            WriteLine(indent, Strings.GeneratedCode);
        }

        public void WriteAttributesForClass(int indent)
        {
            WriteLine(indent, Strings.DebuggerTypeProxy);
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
                if (Config.ShouldSkip(item))
                    continue;
                WriteInterface(0, item);
            }
        }

        public void WriteInterface(int indent, BranchNode node)
        {
            WriteLine();
            WriteLine(indent, $"namespace {node.InterfaceNamespace}");
            WriteLine(indent, $"{{");
            WriteAttributesForInterface(indent + 1);
            WriteLine(indent, $"    {Config.Modifier} interface {node.InterfaceName} : {Strings.IResourceProvider}");
            WriteLine(indent, $"    {{");
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is LeafNode lf)
                    WriteInterfaceProperty(indent + 2, lf);
                else if (item is BranchNode br)
                    WriteInterfaceProperty(indent + 2, br);
            }
            WriteLine(indent, $"    }}");
            WriteLine(indent, $"}}");
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is BranchNode br)
                    WriteInterface(indent, br);
            }
        }

        public void WriteInterfaceProperty(int indent, LeafNode node)
        {
            Check(node);
            var modifier = Helper.IPropModifier(node.MemberName);
            if (Config.IsFormatStringEnabled && node.ResourceName.StartsWith("$"))
            {
                var format = new FormattableResourceString(node.Value);
                var paramNames = format.Arguments.Select(a => Helper.Refine(a.Name)).ToList();
                var param = string.Join(", ", paramNames.Select(a => "object " + a));
                if (format.Arguments.Count != 0)
                {
                    WriteComment(indent, node.Value);
                    WriteLine(indent, $@"{modifier}{Strings.FormattableResource} {node.MemberName}();");
                }
                WriteComment(indent, node.Value);
                WriteLine(indent, $@"{modifier}string {node.MemberName}({param});");

                var provider = Helper.GetUnusedName(paramNames, Strings.ProviderNames);
                if (provider != null)
                {
                    WriteComment(indent, node.Value);
                    if (format.Arguments.Count != 0)
                        WriteLine(indent, $@"{modifier}string {node.MemberName}({Strings.IFormatProvider} {provider}, {param});");
                    else
                        WriteLine(indent, $@"{modifier}string {node.MemberName}({Strings.IFormatProvider} {provider});");
                }
            }
            else
            {
                WriteComment(indent, node.Value);
                this.WriteLine(indent, $@"{modifier}string {node.MemberName} {{ get; }}");
            }
        }

        public void WriteInterfaceProperty(int indent, BranchNode node)
        {
            Check(node);
            WriteLine(indent, $@"{Helper.IPropModifier(node.MemberName)}{node.InterfaceFullName} {node.MemberName} {{ get; }}");
        }

        public void WriteResources(IList<RootNode> tree)
        {
            var indent = 1;
            WriteLine($"namespace {Config.LocalizedStringsNamespace}");
            WriteLine($"{{");
            WriteAttributesForClass(indent);
            WriteLine(indent, $"{Config.Modifier} static class {Config.LocalizedStringsClassName}");
            WriteLine(indent, $"{{");
            foreach (var item in tree)
            {
                if (Config.ShouldSkip(item))
                    continue;
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
            WriteLine(indent, $@"{Config.Modifier} static {node.InterfaceFullName} {node.MemberName} ");
            WriteLine(indent, $@"    => global::System.Threading.LazyInitializer.EnsureInitialized(ref {node.FieldName}, () => new {node.ClassFullName}());");
            WriteLine();
            WriteAttributesForClass(indent);
            WriteLine(indent, $@"private sealed class {node.ClassName} : {Strings.ResourceProviderBase}, {node.InterfaceFullName}");
            WriteLine(indent, $@"{{");
            WriteLine(indent, $@"    public {node.ClassName}() : base(""{Helper.AsLiteral(node.ResourcePath)}/"") {{ }}");
            WriteLine();
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is BranchNode br)
                    WriteInnerResource(indent + 1, br);
            }
            WriteLine();
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is LeafNode lf)
                    WriteProperty(indent + 1, lf);
            }
            WriteLine(indent, $@"}}");
        }

        public void WriteInnerResource(int indent, BranchNode node)
        {
            WriteLine();
            WriteLine(indent, Strings.DebuggerNeverBrowse);
            WriteLine(indent, $@"private {node.InterfaceFullName} {node.FieldName};");
            WriteLine(indent, Strings.ResourcePath(node.ResourcePath));
            WriteLine(indent, $@"{node.InterfaceFullName} {node.Parent.InterfaceFullName}.{node.MemberName} ");
            WriteLine(indent, $@"    => global::System.Threading.LazyInitializer.EnsureInitialized(ref this.{node.FieldName}, () => new {node.ClassFullName}());");
            WriteLine();
            WriteAttributesForClass(indent);
            WriteLine(indent, $@"private sealed class {node.ClassName} : {Strings.ResourceProviderBase}, {node.InterfaceFullName}");
            WriteLine(indent, $@"{{");
            WriteLine(indent, $@"    public {node.ClassName}() : base(""{Helper.AsLiteral(node.ResourcePath)}/"") {{ }}");
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is BranchNode br)
                    WriteInnerResource(indent + 1, br);
            }
            WriteLine();
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is LeafNode lf)
                    WriteProperty(indent + 1, lf);
            }
            WriteLine(indent, $@"}}");
        }

        public void WriteProperty(int indent, LeafNode node)
        {
            var pathlit = Helper.AsLiteral(node.ResourcePath);
            if (Config.IsFormatStringEnabled && node.ResourceName.StartsWith("$"))
            {
                var tempFormatStringFieldName = Helper.GetRandomName(node.MemberName);
                this.WriteLine(indent, $@"{Strings.FormattableResource} {tempFormatStringFieldName};");
                var format = new FormattableResourceString(node.Value);
                var paramNames = format.Arguments.Select(a => Helper.Refine(a.Name)).ToList();
                var param = string.Join(", ", paramNames.Select(a => "object " + a));
                var args = string.Join(", ", format.Arguments.OrderBy(a => a.Index).Select(a => Helper.Refine(a.Name)));

                if (format.Arguments.Count != 0)
                {
                    WriteLine(indent, Strings.ResourcePath(node.ResourcePath));
                    WriteLine(indent, $@"{Strings.FormattableResource} {node.Parent.InterfaceFullName}.{node.MemberName}()");
                    WriteLine(indent, $@"{{");
                    WriteLine(indent, $@"    if ({tempFormatStringFieldName} == null)");
                    WriteLine(indent, $@"        {tempFormatStringFieldName} = new {Strings.FormattableResource}({Strings.LocalizedStrings}.GetValue(""{pathlit}""));");
                    WriteLine(indent, $@"    return {tempFormatStringFieldName};");
                    WriteLine(indent, $@"}}");
                }

                if (format.Arguments.Count == 0)
                    WriteLine(indent, Strings.ResourcePath(node.ResourcePath));
                WriteLine(indent, $@"string {node.Parent.InterfaceFullName}.{node.MemberName}({param})");
                WriteLine(indent, $@"{{");
                WriteLine(indent, $@"    if ({tempFormatStringFieldName} == null)");
                WriteLine(indent, $@"        {tempFormatStringFieldName} = new {Strings.FormattableResource}({Strings.LocalizedStrings}.GetValue(""{pathlit}""));");
                if (format.Arguments.Count == 0)
                    WriteLine(indent, $@"    return string.Format({tempFormatStringFieldName}.FormatString);");
                else
                    WriteLine(indent, $@"    return string.Format({tempFormatStringFieldName}.FormatString, {args});");
                WriteLine(indent, $@"}}");

                var provider = Helper.GetUnusedName(paramNames, Strings.ProviderNames);
                if (provider != null)
                {
                    if (format.Arguments.Count == 0)
                        WriteLine(indent, $@"string {node.Parent.InterfaceFullName}.{node.MemberName}({Strings.IFormatProvider} {provider})");
                    else
                        WriteLine(indent, $@"string {node.Parent.InterfaceFullName}.{node.MemberName}({Strings.IFormatProvider} {provider}, {param})");
                    WriteLine(indent, $@"{{");
                    WriteLine(indent, $@"    if ({tempFormatStringFieldName} == null)");
                    WriteLine(indent, $@"        {tempFormatStringFieldName} = new {Strings.FormattableResource}({Strings.LocalizedStrings}.GetValue(""{pathlit}""));");
                    if (format.Arguments.Count == 0)
                        WriteLine(indent, $@"    return string.Format({provider}, {tempFormatStringFieldName}.FormatString);");
                    else
                        WriteLine(indent, $@"    return string.Format({provider}, {tempFormatStringFieldName}.FormatString, {args});");
                    WriteLine(indent, $@"}}");
                }
            }
            else
            {
                WriteLine(indent, Strings.ResourcePath(node.ResourcePath));
                WriteLine(indent, $@"string {node.Parent.InterfaceFullName}.{node.MemberName}");
                WriteLine(indent + 1, $@"=> {Strings.LocalizedStrings}.GetValue(""{pathlit}"");");
            }
        }
    }
}
