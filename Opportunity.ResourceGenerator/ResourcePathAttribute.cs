using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator
{
    /// <summary>
    /// Attribute stores path of resources, for debug usage.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ResourcePathAttribute : Attribute
    {
        /// <summary>
        /// Create a new instance of <see cref="ResourcePathAttribute"/>.
        /// </summary>
        /// <param name="path">The path of resource.</param>
        public ResourcePathAttribute(string path)
        {
            this.Path = path;
        }

        /// <summary>
        /// The path of resource.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Path { get; }

        /// <summary>
        /// Returns <see cref="Path"/>.
        /// </summary>
        public override string ToString()
        {
            return $"[{Path}]";
        }
    }
}
