using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace Opportunity.ResourceGenerator
{
    /// <summary>
    /// Helper class for localized strings.
    /// </summary>
    public static class LocalizedStrings
    {
        private static Dictionary<string, string> cache = new Dictionary<string, string>();
        private static ResourceLoader loader = ResourceLoader.GetForViewIndependentUse("");

        /// <summary>
        /// Reset the <see cref="LocalizedStrings"/>.
        /// </summary>
        public static void Reset()
        {
            loader = ResourceLoader.GetForViewIndependentUse("");
            cache.Clear();
        }

        /// <summary>
        /// Get value of given <paramref name="resourceKey"/>.
        /// Speical chars (%?#*" \t) will be escaped in this method, don't need to escape before calling.
        /// </summary>
        /// <param name="resourceKey">The key of resource.</param>
        /// <returns>The value of resource.</returns>
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
                .Replace("\"", "%22")
                .Replace(" ", "%20")
                .Replace("\t", "%09");
            return cache[resourceKey] = loader.GetString(escaped);
        }

        private static readonly char[] charsNeedToEscape = "%?#*\"\t ".ToCharArray();
    }
}
