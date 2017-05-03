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
    public class DebuggerDisplay
    {
        [DebuggerDisplay("{DisplayValue,nq}", Name = "{Name,nq}")]
        public struct Pair
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public string Name;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public string DisplayValue;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public object Value;
        }

        private IResourceProvider provider;
        private Pair[] items;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IList<Pair> Items
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
                var v = p.GetValue(this.provider);
                if (v is string vs)
                {
                    this.items[i].DisplayValue = asLiteral(vs);
                }
                else
                {
                    var dbg = v.GetType().GetTypeInfo().GetCustomAttribute<DebuggerDisplayAttribute>(true);
                    if (dbg != null)
                        this.items[i].DisplayValue = dbg.Value;
                    else
                        this.items[i].DisplayValue = v.ToString();
                }
                this.items[i].Value = v;
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

        public DebuggerDisplay(IResourceProvider provider)
        {
            this.provider = provider;
        }
    }
}
