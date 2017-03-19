using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace ResourceGenerator.Debugger
{
    class LocalizedStringsDebugView
    {
        public static LocalizedStringsDebugView Current { get; } = new LocalizedStringsDebugView();

        private LocalizedStringsDebugView()
        {
            var type = typeof(LocalizedStrings);
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
            this.cf = fields.First(field => field.Name.StartsWith("__cache__"));
            this.lf = fields.First(field => field.Name.StartsWith("__loader__"));
        }

        private readonly FieldInfo cf, lf;

        public IDictionary<string, string> Cache => (IDictionary<string, string>)this.cf.GetValue(null);
        public string CacheName => this.cf.Name;

        public ResourceLoader Loader => (ResourceLoader)this.lf.GetValue(null);
        public string LoaderName => this.lf.Name;
    }
}
