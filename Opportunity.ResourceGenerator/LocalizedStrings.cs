using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace Opportunity.ResourceGenerator
{
    public static class LocalizedStrings
    {
        private static IDictionary<string, string> cache
            = new Dictionary<string, string>();
        private static ResourceLoader loader
            = ResourceLoader.GetForViewIndependentUse("");

        public static void Reset()
        {
            loader = ResourceLoader.GetForViewIndependentUse();
            cache.Clear();
        }

        public static string GetValue(string resourceKey)
        {
            if (cache.TryGetValue(resourceKey, out var value))
                return value;
            if (resourceKey.IndexOfAny(charsNeedToEscape) < 0)
                return cache[resourceKey] = loader.GetString(resourceKey);
            var escaped = resourceKey
                .Replace("%", "%25")
                .Replace("?", "%3F")
                .Replace("#", "%23")
                .Replace("*", "%2A")
                .Replace("\"", "%22");
            return cache[resourceKey] = loader.GetString(escaped);
        }

        internal static bool ValueLoaded(string resourceKey)
        {
            if (resourceKey == null)
                return false;
            return cache.ContainsKey(resourceKey);
        }

        private static readonly char[] charsNeedToEscape = "%?#*\"".ToCharArray();
    }
}
