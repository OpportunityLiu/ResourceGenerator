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
                this.InterfaceName = $"I{Name}";
            else
                this.InterfaceName = $"I{r}";
            this.ClassName = Helper.Refine(Helper.GetRandomName(Name));
            this.PropertyName = r;
            this.FieldName = Helper.GetRandomName(r);
        }

        public IList<Node> Childern { get; }

        public Node Parent { get; protected set; }

        public RootNode Root { get; protected set; }

        public bool IsLeaf => Value != null;

        public string Name { get; }
        public string Value { get; }

        public virtual string ResourceName => Helper.CombineResourcePath(Parent?.ResourceName, Name);

        public string InterfaceName { get; private set; }
        public virtual string InterfaceNamespace => $"{Parent.InterfaceNamespace}.{Parent.PropertyName}";

        public string ClassName { get; private set; }

        public string PropertyName { get; private set; }

        public string FieldName { get; private set; }

        public string InterfaceFullName => Configuration.Current.InterfaceFullName(InterfaceName, InterfaceNamespace);

        public virtual string ClassFullName => $"{Parent.ClassFullName}.{ClassName}";

        public virtual string PropertyFullName => $"{Parent.ClassFullName}.{PropertyName}";
    }
}