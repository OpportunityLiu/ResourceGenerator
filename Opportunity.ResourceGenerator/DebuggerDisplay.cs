using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator
{
    /// <summary>
    /// Debug view for generated classes.
    /// </summary>
    public class DebuggerDisplay
    {
        /// <summary>
        /// Key-value pair of resources.
        /// </summary>
        [DebuggerDisplay("{GetDisplayValue(),nq}", Name = "{Name,nq}", Type = "{GetDisplayType(),nq}")]
        public struct Pair
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            internal string Name;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            internal object Value;

            internal string GetDisplayValue()
            {
                if (this.Value == null)
                    return "null";
                if (this.Value is string vs)
                {
                    return asLiteral(vs);
                }
                else
                {
                    var dbg = this.Value.GetType().GetTypeInfo().GetCustomAttribute<DebuggerDisplayAttribute>(true);
                    if (dbg != null)
                        return dbg.Value;
                    else
                        return this.Value.ToString();
                }
            }

            internal string GetDisplayType()
            {
                if (this.Value == null)
                    return "object";
                if (this.Value is string)
                    return "string";
                var type = this.Value.GetType();
                var interfaces = type.GetInterfaces();
                if (interfaces.Length == 0)
                    return type.ToString();
                return interfaces[0].ToString();
            }
        }

        private IResourceProvider provider;
        private Pair[] items;

        /// <summary>
        /// Key-value pairs of resources.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Pair[] Items
        {
            get
            {
                if (this.items == null)
                    init();
                return this.items;
            }
        }

        private void init()
        {
            var q = from p in this.provider.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    where p.GetMethod.GetParameters().Length == 0
                    select p;
            var props = q.ToArray();
            this.items = new Pair[props.Length];
            for (var i = 0; i < props.Length; i++)
            {
                var p = props[i];
                var name = p.Name;
                var dot = name.LastIndexOf('.');
                if (dot != -1)
                    name = name.Substring(dot + 1);
                this.items[i].Name = name;
                this.items[i].Value = p.GetValue(this.provider);
            }
        }


        private static string asLiteral(string value)
        {
            var sb = new StringBuilder(value.Length);
            sb.Append('"');
            foreach (var item in value)
            {
                var chType = CharUnicodeInfo.GetUnicodeCategory(item);
                if (item == '"')
                    sb.Append(@"\""");
                else if (item == '\\')
                    sb.Append(@"\\");
                else if (item == '\0')
                    sb.Append(@"\0");
                else if (item == '\a')
                    sb.Append(@"\a");
                else if (item == '\b')
                    sb.Append(@"\b");
                else if (item == '\f')
                    sb.Append(@"\f");
                else if (item == '\r')
                    sb.Append(@"\r");
                else if (item == '\n')
                    sb.Append(@"\n");
                else if (item == '\t')
                    sb.Append(@"\t");
                else if (item == '\v')
                    sb.Append(@"\v");
                else if (chType == UnicodeCategory.Control)
                {
                    sb.Append(@"\u");
                    sb.Append(((ushort)item).ToString("X4"));
                }
                else
                {
                    sb.Append(item);
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        internal DebuggerDisplay(IResourceProvider provider)
        {
            this.provider = provider;
        }
    }
}
