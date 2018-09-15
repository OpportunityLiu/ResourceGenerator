using Newtonsoft.Json;
using Opportunity.ResourceGenerator.Generator.Tree;
using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Opportunity.ResourceGenerator.Generator
{
    public class Configuration
    {
        private Configuration(string projPath)
        {
            this.ProjectPath = Path.GetFullPath(projPath);
            this.ProjectDirectory = Path.GetDirectoryName(this.ProjectPath);
            var project = XDocument.Load(this.ProjectPath);
            var ns = project.Descendants(XName.Get("RootNamespace", "http://schemas.microsoft.com/developer/msbuild/2003")).FirstOrDefault();
            var an = project.Descendants(XName.Get("AssemblyName", "http://schemas.microsoft.com/developer/msbuild/2003")).FirstOrDefault();
            this.ProjectDefaultNamespace = ns?.Value ?? Path.GetFileNameWithoutExtension(this.ProjectPath);
            this.ProjectAssemblyName = an?.Value ?? Path.GetFileNameWithoutExtension(this.ProjectPath);
            this.InterfacesNamespace = null;
            this.LocalizedStringsNamespace = null;
            this.LocalizedStringsClassName = null;
        }

        private void init()
        {
            if (SourceLanguagePath.IsNullOrEmpty())
            {
                try
                {
                    SourceLanguagePath = Directory.EnumerateDirectories(Path.Combine(ProjectDirectory, ResourcePath)).First();
                }
                catch (Exception) { }
            }
            FormatStringFunction = FormatStringFunction.CoalesceNullOrWhiteSpace("string.Format").Trim();

            {
                var projext = Path.GetExtension(ProjectPath);
                if (projext.StartsWith(".vb"))
                    FileType = "vb";
                else if (projext.StartsWith(".cs"))
                    FileType = "cs";
            }
        }

        public string InitializerName { get; } = Helper.GetRandomName("InitializeResourceClass");
        public string InitializerFullName => $"{LocalizedStringsFullName}.{InitializerName}";

        public string FileType { get; private set; }

        private void setIdentity(ref string field, string value, string def, bool allowDots)
        {
            if (!string.IsNullOrWhiteSpace(value))
                field = Helper.Refine(value.Trim(), allowDots);
            else
                field = def;
        }

        public SelectRule[] Exclude { get; set; }
        public SelectRule[] Include { get; set; }

        public bool ShouldSkip(Node node)
        {
            if (node == null)
                return true;
            var skip = false;
            foreach (var item in Exclude ?? Array.Empty<SelectRule>())
            {
                if (item == null)
                    continue;
                if (item.Validate(node))
                {
                    skip = true;
                    break;
                }
            }
            if (!skip)
                return false;
            foreach (var item in Include ?? Array.Empty<SelectRule>())
            {
                if (item == null)
                    continue;
                if (item.Validate(node))
                {
                    skip = false;
                    break;
                }
            }
            return skip;
        }

        public string ProjectDirectory { get; }
        public string ProjectPath { get; }
        public string ProjectDefaultNamespace { get; }
        public string ProjectAssemblyName { get; }

        public bool DebugGeneratedCode { get; set; }
        private string rpath = "Strings";
        public string ResourcePath
        {
            get => rpath;
            set => rpath = value.Trim().Trim("\\/".ToCharArray());
        }
        public string SourceLanguagePath { get; set; }
        public bool IsDefaultProject { get; set; } = true;

        private string rns;
        private string rc;
        private string ins;
        private string mf = "internal";
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
            set => setIdentity(ref this.ins, value, $"{ProjectDefaultNamespace}.ResourceInfo", true);
        }
        public string Modifier
        {
            get => this.mf; set
            {
                value = (value ?? "").Trim();
                this.mf = string.IsNullOrEmpty(value) ? "internal" : value;
            }
        }

        public bool IsFormatStringEnabled { get; set; }

        public string FormatStringFunction { get; set; }

        public CodeMethodInvokeExpression FormatString(CodeExpression format, CodeExpression formatprovider, CodeExpression[] args)
        {
            var method = default(CodeMethodReferenceExpression);
            do
            {
                if (FormatStringFunction == "string.Format")
                {
                    method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(Statics.String), nameof(string.Format));
                    break;
                }
                var doi = FormatStringFunction.LastIndexOf('.');
                if (doi <= 0)
                {
                    method = new CodeMethodReferenceExpression(null, FormatStringFunction);
                    break;
                }

                var type = new CodeTypeReferenceExpression(new CodeTypeReference(FormatStringFunction.Substring(0, doi), CodeTypeReferenceOptions.GlobalReference));
                var methodname = FormatStringFunction.Substring(doi + 1);

                method = new CodeMethodReferenceExpression(type, methodname);
            } while (false);
            var call = new CodeMethodInvokeExpression(method);
            call.Parameters.Add(format);
            if (formatprovider != null)
                call.Parameters.Add(formatprovider);
            call.Parameters.AddRange(args);
            return call;
        }

        public string LocalizedStringsFullName
            => $"{LocalizedStringsNamespace}.{LocalizedStringsClassName}";
        public string InterfaceFullName(string interfaceName, string ins)
        {
            if (string.IsNullOrEmpty(ins))
                return $"{InterfacesNamespace}.{interfaceName}";
            else
                return $"{ins}.{interfaceName}";
        }

        internal static void SetCurrent(string csprojPath, string resgenconfigPath)
        {
            var r = new Configuration(csprojPath);
            if (resgenconfigPath != null)
            {
                var t = File.ReadAllText(resgenconfigPath);
                JsonConvert.PopulateObject(t, r);
                var className = Path.GetFileNameWithoutExtension(resgenconfigPath);
                className = Helper.Refine(className);
                r.LocalizedStringsClassName = className;
            }
            r.init();
            Config = r;
        }

        public static Configuration Config { get; private set; }
    }
}