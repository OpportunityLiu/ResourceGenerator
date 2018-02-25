using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator.Generator.ResourceProvider
{
    public class BranchNode : Node
    {
        public BranchNode(BranchNode parent, string name)
            : base(parent, name)
        {
            if (MemberName.StartsWith("@"))
                this.InterfaceName = $"I{Name}";
            else
                this.InterfaceName = $"I{MemberName}";
            this.ClassName = Helper.Refine(Helper.GetRandomName(Name));
            this.FieldName = Helper.Refine(Helper.GetRandomName(Name));
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
