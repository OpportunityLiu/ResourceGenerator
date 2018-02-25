using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Opportunity.ResourceGenerator.Generator.ResourceProvider
{
    class ResjsonProvider : Provider
    {
        protected override string CanHandleExt => ".resjson";

        protected override void Analyze(string fileName)
        {
            var resourceName = Path.GetFileNameWithoutExtension(fileName);
            var json = File.ReadAllText(fileName);
            var tree = JsonConvert.DeserializeObject<JObject>(json);
            analyzeObject(new List<string>(), tree);
        }

        private void analyzeObject(List<string> path, JObject obj)
        {
            foreach (var item in obj)
            {
                if (item.Key.StartsWith("_"))
                    continue; // Resources starts with "_" is comments.
                var subPath = item.Key.Split('/');
                path.AddRange(subPath);
                switch (item.Value.Type)
                {
                case JTokenType.Object:
                    analyzeObject(path, (JObject)item.Value);
                    break;
                case JTokenType.String:
                    SetValue(path, item.Value.ToString());
                    break;
                }
                path.RemoveRange(path.Count - subPath.Length, subPath.Length);
            }
        }
    }
}
