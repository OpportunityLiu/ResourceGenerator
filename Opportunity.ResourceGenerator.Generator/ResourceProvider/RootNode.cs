using System.Collections.Generic;

namespace Opportunity.ResourceGenerator.Generator.ResourceProvider
{
    public class RootNode : BranchNode
    {
        public RootNode(string name)
            : base(null, name)
        {
        }

        public override string ResourceName
        {
            get
            {
                if (Configuration.Config.IsDefaultProject)
                    return $"ms-resource:///{Name}";
                else
                    return $"ms-resource:///{Configuration.Config.ProjectAssemblyName}/{Name}";
            }
        }

        public override string InterfaceNamespace => Configuration.Config.InterfacesNamespace;
        public override string ClassFullName => $"{Configuration.Config.LocalizedStringsFullName}.{ClassName}";
        public override string MemberFullName => $"{Configuration.Config.LocalizedStringsFullName}.{MemberName}";
    }
}