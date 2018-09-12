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
    internal class ResourceWriter : FileWriter
    {
        public ResourceWriter(string path) : base(path)
        {
        }

        public void Execute()
        {
            if (Config.SourceLanguagePath == null)
                Config.SourceLanguagePath = Directory.EnumerateDirectories(Path.Combine(Config.ProjectDirectory, Config.ResourcePath)).First();
            var stringsPath = Path.Combine(Config.ProjectDirectory, Config.ResourcePath, Config.SourceLanguagePath);
            var files = Directory.Exists(stringsPath) ? Directory.GetFiles(stringsPath) : Array.Empty<string>();
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

        public void WriteAttributesForInterface()
        {
            WriteLine(Strings.GeneratedCode);
        }

        public void WriteAttributesForClass()
        {
            if (!Config.DebugGeneratedCode)
                WriteLine(Strings.DebuggerNonUserCode);
            WriteLine(Strings.GeneratedCode);
        }

        public void WriteComment(string summary)
        {
            var lines = from line in summary.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                        select $"/// <para>{HttpUtility.HtmlEncode(line)}";
            WriteBlock($@"
/// <summary>
{string.Join("\n", lines)}
/// </summary>
");
        }

        public void WriteInterfaces(IList<RootNode> tree)
        {
            foreach (var item in tree)
            {
                if (Config.ShouldSkip(item))
                    continue;
                WriteInterface(item);
            }
        }

        public void WriteInterface(BranchNode node)
        {
            WriteBlock($@"

namespace {node.InterfaceNamespace}
{{
");
            Indent++;
            WriteAttributesForInterface();
            WriteBlock($@"
{Config.Modifier} interface {node.InterfaceName} : {Strings.IResourceProvider}
{{
");
            Indent++;
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is LeafNode lf)
                    WriteInterfaceProperty(lf);
                else if (item is BranchNode br)
                    WriteInterfaceProperty(br);
            }
            Indent--;
            WriteLine($"}}");
            Indent--;
            WriteLine($"}}");
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is BranchNode br)
                    WriteInterface(br);
            }
        }

        public void WriteInterfaceProperty(LeafNode node)
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
                    WriteComment(node.Value);
                    WriteLine($@"{modifier}{Strings.FormattableResource} {node.MemberName}();");
                }
                WriteComment(node.Value);
                WriteLine($@"{modifier}string {node.MemberName}({param});");

                var provider = Helper.GetUnusedName(paramNames, Strings.ProviderNames);
                if (provider != null)
                {
                    WriteComment(node.Value);
                    if (format.Arguments.Count != 0)
                        WriteLine($@"{modifier}string {node.MemberName}({Strings.IFormatProvider} {provider}, {param});");
                    else
                        WriteLine($@"{modifier}string {node.MemberName}({Strings.IFormatProvider} {provider});");
                }
            }
            else
            {
                WriteComment(node.Value);
                WriteLine($@"{modifier}string {node.MemberName} {{ get; }}");
            }
        }

        public void WriteInterfaceProperty(BranchNode node)
        {
            Check(node);
            WriteLine($@"{Helper.IPropModifier(node.MemberName)}{node.InterfaceFullName} {node.MemberName} {{ get; }}");
        }

        public void WriteResources(IList<RootNode> tree)
        {
            WriteLine($"namespace {Config.LocalizedStringsNamespace}");
            WriteLine($"{{");
            Indent++;
            WriteAttributesForClass();
            WriteBlock($@"
{Config.Modifier} static class {Config.LocalizedStringsClassName}
{{
    private static T {Config.InitializerName}<T>(ref T value) where T : class, new()
    {{
        if (value == null)
            value = new T();
        return value;
    }}
");
            Indent++;
            foreach (var item in tree)
            {
                if (Config.ShouldSkip(item))
                    continue;
                WriteRootResource(item);
            }
            Indent--;
            WriteLine("}");
            Indent--;
            WriteLine("}");
        }

        public void WriteRootResource(RootNode node)
        {
            WriteBlock($@"

{Strings.DebuggerNeverBrowse}
private static {node.ClassFullName} {node.FieldName};
{Config.Modifier} static {node.InterfaceFullName} {node.MemberName}
    => {Config.InitializerFullName}(ref {node.FieldName});

");
            WriteAttributesForClass();
            WriteBlock($@"
private sealed class {node.ClassName} : {Strings.ResourceProviderBase}, {node.InterfaceFullName}
{{
    public {node.ClassName}() : base(""{Helper.AsLiteral(node.ResourcePath)}/"") {{ }}

");
            Indent++;
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is BranchNode br)
                    WriteInnerResource(br);
            }
            WriteLine();
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is LeafNode lf)
                    WriteProperty(lf);
            }
            Indent--;
            WriteLine($@"}}");
        }

        public void WriteInnerResource(BranchNode node)
        {
            WriteBlock($@"

{Strings.DebuggerNeverBrowse}
private static {node.ClassFullName} {node.FieldName};
{Strings.ResourcePath(node.ResourcePath)}
{node.InterfaceFullName} {node.Parent.InterfaceFullName}.{node.MemberName}
    => {Config.InitializerFullName}(ref {node.FieldName});

");
            WriteAttributesForClass();
            WriteBlock($@"
private sealed class {node.ClassName} : {Strings.ResourceProviderBase}, {node.InterfaceFullName}
{{
    public {node.ClassName}() : base(""{Helper.AsLiteral(node.ResourcePath)}/"") {{ }}
");
            Indent++;
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is BranchNode br)
                    WriteInnerResource(br);
            }
            WriteLine();
            foreach (var item in node.Childern.Values)
            {
                if (Config.ShouldSkip(item))
                    continue;
                if (item is LeafNode lf)
                    WriteProperty(lf);
            }
            Indent--;
            WriteLine($@"}}");
        }

        public void WriteProperty(LeafNode node)
        {
            var pathlit = Helper.AsLiteral(node.ResourcePath);
            if (Config.IsFormatStringEnabled && node.ResourceName.StartsWith("$"))
            {
                var tempFormatStringFieldName = Helper.GetRandomName(node.MemberName);
                this.WriteLine($@"static {Strings.FormattableResource} {tempFormatStringFieldName};");
                var format = new FormattableResourceString(node.Value);
                var paramNames = format.Arguments.Select(a => Helper.Refine(a.Name)).ToList();
                var param = string.Join(", ", paramNames.Select(a => "object " + a));
                var args = string.Join(", ", format.Arguments.OrderBy(a => a.Index).Select(a => Helper.Refine(a.Name)));
                if (format.Arguments.Count != 0)
                    args = ", " + args;

                if (format.Arguments.Count != 0)
                {
                    WriteBlock($@"
{Strings.ResourcePath(node.ResourcePath)}
{Strings.FormattableResource} {node.Parent.InterfaceFullName}.{node.MemberName}()
{{
    if ({tempFormatStringFieldName} == null)
        {tempFormatStringFieldName} = new {Strings.FormattableResource}({Strings.LocalizedStrings}.GetValue(""{pathlit}""));
    return {tempFormatStringFieldName};
}}
");
                }

                if (format.Arguments.Count == 0)
                    WriteLine(Strings.ResourcePath(node.ResourcePath));
                WriteBlock($@"
string {node.Parent.InterfaceFullName}.{node.MemberName}({param})
{{
    if ({tempFormatStringFieldName} == null)
        {tempFormatStringFieldName} = new {Strings.FormattableResource}({Strings.LocalizedStrings}.GetValue(""{pathlit}""));
    return {Config.FormatStringFunction}({tempFormatStringFieldName}.FormatString{args});
}}
");

                var provider = Helper.GetUnusedName(paramNames, Strings.ProviderNames);
                if (provider != null)
                {
                    var myparam = $"{Strings.IFormatProvider} {provider}";
                    if (format.Arguments.Count != 0)
                        myparam += ", " + param;
                    WriteBlock($@"
string {node.Parent.InterfaceFullName}.{node.MemberName}({myparam})
{{
    if ({tempFormatStringFieldName} == null)
        {tempFormatStringFieldName} = new {Strings.FormattableResource}({Strings.LocalizedStrings}.GetValue(""{pathlit}""));
    return {Config.FormatStringFunction}({provider}, {tempFormatStringFieldName}.FormatString{args});
}}
");
                }
            }
            else
            {
                WriteBlock($@"
{Strings.ResourcePath(node.ResourcePath)}
string {node.Parent.InterfaceFullName}.{node.MemberName}
    => {Strings.LocalizedStrings}.GetValue(""{pathlit}"");
");
            }
        }
    }
}
