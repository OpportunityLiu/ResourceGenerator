using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator
{
    public interface IResourceProvider
    {
        GeneratedResourceProvider this[string resourceKey] { get; }
        string GetValue(string resourceKey);
    }
}
