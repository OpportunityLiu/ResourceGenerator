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
            internal ResourcePathAttribute ResourcePath { get; set; }
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            internal string Name { get; set; }
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
            var q = from p in this.provider.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    where p.GetMethod.GetParameters().Length == 0
                    let value = p.GetValue(this.provider)
                    let path = p.GetCustomAttribute<ResourcePathAttribute>()
                    orderby value.GetType().ToString(), path.ToString()
                    select new { Prop = p, Value = value, Path = path };
            var props = q.ToArray();
            this.items = new ResourceView[props.Length];
            for (var i = 0; i < props.Length; i++)
            {
                var p = props[i];
                var name = p.Prop.Name;
                var dot = name.LastIndexOf('.');
                if (dot != -1)
                    name = name.Substring(dot + 1);
                if (p.Value is string vs)
                {
                    this.items[i] = new ResourceValueView
                    {
                        Name = name,
                        Value = vs,
                        ResourcePath = p.Path
                    };
                }
                else
                {
                    this.items[i] = new ResourcePathView
                    {
                        Name = name,
                        Value = p.Value,
                        ResourcePath = p.Path
                    };
                }
            }
        }

        internal DebuggerDisplay(IResourceProvider provider)
        {
            this.provider = provider;
        }
    }
}
