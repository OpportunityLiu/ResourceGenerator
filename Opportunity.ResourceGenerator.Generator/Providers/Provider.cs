using Opportunity.ResourceGenerator.Generator.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator.Generator.Providers
{
    public abstract class Provider
    {
        public static IList<Provider> Providers { get; } = new List<Provider>
        {
            new ResjsonProvider(),
            new ReswProvider()
        };

        public static List<RootNode> Analyze(IEnumerable<string> files)
        {
            var r = new List<RootNode>();
            foreach (var fileName in files)
            {
                var resourceRootName = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                foreach (var item in Providers)
                {
                    if (string.Equals(item.CanHandleExt, ext, StringComparison.OrdinalIgnoreCase))
                    {
                        var tree = r.Find(t => string.Equals(t.ResourceName, resourceRootName, StringComparison.OrdinalIgnoreCase));
                        if (tree is null)
                        {
                            tree = new RootNode(resourceRootName);
                            r.Add(tree);
                        }
                        item.Analyze(fileName, tree);
                        break;
                    }
                }
            }
            return r;
        }

        protected abstract void Analyze(string fileName, RootNode resourceTree);

        protected abstract string CanHandleExt { get; }

        protected void SetValue(RootNode resourceTree, IList<string> path, string value)
        {
            var currentNode = (BranchNode)resourceTree;
            for (var i = 0; i < path.Count - 1; i++)
            {
                var nodeName = path[i];
                if (!currentNode.Childern.TryGetValue(nodeName, out var child))
                {
                    child = new BranchNode(currentNode, nodeName);
                }
                currentNode = (BranchNode)child;
            }
            var leafName = path[path.Count - 1];
            new LeafNode(currentNode, leafName, value);
        }
    }
}
