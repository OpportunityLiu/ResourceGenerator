using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator.Generator.Tree
{
    public class BranchNode : Node
    {
        public BranchNode(BranchNode parent, string name)
            : base(parent, name)
        {
            this.ClassName = Helper.Refine(Helper.GetRandomName(ResourceName));
            this.FieldName = "s_" + Helper.Refine(Helper.GetRandomName(ResourceName));
        }

        public List<Node> Childern { get; } = new List<Node>();
        public HashSet<string> ChildrenResourceNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public string InterfaceName => $"I{MemberName}";
        public CodeTypeReference InterfaceRef => new CodeTypeReference($"{InterfaceNamespace}.{InterfaceName}", CodeTypeReferenceOptions.GlobalReference);
        public virtual string InterfaceNamespace => $"{Parent.InterfaceNamespace}.{Parent.MemberName}";

        public string ClassName { get; }

        public string FieldName { get; }
        public CodeFieldReferenceExpression FieldRef => new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(Configuration.Config.LocalizedStringsFullName, CodeTypeReferenceOptions.GlobalReference)), FieldName);

        public CodeTypeReference ClassRef => new CodeTypeReference($"{Configuration.Config.LocalizedStringsFullName}.{ClassName}", CodeTypeReferenceOptions.GlobalReference);
    }
}
