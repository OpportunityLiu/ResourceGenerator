using System;
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
            if (MemberName.StartsWith("@"))
                this.InterfaceName = $"I{MemberName.Substring(1)}";
            else
                this.InterfaceName = $"I{MemberName}";
            this.ClassName = Helper.Refine(Helper.GetRandomName(ResourceName));
            this.FieldName = Helper.Refine(Helper.GetRandomName(ResourceName));
        }

        public Dictionary<string, Node> Childern { get; } = new Dictionary<string, Node>(StringComparer.OrdinalIgnoreCase);

        public string InterfaceName { get; }

        public virtual string InterfaceNamespace => $"{Parent.InterfaceNamespace}.{Parent.MemberName}";

        public string ClassName { get; }

        public string FieldName { get; }

        public string InterfaceFullName => Configuration.Config.InterfaceFullName(InterfaceName, InterfaceNamespace);

        public virtual string ClassFullName => $"{Parent.ClassFullName}.{ClassName}";
    }
}
