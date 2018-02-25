using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator.Generator.ResourceProvider
{
    public class LeafNode : Node
    {
        public LeafNode(BranchNode parent, string name, string value)
            : base(parent, name)
        {
            this.Value = value;
        }

        public string Value { get; }
    }
}
