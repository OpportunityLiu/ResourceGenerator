using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator.Generator
{
    public static class Strings
    {
        public static string DebuggerNeverBrowse
            => "[global::System.Diagnostics.DebuggerBrowsableAttribute(global::System.Diagnostics.DebuggerBrowsableState.Never)]";
        public static string DebuggerNonUserCode => "[global::System.Diagnostics.DebuggerNonUserCodeAttribute]";
        public static string GeneratedCode
            => $@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{Helper.ProductName}"", ""{Helper.ProductVersion}"")]";
        public static string ResourceProviderBase => "global::Opportunity.ResourceGenerator.ResourceProviderBase";

        public static string IResourceProvider => "global::Opportunity.ResourceGenerator.IResourceProvider";
        public static string GeneratedResourceProvider => "global::Opportunity.ResourceGenerator.GeneratedResourceProvider";
        public static string LocalizedStrings => "global::Opportunity.Helpers.Universal.LocalizedStrings";

        public static string DebuggerDisplay(string value)
            => $@"[global::System.Diagnostics.DebuggerDisplayAttribute(""{Helper.AsLiteral(value)}"")]";

        public static string ResourcePath(string value)
            => $@"[global::Opportunity.ResourceGenerator.ResourcePathAttribute(""{Helper.AsLiteral(value)}"")]";
    }
}
