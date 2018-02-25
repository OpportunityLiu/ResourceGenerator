using System.Collections.Generic;

namespace Opportunity.ResourceGenerator.Generator.ResourceProvider
{
    public abstract class Node
    {
        protected Node(BranchNode parent, string name)
        {
            this.Parent = parent;
            this.Name = name;
            if (Configuration.Config.IsFormatStringEnabled && name.StartsWith("$") && this.IsLeaf)
            {
                this.MemberName = Helper.Refine(name.Substring(1));
            }
            else
                this.MemberName = Helper.Refine(name);
            if (parent != null)
            {
                parent.Childern.Add(name, this);
            }
        }

        public BranchNode Parent { get; }

        public RootNode Root => this is RootNode r ? r : Parent.Root;

        public bool IsLeaf => this is LeafNode;
        public bool IsRoot => this is RootNode;

        public string Name { get; }
        public virtual string ResourceName => Helper.CombineResourcePath(Parent?.ResourceName, Name);

        public string MemberName { get; }
        public virtual string MemberFullName => $"{Parent.ClassFullName}.{MemberName}";
    }
}