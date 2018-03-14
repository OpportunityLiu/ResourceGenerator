﻿using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Newtonsoft.Json;
using Opportunity.ResourceGenerator.Generator.Tree;

namespace Opportunity.ResourceGenerator.Generator
{
    public class Configuration
    {
        private Configuration(string csprojPath)
        {
            this.ProjectDirectory = Path.GetDirectoryName(csprojPath);
            var project = XDocument.Load(csprojPath);
            var ns = project.Descendants(XName.Get("RootNamespace", "http://schemas.microsoft.com/developer/msbuild/2003")).FirstOrDefault();
            var an = project.Descendants(XName.Get("AssemblyName", "http://schemas.microsoft.com/developer/msbuild/2003")).FirstOrDefault();
            this.ProjectDefaultNamespace = ns?.Value ?? "MyProject";
            this.ProjectAssemblyName = an?.Value ?? "MyAssembly";
            this.InterfacesNamespace = null;
            this.LocalizedStringsNamespace = null;
            this.LocalizedStringsClassName = null;
        }

        private void setIdentity(ref string field, string value, string def, bool allowDots)
        {
            if (!string.IsNullOrWhiteSpace(value))
                field = Helper.Refine(value.Trim(), allowDots);
            else
                field = def;
        }

        public string[] Exclude { get; set; }
        public string[] Include { get; set; }

        public bool ShouldSkip(Node node)
        {
            if (node == null)
                return true;
            var rName = node.ResourcePath;
            var r = false;
            foreach (var item in Exclude ?? Enumerable.Empty<string>())
            {
                if (item != null && rName.Contains(item))
                {
                    r = true;
                    break;
                }
            }
            if (r)
            {
                foreach (var item in Include ?? Enumerable.Empty<string>())
                {
                    if (item != null && rName.Contains(item))
                    {
                        r = false;
                        break;
                    }
                }
            }
            return r;
        }

        public string ProjectDirectory { get; set; }
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
            Config = r;
        }

        public static Configuration Config { get; private set; }
    }
}