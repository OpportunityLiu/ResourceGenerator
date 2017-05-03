using System;
using System.Diagnostics;

namespace Opportunity.ResourceGenerator
{
    [DebuggerDisplay("[{key,nq}] = {Value}")]
    public struct GeneratedResourceProvider : IResourceProvider
    {
        public GeneratedResourceProvider(string key)
        {
            this.Key = key;
        }

        public string Key { get; }

        public string Value => LocalizedStrings.GetValue(Key);

        public GeneratedResourceProvider this[string resourceKey]
        {
            get
            {
                if (resourceKey == null)
                    throw new ArgumentNullException();
                return new GeneratedResourceProvider($"{Key}/{resourceKey}");
            }
        }

        public string GetValue(string resourceKey)
        {
            if (resourceKey == null)
                return this.Value;
            return LocalizedStrings.GetValue($"{Key}/{resourceKey}");
        }
    }
}
