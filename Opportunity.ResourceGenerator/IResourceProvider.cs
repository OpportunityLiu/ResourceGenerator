using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator
{
    /// <summary>
    /// Represnts a resource path.
    /// Base interface for generated interfaces.
    /// </summary>
    public interface IResourceProvider
    {
        /// <summary>
        /// Get the <see cref="GeneratedResourceProvider"/> represents resource path ralative to current <see cref="IResourceProvider"/>.
        /// </summary>
        /// <param name="resourceKey">The subpath name of resource path.</param>
        /// <returns>
        /// The <see cref="GeneratedResourceProvider"/> represents resource path ralative to current <see cref="IResourceProvider"/>.
        /// </returns>
        GeneratedResourceProvider this[string resourceKey] { get; }

        /// <summary>
        /// Get the resource string of resource path ralative to current <see cref="IResourceProvider"/>.
        /// </summary>
        /// <param name="resourceKey">The subpath name of resource path.</param>
        /// <returns>
        /// The resource string of resource path ralative to current <see cref="IResourceProvider"/>.
        /// </returns>
        string GetValue(string resourceKey);
    }
}
