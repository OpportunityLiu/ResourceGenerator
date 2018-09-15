using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator.Generator
{
    public static class Strings
    {
        public static string IFormatProvider => "global::System.IFormatProvider";
        public static string[] ProviderNames = new string[] { "provider", "formatProvider", "format_Provider", "format_provider", "_provider", "_formatProvider", "_Provider", "_FormatProvider", "__provider", "__formatProvider", "__Provider", "__FormatProvider" };
    }

    public static class Statics
    {
        public static CodeTypeReference ResourceProviderBase { get; } =
            new CodeTypeReference("Opportunity.ResourceGenerator.ResourceProviderBase", CodeTypeReferenceOptions.GlobalReference);
        public static CodeTypeReference IResourceProvider { get; } =
            new CodeTypeReference("Opportunity.ResourceGenerator.IResourceProvider", CodeTypeReferenceOptions.GlobalReference);
        public static CodeTypeReference GeneratedResourceProvider { get; } =
            new CodeTypeReference("Opportunity.ResourceGenerator.GeneratedResourceProvider", CodeTypeReferenceOptions.GlobalReference);
        public static CodeTypeReference LocalizedStrings { get; } =
            new CodeTypeReference("Opportunity.Helpers.Universal.LocalizedStrings", CodeTypeReferenceOptions.GlobalReference);
        public static CodeTypeReference FormattableResource { get; } =
            new CodeTypeReference(typeof(FormattableResourceString), CodeTypeReferenceOptions.GlobalReference);

        public static CodeTypeReference Object { get; } =
            new CodeTypeReference(typeof(object), CodeTypeReferenceOptions.GlobalReference);
        public static CodeTypeReference String { get; } =
            new CodeTypeReference(typeof(string), CodeTypeReferenceOptions.GlobalReference);
        public static CodeTypeReference IFormatProvider { get; } =
            new CodeTypeReference(typeof(IFormatProvider), CodeTypeReferenceOptions.GlobalReference);

        public static CodeAttributeDeclaration ResourcePath(string value)
        {
            return new CodeAttributeDeclaration(new CodeTypeReference("Opportunity.ResourceGenerator.ResourcePathAttribute", CodeTypeReferenceOptions.GlobalReference), new CodeAttributeArgument(new CodePrimitiveExpression(value)));
        }

        public static CodeMethodReturnStatement LazyLoad(CodeTypeReference type, CodeFieldReferenceExpression field)
        {
            return new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                    new CodeTypeReferenceExpression(typeof(System.Threading.LazyInitializer)),
                    nameof(System.Threading.LazyInitializer.EnsureInitialized),
                    type),
                new CodeDirectionExpression(FieldDirection.Ref, field)));
        }

        public static CodeMethodInvokeExpression GetResourceString(string resourcePath)
        {
            return new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(LocalizedStrings), "GetValue"),
                new CodePrimitiveExpression(resourcePath));
        }
    }
}
