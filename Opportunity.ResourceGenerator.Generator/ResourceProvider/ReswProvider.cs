using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Opportunity.ResourceGenerator.Generator.ResourceProvider
{
    class ReswProvider : Provider
    {
        protected override string CanHandleExt => ".resw";

        protected override void Analyze(string fileName, RootNode resourceTree)
        {
            var document = new XmlDocument();
            document.Load(fileName);

            var dataNodes = document.GetElementsByTagName("data");
            foreach (var dataNode in dataNodes.Cast<XmlElement>())
            {
                if (dataNode != null)
                {
                    var value = dataNode.GetAttribute("name");
                    SetValue(resourceTree, value.Split('.', '/'), dataNode["value"].InnerText);
                }
            }
        }
    }
}
