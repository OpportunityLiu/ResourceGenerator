using System;
using System.Diagnostics;

namespace Opportunity.ResourceGenerator
{
    /// <summary>
    /// Represent a resource path.
    /// </summary>
    [DebuggerDisplay("[{Key,nq}] = {Value}")]
    public struct GeneratedResourceProvider : IResourceProvider
    {
        /// <summary>
        /// Create a new instance of <see cref="GeneratedResourceProvider"/> with specified resource path.
        /// </summary>
        /// <param name="key">The specified resource path.</param>
        public GeneratedResourceProvider(string key)
        {
            this.Key = key;
        }

        /// <summary>
        /// The resource path of current <see cref="GeneratedResourceProvider"/>
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The resource string of <see cref="Key"/>.
        /// </summary>
        public string Value => LocalizedStrings.GetValue(Key);

        /// <summary>
        /// Get the <see cref="GeneratedResourceProvider"/> represents resource path ralative to current <see cref="GeneratedResourceProvider"/>.
        /// </summary>
        /// <param name="resourceKey">The subpath name of resource path.</param>
        /// <returns>
        /// The <see cref="GeneratedResourceProvider"/> represents resource path ralative to current <see cref="GeneratedResourceProvider"/>.
        /// </returns>
        public GeneratedResourceProvider this[string resourceKey]
        {
            get
            {
                if (resourceKey == null)
                    throw new ArgumentNullException();
                return new GeneratedResourceProvider($"{Key}/{resourceKey}");
            }
        }

        /// <summary>
        /// Get the resource string of resource path ralative to current <see cref="GeneratedResourceProvider"/>.
        /// </summary>
        /// <param name="resourceKey">The subpath name of resource path.</param>
        /// <returns>
        /// The resource string of resource path ralative to current <see cref="GeneratedResourceProvider"/>.
        /// </returns>
        public string GetValue(string resourceKey)
        {
            if (resourceKey == null)
                return this.Value;
            return LocalizedStrings.GetValue($"{Key}/{resourceKey}");
        }
    }
}
