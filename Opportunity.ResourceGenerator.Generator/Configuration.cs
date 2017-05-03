using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace Opportunity.ResourceGenerator.Generator
{
    public class Configuration
    {
        private Configuration(string projectFilePath)
        {
            this.ProjectPath = Path.GetDirectoryName(projectFilePath);
            var project = XDocument.Load(projectFilePath);
            var ns = project.Descendants(XName.Get("RootNamespace", "http://schemas.microsoft.com/developer/msbuild/2003")).FirstOrDefault();
            var an = project.Descendants(XName.Get("AssemblyName", "http://schemas.microsoft.com/developer/msbuild/2003")).FirstOrDefault();
            this.ProjectDefaultNamespace = ns?.Value ?? "MyProject";
            this.ProjectAssemblyName = an?.Value ?? "MyAssembly";
            this.InterfacesNamespace = null;
            this.LocalizedStringsNamespace = null;
            this.LocalizedStringsClassName = null;
        }

        private string GetProjectDefaultNamespace(string projectFilePath)
        {
            throw new NotImplementedException();
        }

        private string GetProjectAssemblyName(string projectFilePath)
        {
            throw new NotImplementedException();
        }

        private string GetProjectPath(string projectFilePath)
        {
            throw new NotImplementedException();
        }

        private void setIdentity(ref string field, string value, string def, bool allowDots)
        {
            if (!string.IsNullOrWhiteSpace(value))
                field = Helper.Refine(value.Trim(), allowDots);
            else
                field = def;
        }

        public string ProjectPath { get; set; }
        public string ProjectDefaultNamespace { get; set; }
        public string ProjectAssemblyName { get; set; }

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
            if (string.IsNullOrEmpty(ins))
                return $"global::{InterfacesNamespace}.{interfaceName}";
            else if (ins.StartsWith("global::"))
                return $"{ins}.{interfaceName}";
            else
                return $"global::{ins}.{interfaceName}";
        }
        public string InterfaceFullName(string propertyName)
        {
            if (!propertyName.StartsWith(LocalizedStringsFullName + "."))
                throw new Exception();
            propertyName = propertyName.Substring(LocalizedStringsFullName.Length + 1);
            var names = propertyName.Split('.');
            names[names.Length - 1] = "I" + names[names.Length - 1];
            return $"global::{InterfacesNamespace}.{string.Join(".", names)}";
        }

        public string IRPFullName => "global::Opportunity.ResourceGenerator.IResourceProvider";
        public string GRPFullName => "global::Opportunity.ResourceGenerator.GeneratedResourceProvider";
        public string RLFullName => "global::Opportunity.ResourceGenerator.LocalizedStrings";

        public static Configuration Current { get; private set; }

        public static void SetCurrent(string projectFilePath)
        {
            Current = new Configuration(projectFilePath);
        }
    }
}