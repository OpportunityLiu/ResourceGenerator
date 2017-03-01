using EnvDTE;
using System.IO;

namespace ResourceGenerator
{
    partial class Functions
    {
        public string ProductName => ResourceGenerator.Helper.ProductName;
        public string ProductVersion => ResourceGenerator.Helper.ProductVersion;

        public HostPlaceHolder Host { get; private set; } = new HostPlaceHolder();

        public Functions(TextWriter sw)
        {
            this.sw = sw;
        }

        private TextWriter sw;

        public void WriteLine(string value)
        {
            this.sw.WriteLine(value);
        }

        public void Write(string value)
        {
            this.sw.Write(value);
        }
    }
}
