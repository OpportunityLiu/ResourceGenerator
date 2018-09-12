using Opportunity.Helpers.Universal;
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
    internal sealed class DebuggerDisplay
    {
        /// <summary>
        /// Key-value pair of resources.
        /// </summary>
        public abstract class ResourceView
        {
            internal ResourceView() { }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            internal ResourcePathAttribute ResourcePath { get; set; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            internal string Name { get; set; }
        }

        [DebuggerDisplay("{Value}", Name = "{Name,nq}", Type = "Resource String")]
        private sealed class ResourceValueView : ResourceView
        {
            public Uri Path => new Uri(this.ResourcePath.ToString());
            public string Value { get; set; }
        }

        [DebuggerDisplay(@"\{{ResourcePath.Path,nq}\}", Name = "{Name,nq}", Type = "{DisplayType,nq}")]
        private sealed class ResourcePathView : ResourceView
        {
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            internal object Value;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            internal string DisplayType
            {
                get
                {
                    if (this.Value == null)
                        return "object";
                    var type = this.Value.GetType();
                    var interfaces = type.GetInterfaces();
                    if (interfaces.Length == 0)
                        return type.ToString();
                    return interfaces.Last().ToString();
                }
            }
        }

        [DebuggerDisplay("${Value}", Name = "{Name,nq}", Type = "Format Method")]
        private sealed class ResourceFormatMethodView : ResourceView
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public string Value { get; set; }

            public Uri Path => new Uri(this.ResourcePath.ToString());

            public FormattableResourceString Format { get; set; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IResourceProvider provider;
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
                if (this.items is null)
                    init();
                return this.items;
            }
        }

        private void init()
        {
            var type = this.provider.GetType();
            var props = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
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
                });
            var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Select(method => new { method, path = method.GetCustomAttribute<ResourcePathAttribute>() })
                .Where(o => o.path != null)
                .OrderBy(o => o.path.Path)
                .Select(o =>
                {
                    var name = o.method.Name;
                    var dot = name.LastIndexOf('.');
                    if (dot != -1)
                        name = name.Substring(dot + 1);
                    var value = LocalizedStrings.GetValue(o.path.Path);
                    var format = new FormattableResourceString(value);
                    return (ResourceView)new ResourceFormatMethodView
                    {
                        Name = name,
                        Value = value,
                        Format = format,
                        ResourcePath = o.path
                    };
                });
            this.items = props.Concat(methods).ToArray();
        }

        internal DebuggerDisplay(IResourceProvider provider)
        {
            this.provider = provider;
        }
    }
}
