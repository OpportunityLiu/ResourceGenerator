using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator.Generator.Tree
{
    public class LeafNode : Node
    {
        public LeafNode(BranchNode parent, string name, string value)
            : base(parent, name)
        {
            this.Value = value;
            MemberName = base.MemberName;

            if (Configuration.Config.IsFormatStringEnabled && ResourceName.StartsWith("$"))
            {
                this.MemberName = Helper.Refine(ResourceName.Substring(1));
                this.FormatStringValue = new FormattableResourceString(value);
            }
        }

        public string Value { get; }

        public override string MemberName { get; }

        public FormattableResourceString FormatStringValue { get; }
    }
}
