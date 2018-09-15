using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Opportunity.Helpers.Universal;
using Opportunity.ResourceGenerator;

[assembly: DebuggerDisplay(@"[{Key,nq}]: {Value}", Target = typeof(GeneratedResourceProvider))]

namespace Opportunity.ResourceGenerator
{
    /// <summary>
    /// Represent a resource path.
    /// </summary>
    public readonly struct GeneratedResourceProvider : IResourceProvider,
        IComparable, IComparable<GeneratedResourceProvider>, IEquatable<GeneratedResourceProvider>,
        IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Create a new instance of <see cref="GeneratedResourceProvider"/> with specified resource path.
        /// </summary>
        /// <param name="resourceKey">The specified resource path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resourceKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Format of <paramref name="resourceKey"/> is wrong.</exception>
        public GeneratedResourceProvider(string resourceKey)
        {
            if (resourceKey == null)
                throw new ArgumentNullException(nameof(resourceKey));
            if (resourceKey.Length == 0)
                throw new ArgumentException("Cannot be empty string", nameof(resourceKey));
            if (resourceKey.EndsWith("/"))
                throw new ArgumentException("Cannot end with '/'", nameof(resourceKey));
            this.Key = resourceKey;
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
        /// The resource string of <see cref="Key"/>.
        /// </summary>
        public override string ToString() => Value;

        int IComparable.CompareTo(object obj)
        {
            if (obj is null)
                return this.Key.CompareTo(null);
            if (obj is GeneratedResourceProvider v)
                return this.CompareTo(v);
            throw new ArgumentException("Wrong type.", nameof(obj));
        }

        /// <summary>
        /// Compare instances of <see cref="GeneratedResourceProvider"/>.
        /// </summary>
        /// <param name="other">Instance to compare with.</param>
        /// <returns>Order of two instances.</returns>
        public int CompareTo(GeneratedResourceProvider other) => this.Key.CompareTo(other.Key);

        /// <summary>
        /// Indicates whether current instance equals other instance.
        /// </summary>
        /// <param name="obj">Other instance to compare.</param>
        /// <returns>Whether current instance equals <paramref name="obj"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is GeneratedResourceProvider v)
                return Equals(v);
            return false;
        }

        /// <summary>
        /// Indicates whether current instance equals other instance.
        /// </summary>
        /// <param name="other">Other instance to compare.</param>
        /// <returns>Whether current instance equals <paramref name="other"/>.</returns>
        public bool Equals(GeneratedResourceProvider other) => this.Key == other.Key;

        /// <summary>
        /// Returns hash code of the <see cref="GeneratedResourceProvider"/>.
        /// </summary>
        /// <returns>Hash code of the <see cref="GeneratedResourceProvider"/>.</returns>
        public override int GetHashCode() => this.Key.GetHashCode();

        /// <summary>
        /// Get the <see cref="GeneratedResourceProvider"/> represents resource path ralative to current <see cref="GeneratedResourceProvider"/>.
        /// </summary>
        /// <param name="resourceKey">The subpath name of resource path.</param>
        /// <returns>
        /// The <see cref="GeneratedResourceProvider"/> represents resource path ralative to current <see cref="GeneratedResourceProvider"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="resourceKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Format of <paramref name="resourceKey"/> is wrong.</exception>
        public GeneratedResourceProvider this[string resourceKey]
        {
            get
            {
                if (resourceKey == null)
                    throw new ArgumentNullException(nameof(resourceKey));
                return new GeneratedResourceProvider(Key + "/" + resourceKey);
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
            return LocalizedStrings.GetValue(Key + "/" + resourceKey);
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new ResourceDynamicMetaObject(parameter, this);
        }
    }
}
