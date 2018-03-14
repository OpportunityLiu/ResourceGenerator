using Newtonsoft.Json;
using Opportunity.ResourceGenerator.Generator.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator.Generator
{
    [JsonConverter(typeof(SelectRuleConverter))]
    public class SelectRule
    {
        private class SelectRuleConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(SelectRule);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var v = reader.Value;
                if (v is string s)
                    return new SelectRule(s);
                else
                    return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value is SelectRule r)
                    writer.WriteValue(r.Data);
                else
                    writer.WriteNull();
            }
        }

        public SelectRule(string data)
        {
            this.Data = data;
            var regex = data.Replace('*', '\0');

            if (regex.StartsWith("**/"))
                regex = "//.*?/" + regex.Substring(3);
            else if (regex.StartsWith("/") && !regex.StartsWith("///"))
                regex = "/" + regex;
            else
                regex = "//.*?/" + regex;

            if (regex.EndsWith("/**"))
                regex = regex.Substring(0, regex.Length - 3) + "/.*?$";
            else if (regex.EndsWith("/"))
                regex = regex + ".*?$";
            else
                regex = regex + "$";

            regex = regex
                .Replace("/\0\0/", "(/|/.+?/)")
                .Replace("\0", "[^/]*?");

            this.Regex = new Regex(regex);
        }

        public string Data { get; }

        public Regex Regex { get; }

        public bool Validate(Node target)
        {
            if (target == null)
                return false;
            var rName = target.ResourcePath;
            if (!(target is LeafNode))
                rName += "/";
            return Regex.IsMatch(rName);
        }
    }

}
