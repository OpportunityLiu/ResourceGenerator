using System.Collections.Generic;

namespace Opportunity.ResourceGenerator.Generator.ResourceProvider
{
    public class RootNode : Node
    {
        public static List<RootNode> GetTree(Dictionary<string, Dictionary<string, object>> raw)
        {
            var l = new List<RootNode>();
            foreach (var item in raw)
            {
                l.Add(getRootNode(item));
            }
            return l;
        }

        private static RootNode getRootNode(KeyValuePair<string, Dictionary<string, object>> item)
        {
            var node = new RootNode(item.Key);
            foreach (var i in item.Value)
            {
                if (i.Value is string v)
                {
                    node.Childern.Add(GetNode(i.Key, v));
                }
                else if (i.Value is Dictionary<string, object> v2)
                {
                    node.Childern.Add(GetNode(i.Key, v2));
                }
            }
            node.Refine();
            return node;
        }

        public RootNode(string name)
            : base(name)
        {
            this.Root = this;
            this.Parent = null;
        }

        public override string RName
        {
            get
            {
                if (Configuration.Current.IsDefaultProject)
                    return $"ms-resource:///{Name}";
                else
                    return $"ms-resource:///{Configuration.Current.ProjectAssemblyName}/{Name}";
            }
        }
        public override string INs => Configuration.Current.InterfacesNamespace;
        public override string CFName => $"{Configuration.Current.LocalizedStringsFullName}.{CName}";
        public override string PFName => $"{Configuration.Current.LocalizedStringsFullName}.{PName}";
    }
}