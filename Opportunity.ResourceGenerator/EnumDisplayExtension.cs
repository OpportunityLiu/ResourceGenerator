using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Opportunity.ResourceGenerator;

namespace System
{
    public static class EnumDisplayExtension
    {
        private static class EnumExtentionCache<T>
            where T : struct, IComparable, IFormattable, IConvertible
        {
            static EnumExtentionCache()
            {
                foreach (var field in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    var name = field.Name;
                    var value = field.GetValue(null).Cast<T>();
                    var da = field.GetCustomAttribute<EnumDisplayNameAttribute>();
                    if (da == null || da.Value == null)
                        DisplayNames[value] = name;
                    else if (!da.Value.StartsWith("ms-resource"))
                        DisplayNames[value] = da.Value;
                    else
                        DisplayNames[value] = LocalizedStrings.GetValue(da.Value);

                }
            }
            public static readonly Dictionary<T, string> DisplayNames = new Dictionary<T, string>();
        }

        public static string ToDisplayNameString<T>(this T that)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return EnumExtension.ToFriendlyNameString(that, v => EnumExtentionCache<T>.DisplayNames[v]);
        }
    }
}
