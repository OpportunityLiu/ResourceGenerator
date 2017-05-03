using System.Collections.Generic;

namespace Opportunity.ResourceGenerator.Generator.ResourceProvider
{
    public class Node
    {
        public static Node GetNode(string name, string value)
        {
            return new Node(name, value);
        }

        public static Node GetNode(string name, Dictionary<string, object> values)
        {
            var r = new Node(name);
            foreach (var i in values)
            {
                if (i.Value is string v)
                {
                    r.Childern.Add(GetNode(i.Key, v));
                }
                else if (i.Value is Dictionary<string, object> v2)
                {
                    r.Childern.Add(GetNode(i.Key, v2));
                }
            }
            return r;
        }

        public Node(string name, string value)
        {
            this.Name = name;
            this.Value = value;
            setNames();
        }

        public Node(string name)
        {
            this.Name = name;
            this.Childern = new List<Node>();
            setNames();
        }

        protected void Refine()
        {
            if (Childern == null)
                return;
            foreach (var item in Childern)
            {
                item.Parent = this;
                item.Root = this.Root;
                item.Refine();
            }
        }

        private void setNames()
        {
            var r = Helper.Refine(Name);
            if (r.StartsWith("@"))
                this.IName = $"I{Name}";
            else
                this.IName = $"I{r}";
            this.CName = Helper.Refine(Helper.GetRandomName(Name));
            this.PName = r;
            this.FName = Helper.GetRandomName(r);
        }

        public IList<Node> Childern { get; }

        public Node Parent { get; protected set; }

        public RootNode Root { get; protected set; }

        public bool IsLeaf => Value != null;

        public string Name { get; }
        public string Value { get; }

        public virtual string RName => Helper.CombineResourcePath(Parent?.RName, Name);

        public string IName { get; private set; }
        public virtual string INs => $"{Parent.INs}.{Parent.PName}";

        public string CName { get; private set; }

        public string PName { get; private set; }

        public string FName { get; private set; }

        public string IFName => Configuration.Current.InterfaceFullName(IName, INs);

        public virtual string CFName => $"{Parent.CFName}.{CName}";

        public virtual string PFName => $"{Parent.CFName}.{PName}";
    }
}