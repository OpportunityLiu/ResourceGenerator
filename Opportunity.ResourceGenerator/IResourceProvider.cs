using System;
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
        /// <exception cref="ArgumentNullException"><paramref name="resourceKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Format of <paramref name="resourceKey"/> is wrong.</exception>
        GeneratedResourceProvider this[string resourceKey] { get; }

        /// <summary>
        /// Get the resource string of resource path ralative to current <see cref="IResourceProvider"/>.
        /// </summary>
        /// <param name="resourceKey">The subpath name of resource path.</param>
        /// <returns>
        /// The resource string of resource path ralative to current <see cref="IResourceProvider"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="resourceKey"/> is <see langword="null"/>.</exception>
        string GetValue(string resourceKey);
    }
}
