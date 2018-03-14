using System.Collections.Generic;
using System.Diagnostics;

namespace Opportunity.ResourceGenerator.Generator.Tree
{
    [DebuggerDisplay(@"{ResourceName}")]
    public abstract class Node
    {
        protected Node(BranchNode parent, string resourceName)
        {
            this.Parent = parent;
            this.ResourceName = resourceName;
            if (Configuration.Config.IsFormatStringEnabled)
            {
                if (resourceName.StartsWith("$") && this.IsLeaf)
                    this.MemberName = Helper.Refine(resourceName.Substring(1));
                else
                    this.MemberName = Helper.Refine(resourceName);
            }
            else
                this.MemberName = Helper.Refine(resourceName);
            if (parent != null)
            {
                parent.Childern.Add(resourceName, this);
            }
        }

        public BranchNode Parent { get; }

        public RootNode Root => this is RootNode r ? r : Parent.Root;

        public bool IsLeaf => this is LeafNode;
        public bool IsRoot => this is RootNode;

        public string ResourceName { get; }
        public virtual string ResourcePath => Helper.CombineResourcePath(Parent?.ResourcePath, ResourceName);

        public string MemberName { get; }
        public virtual string MemberFullName => $"{Parent.ClassFullName}.{MemberName}";
    }
}