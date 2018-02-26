using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Opportunity.Helpers.Universal;

namespace Opportunity.ResourceGenerator
{
    /// <summary>
    /// Base class for generated resource classes.
    /// </summary>
    [DebuggerDisplay("{DebugString,nq}")]
    public abstract class ResourceProviderBase : IResourceProvider, IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Create instance with given resource path.
        /// </summary>
        /// <param name="path">A resource path starts with "ms-resource:", ends with "/".</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> dosen't meet the requirements.</exception>
        protected ResourceProviderBase(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (!path.StartsWith("ms-resource:") || !path.EndsWith("/"))
                throw new ArgumentException("Invalid path", nameof(path));
            this.path = path;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string path;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebugString => $"[{this.path.Substring(0, this.path.Length - 1)}]";

        /// <summary>
        /// Returns resource path of this instance.
        /// </summary>
        public override string ToString() => this.path;

        /// <summary>
        /// Get the <see cref="GeneratedResourceProvider"/> represents resource path ralative to current <see cref="ResourceProviderBase"/>.
        /// </summary>
        /// <param name="resourceKey">The subpath name of resource path.</param>
        /// <returns>
        /// The <see cref="GeneratedResourceProvider"/> represents resource path ralative to current <see cref="ResourceProviderBase"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="resourceKey"/> is null.</exception>
        /// <exception cref="ArgumentException">Format of <paramref name="resourceKey"/> is wrong.</exception>
        public GeneratedResourceProvider this[string resourceKey]
        {
            get
            {
                if (resourceKey == null)
                    throw new ArgumentNullException(nameof(resourceKey));
                return new GeneratedResourceProvider(this.path + resourceKey);
            }
        }

        /// <summary>
        /// Get the resource string of resource path ralative to current <see cref="ResourceProviderBase"/>.
        /// </summary>
        /// <param name="resourceKey">The subpath name of resource path.</param>
        /// <returns>
        /// The resource string of resource path ralative to current <see cref="ResourceProviderBase"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="resourceKey"/> is <see langword="null"/>.</exception>
        public string GetValue(string resourceKey)
        {
            if (resourceKey == null)
                throw new ArgumentNullException(nameof(resourceKey));
            return LocalizedStrings.GetValue(this.path + resourceKey);
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new ResourceDynamicMetaObject(parameter, this);
        }
    }
}
