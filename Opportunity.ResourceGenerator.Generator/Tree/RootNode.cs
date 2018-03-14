using System.Collections.Generic;

namespace Opportunity.ResourceGenerator.Generator.Tree
{
    public class RootNode : BranchNode
    {
        public RootNode(string resourceName)
            : base(null, resourceName) { }

        public override string ResourcePath
        {
            get
            {
                if (Configuration.Config.IsDefaultProject)
                    return $"ms-resource:///{ResourceName}";
                else
                    return $"ms-resource:///{Configuration.Config.ProjectAssemblyName}/{ResourceName}";
            }
        }

        public override string InterfaceNamespace => Configuration.Config.InterfacesNamespace;
        public override string ClassFullName => $"{Configuration.Config.LocalizedStringsFullName}.{ClassName}";
        public override string MemberFullName => $"{Configuration.Config.LocalizedStringsFullName}.{MemberName}";
    }
}