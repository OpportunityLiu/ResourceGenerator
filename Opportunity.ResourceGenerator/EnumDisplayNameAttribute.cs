using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator
{
    /// <summary>
    /// Attribute stores display text of enum values.
    /// </summary>
    /// <see cref="EnumDisplayExtension.ToDisplayNameString{T}(T)"/>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumDisplayNameAttribute : Attribute
    {
        /// <summary>
        /// Create new instance of <see cref="EnumDisplayNameAttribute"/>.
        /// </summary>
        /// <param name="value">The display text of the enum value, can be a display text string or a resource string starts with "ms-resource:".</param>
        public EnumDisplayNameAttribute(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// The display text of the enum value, can be a display text string or a resource string starts with "ms-resource:".
        /// </summary>
        public string Value { get; }
    }
}
