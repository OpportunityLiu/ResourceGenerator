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
        public abstract class ResourceView
        {
            internal ResourceView() { }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if !DEBUG
            internal 
#else
            public
#endif
            ResourcePathAttribute ResourcePath
            {
                get; set;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if !DEBUG
            internal 
#else
            public
#endif
            string Name
            {
                get; set;
            }
        }

        [DebuggerDisplay("{Value}", Name = "{Name,nq}", Type = "string")]
        private sealed class ResourceValueView : ResourceView
        {
            public string Value { get; set; }

            public ResourcePathAttribute Path => this.ResourcePath;
        }

        [DebuggerDisplay("{ResourcePath.ToString(),nq}", Name = "{Name,nq}", Type = "{GetDisplayType(),nq}")]
        private sealed class ResourcePathView : ResourceView
        {
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            internal object Value;

            internal string GetDisplayType()
            {
                if (this.Value == null)
                    return "object";
                var type = this.Value.GetType();
                var interfaces = type.GetInterfaces();
                if (interfaces.Length == 0)
                    return type.ToString();
                return interfaces[0].ToString();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IResourceProvider provider;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ResourceView[] items;

        /// <summary>
        /// Key-value pairs of resources.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public ResourceView[] Items
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
            this.items = this.provider.GetType()
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Select(prop => new { prop, path = prop.GetCustomAttribute<ResourcePathAttribute>() })
                .Where(o => o.path != null)
                .OrderBy(o => o.prop.PropertyType, Comparer<Type>.Create((a, b) =>
                {
                    if (a == b)
                        return 0;
                    if (a == typeof(string))
                        return 1;
                    else if (b == typeof(string))
                        return -1;
                    return StringComparer.Ordinal.Compare(a.ToString(), b.ToString());
                }))
                .ThenBy(o => o.path.Path)
                .Select(o =>
                {
                    var name = o.prop.Name;
                    var dot = name.LastIndexOf('.');
                    if (dot != -1)
                        name = name.Substring(dot + 1);
                    var value = o.prop.GetValue(this.provider);
                    if (value is string vs)
                    {
                        return (ResourceView)new ResourceValueView
                        {
                            Name = name,
                            Value = vs,
                            ResourcePath = o.path
                        };
                    }
                    else
                    {
                        return new ResourcePathView
                        {
                            Name = name,
                            Value = value,
                            ResourcePath = o.path
                        };
                    }
                }).ToArray();
        }

#if !DEBUG
        internal 
#else
        public
#endif
            DebuggerDisplay(IResourceProvider provider)
        {
            this.provider = provider;
        }
    }
}
