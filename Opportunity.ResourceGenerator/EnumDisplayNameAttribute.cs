using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumDisplayNameAttribute : Attribute
    {
        public EnumDisplayNameAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; }
    }
}
