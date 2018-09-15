using Opportunity.ResourceGenerator.Generator.Tree;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Opportunity.ResourceGenerator.Generator.CodeGen
{
    internal class Generator
    {
        private string[] ResourceFiles { get; }
        private List<RootNode> Roots { get; }
        private Configuration Config { get; }
        private System.CodeDom.Compiler.CodeDomProvider CodeDomProvider { get; }

        private List<CodeNamespace> InterfaceNamespaces { get; } = new List<CodeNamespace>();

        private CodeTypeDeclaration LocalizedStringsClass { get; }

        public Generator(Configuration config)
        {
            Config = config;
            CodeDomProvider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider(config.FileType);
            if (Config.SourceLanguagePath.IsNullOrWhiteSpace())
                Config.SourceLanguagePath = Directory.EnumerateDirectories(Path.Combine(Config.ProjectDirectory, Config.ResourcePath)).First();
            var stringsPath = Path.Combine(Config.ProjectDirectory, Config.ResourcePath, Config.SourceLanguagePath);
            ResourceFiles = Directory.Exists(stringsPath) ? Directory.GetFiles(stringsPath) : Array.Empty<string>();
            Roots = Providers.Provider.Analyze(ResourceFiles);
            LocalizedStringsClass = new CodeTypeDeclaration(config.LocalizedStringsClassName)
            {
                TypeAttributes = System.Reflection.TypeAttributes.Sealed,
                Members =
                {
                    new CodeConstructor { Attributes = MemberAttributes.Private }
                }
            };
            AddAttribute(LocalizedStringsClass);
            if (!Config.DebugGeneratedCode)
                LocalizedStringsClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(System.Diagnostics.DebuggerNonUserCodeAttribute), CodeTypeReferenceOptions.GlobalReference)));
            SetAccess(LocalizedStringsClass);
            Generate();
        }

        public void Write(TextWriter writer)
        {
            foreach (var item in ResourceFiles)
            {
                GenHash(item, writer);
            }
            var p = new CodeCompileUnit();
            p.Namespaces.AddRange(InterfaceNamespaces.ToArray());
            p.Namespaces.Add(new CodeNamespace(Helper.FormatNamespace(Config.LocalizedStringsNamespace))
            {
                Types = { LocalizedStringsClass }
            });
            CodeDomProvider.GenerateCodeFromCompileUnit(p, writer, null);
        }

        private void SetAccess(CodeTypeMember member)
        {
            if (member is CodeTypeDeclaration type)
            {
                type.TypeAttributes = type.TypeAttributes & ~System.Reflection.TypeAttributes.VisibilityMask;
                switch (Config.Modifier.ToLowerInvariant())
                {
                case "internal":
                    type.TypeAttributes |= System.Reflection.TypeAttributes.NestedAssembly;
                    break;
                case "public":
                    type.TypeAttributes |= System.Reflection.TypeAttributes.Public;
                    break;
                default:
                    break;
                }
            }
            member.Attributes = member.Attributes & ~MemberAttributes.AccessMask;
            switch (Config.Modifier.ToLowerInvariant())
            {
            case "internal":
                member.Attributes |= MemberAttributes.Assembly;
                break;
            case "public":
                member.Attributes |= MemberAttributes.Public;
                break;
            default:
                break;
            }
        }

        private void Generate()
        {
            var interns = new CodeNamespace(Helper.FormatNamespace(Config.InterfacesNamespace));
            InterfaceNamespaces.Add(interns);
            foreach (var item in Roots)
            {
                GenInterface(item, interns);
                GenClass(item);
                GenClassMember(item);
            }
        }

        private void GenClass(BranchNode node)
        {
            var cl = new CodeTypeDeclaration(node.ClassName)
            {
                IsClass = true,
                Attributes = MemberAttributes.Private,
                TypeAttributes = System.Reflection.TypeAttributes.Sealed | System.Reflection.TypeAttributes.NestedPrivate,
                BaseTypes = { Statics.ResourceProviderBase, node.InterfaceRef }
            };

            var constuctor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public,
                BaseConstructorArgs = { new CodePrimitiveExpression(node.ResourcePath + "/") },
            };
            cl.Members.Add(constuctor);

            AddAttribute(cl);
            LocalizedStringsClass.Members.Add(cl);

            var field = new CodeMemberField
            {
                Name = node.FieldName,
                Attributes = MemberAttributes.Static | MemberAttributes.Private,
                Type = node.ClassRef,
            };
            LocalizedStringsClass.Members.Add(field);

            foreach (var item in node.Childern)
            {
                if (item is BranchNode b)
                {
                    GenClass(b);
                    GenClassMember(b, cl);
                }
                else if (item is LeafNode l)
                    GenClassMember(l, cl);
            }
        }

        private void GenClassMember(RootNode node)
        {
            var member = new CodeMemberProperty
            {
                Name = node.MemberName,
                Attributes = MemberAttributes.Static | MemberAttributes.Public,
                HasGet = true,
                HasSet = false,
                Type = node.InterfaceRef,
                GetStatements =
                {
                    Statics.LazyLoad(node.ClassRef, node.FieldRef),
                }
            };
            SetAccess(member);
            LocalizedStringsClass.Members.Add(member);
        }

        private void GenClassMember(BranchNode node, CodeTypeDeclaration outerClass)
        {
            var member = new CodeMemberProperty
            {
                Name = node.MemberName,
                PrivateImplementationType = node.Parent.InterfaceRef,
                HasGet = true,
                HasSet = false,
                Type = node.InterfaceRef,
                CustomAttributes = { Statics.ResourcePath(node.ResourcePath) },
                GetStatements =
                {
                    Statics.LazyLoad(node.ClassRef, node.FieldRef),
                }
            };
            SetAccess(member);
            outerClass.Members.Add(member);
        }

        private void GenClassMember(LeafNode node, CodeTypeDeclaration outerClass)
        {
            if (node.FormatStringValue is null)
            {
                var member = new CodeMemberProperty
                {
                    Name = node.MemberName,
                    PrivateImplementationType = node.Parent.InterfaceRef,
                    HasGet = true,
                    HasSet = false,
                    Type = Statics.String,
                    CustomAttributes = { Statics.ResourcePath(node.ResourcePath) },
                    GetStatements = { Returns(Statics.GetResourceString(node.ResourcePath)) },
                };
                SetAccess(member);
                outerClass.Members.Add(member);
            }
            else
            {
                var field = new CodeMemberField
                {
                    Name = Helper.GetRandomName(node.MemberName),
                    Attributes = MemberAttributes.Static | MemberAttributes.Private,
                    Type = Statics.FormattableResource,
                    InitExpression = new CodeObjectCreateExpression(Statics.FormattableResource, Statics.GetResourceString(node.ResourcePath)),
                };
                outerClass.Members.Add(field);
                var fieldref = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(node.Parent.ClassRef), field.Name);


                var paramNames = node.FormatStringValue.Arguments.Select(a => Helper.Refine(a.Name)).ToArray();
                var param = paramNames.Select(a => new CodeParameterDeclarationExpression(Statics.Object, a)).ToArray();
                var args = node.FormatStringValue.Arguments.OrderBy(a => a.Index).Select(a => new CodeArgumentReferenceExpression(Helper.Refine(a.Name))).ToArray();
                if (node.FormatStringValue.Arguments.Count != 0)
                {
                    var getmethod = new CodeMemberMethod
                    {
                        Name = node.MemberName,
                        PrivateImplementationType = node.Parent.InterfaceRef,
                        CustomAttributes = { Statics.ResourcePath(node.ResourcePath) },
                        ReturnType = Statics.FormattableResource,
                        Statements = { Returns(fieldref) },
                    };
                    outerClass.Members.Add(getmethod);
                }

                var formatstrref = new CodePropertyReferenceExpression(fieldref, nameof(FormattableResourceString.FormatString));

                var formatmethod = new CodeMemberMethod
                {
                    Name = node.MemberName,
                    PrivateImplementationType = node.Parent.InterfaceRef,
                    ReturnType = Statics.String,
                    Statements = { Returns(Config.FormatString(formatstrref, null, args)) },
                };
                formatmethod.Parameters.AddRange(param);
                if (node.FormatStringValue.Arguments.Count == 0)
                    formatmethod.CustomAttributes.Add(Statics.ResourcePath(node.ResourcePath));
                outerClass.Members.Add(formatmethod);

                var provider = Helper.GetUnusedName(paramNames, Strings.ProviderNames);
                if (provider != null)
                {
                    var formatmethodfull = new CodeMemberMethod
                    {
                        Name = node.MemberName,
                        PrivateImplementationType = node.Parent.InterfaceRef,
                        ReturnType = Statics.String,
                        Parameters = { new CodeParameterDeclarationExpression(Statics.IFormatProvider, provider) },
                        Statements = { Returns(Config.FormatString(formatstrref, new CodeArgumentReferenceExpression(provider), args)) },
                    };
                    formatmethodfull.Parameters.AddRange(param);
                    outerClass.Members.Add(formatmethodfull);
                }
            }
        }

        private static void AddAttribute(CodeTypeMember member)
        {
            member.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(typeof(System.CodeDom.Compiler.GeneratedCodeAttribute), CodeTypeReferenceOptions.GlobalReference),
                    new CodeAttributeArgument(new CodePrimitiveExpression(Helper.ProductName)),
                    new CodeAttributeArgument(new CodePrimitiveExpression(Helper.ProductVersion))
                )
            );
        }

        private void GenInterface(BranchNode node, CodeNamespace parentInterfaceNamespaceContainer)
        {
            var interf = new CodeTypeDeclaration(node.InterfaceName)
            {
                IsInterface = true,
                BaseTypes = { Statics.IResourceProvider },
            };
            SetAccess(interf);
            AddAttribute(interf);
            AddMemberComment(interf, $"Path: {node.ResourcePath}");
            parentInterfaceNamespaceContainer.Types.Add(interf);

            var myContainer = default(CodeNamespace);
            foreach (var item in node.Childern)
            {
                if (item is BranchNode b)
                {
                    if (myContainer is null)
                    {
                        myContainer = new CodeNamespace(Helper.FormatNamespace($"{node.InterfaceNamespace}.{node.MemberName}"));
                        InterfaceNamespaces.Add(myContainer);
                    }
                    GenInterfaceMember(b, interf);
                    GenInterface(b, myContainer);
                }
                else if (item is LeafNode l)
                    GenInterfaceMember(l, interf);
            }
        }

        private void GenInterfaceMember(LeafNode node, CodeTypeDeclaration parentInterfaceContainer)
        {
            var summary = $"Path: {node.ResourcePath}\nValue:\n{node.Value}";
            if (node.FormatStringValue is null)
            {
                var member = new CodeMemberProperty
                {
                    Name = node.MemberName,
                    Type = Statics.String,
                    HasGet = true,
                    HasSet = false,
                };
                parentInterfaceContainer.Members.Add(member);
                AddMemberComment(member, summary);
            }
            else
            {
                var paramNames = node.FormatStringValue.Arguments.Select(a => Helper.Refine(a.Name)).ToArray();
                var param = paramNames.Select(a => new CodeParameterDeclarationExpression(Statics.Object, a)).ToArray();
                if (node.FormatStringValue.Arguments.Count != 0)
                {
                    var getmethod = new CodeMemberMethod
                    {
                        Name = node.MemberName,
                        ReturnType = Statics.FormattableResource,
                    };
                    AddMemberComment(getmethod, summary);
                    parentInterfaceContainer.Members.Add(getmethod);
                }
                var formatmethod = new CodeMemberMethod
                {
                    Name = node.MemberName,
                    ReturnType = Statics.String,
                };
                formatmethod.Parameters.AddRange(param);
                AddMemberComment(formatmethod, summary);
                parentInterfaceContainer.Members.Add(formatmethod);

                var provider = Helper.GetUnusedName(paramNames, Strings.ProviderNames);
                if (provider != null)
                {
                    var formatmethodfull = new CodeMemberMethod
                    {
                        Name = node.MemberName,
                        ReturnType = Statics.String,
                        Parameters = { new CodeParameterDeclarationExpression(Statics.IFormatProvider, provider) }
                    };
                    formatmethodfull.Parameters.AddRange(param);
                    AddMemberComment(formatmethodfull, summary);
                    parentInterfaceContainer.Members.Add(formatmethodfull);
                }
            }
        }

        private void GenInterfaceMember(BranchNode node, CodeTypeDeclaration parentInterfaceContainer)
        {
            var member = new CodeMemberProperty
            {
                Name = node.MemberName,
                Type = node.InterfaceRef,
                HasGet = true,
                HasSet = false,
            };
            parentInterfaceContainer.Members.Add(member);
            AddMemberComment(member, $"Path: {node.ResourcePath}");
        }

        private static void AddMemberComment(CodeTypeMember member, string summary)
        {
            var lines = from line in summary.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                        select $"<para>{HttpUtility.HtmlEncode(line)}</para>";
            member.Comments.Add(new CodeCommentStatement($"<summary>{string.Concat(lines)}</summary>", true));
        }

        private static readonly System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        private void GenHash(string file, TextWriter writer)
        {
            var hashdic = "";
            using (var stream = new FileStream(file, FileMode.Open))
            {
                var hash = md5.ComputeHash(stream);
                var hashstr = string.Concat(BitConverter.ToString(hash).Split('-'));
                if (Config.FileType == "vb")
                    hashdic = $@"#ExternalChecksum (""{file}"", ""{{406ea660-64cf-4c82-b6f0-42d48172a799}}"", ""{hashstr}"")";
                else
                    hashdic = $@"#pragma checksum ""{file}"" ""{{406ea660-64cf-4c82-b6f0-42d48172a799}}"" ""{hashstr}""";
            }
            CodeDomProvider.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit(hashdic), writer, null);
        }

        private static CodeMethodReturnStatement Returns(CodeExpression expression)
        {
            return new CodeMethodReturnStatement(expression);
        }
    }
}
