using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator.Generator.ResourceProvider
{
    public abstract class Provider
    {
        public static IList<Provider> Providers { get; } = new List<Provider>
        {
            new ResjsonProvider(),
            new ReswProvider()
        };

        private static Dictionary<string, Dictionary<string, object>> result;

        public static List<RootNode> Analyze(IEnumerable<string> files)
        {
            result = new Dictionary<string, Dictionary<string, object>>();
            foreach (var fileName in files)
            {
                resourceRootName = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                foreach (var item in Providers)
                {
                    if (string.Equals(item.CanHandleExt, ext, StringComparison.InvariantCultureIgnoreCase))
                    {
                        item.Analyze(fileName);
                        break;
                    }
                }
            }
            var r = RootNode.GetTree(result);
            result = null;
            return r;
        }

        protected abstract void Analyze(string fileName);

        protected abstract string CanHandleExt { get; }


        private static string resourceRootName;

        protected void SetValue(IList<string> path, string value)
        {
            result.TryGetValue(resourceRootName, out var o);
            if (o == null)
                result[resourceRootName] = o = new Dictionary<string, object>();
            SetValueCore(o, path, 0, value);
        }

        private void SetValueCore(Dictionary<string, object> output, IList<string> path, int index, string value)
        {
            if (index == path.Count - 1)
                SetValueCore(output, path[index], value);
            else
            {
                output.TryGetValue(path[index], out var o);
                var dic = o as Dictionary<string, object>;
                if (dic == null)
                    output[path[index]] = dic = new Dictionary<string, object>();
                SetValueCore(dic, path, index + 1, value);
            }
        }

        private void SetValueCore(Dictionary<string, object> output, string path, string value)
        {
            output[path] = value;
        }
    }
}
