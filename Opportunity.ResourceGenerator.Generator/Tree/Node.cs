using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;

namespace Opportunity.ResourceGenerator.Generator.Tree
{
    [DebuggerDisplay(@"{ResourceName}")]
    public abstract class Node
    {
        protected Node(BranchNode parent, string resourceName)
        {
            if (parent != null)
            {
                if (!parent.ChildrenResourceNames.Add(resourceName))
                    throw new InvalidOperationException($"Dupicated resource name `{resourceName}`");
                parent.Childern.Add(this);
            }

            this.Parent = parent;
            this.ResourceName = resourceName;

            this.MemberName = Helper.Refine(resourceName);
        }

        public BranchNode Parent { get; }

        public RootNode Root => this is RootNode r ? r : Parent.Root;

        public bool IsLeaf => this is LeafNode;
        public bool IsRoot => this is RootNode;

        public string ResourceName { get; }
        public virtual string ResourcePath => Helper.CombineResourcePath(Parent?.ResourcePath, ResourceName);

        public virtual string MemberName { get; }
    }
}