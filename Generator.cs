using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Generator
    {
        public Generator(TextWriter sw)
        {
            this.sw = sw;
        }

        private TextWriter sw;

        public void Generate()
        {
            sw.WriteLine(
@"<#@ template debug=""false"" hostspecific=""true"" language=""C#"" #>
<#@ assembly name=""System"" #>
<#@ assembly name=""System.Core"" #>
<#@ assembly name=""System.Xml"" #>
<#@ assembly name=""System.Web"" #>
<#@ assembly name=""System.Runtime.Serialization"" #>
<#@ assembly name=""EnvDTE"" #>
<#@ import namespace=""System"" #>
<#@ import namespace=""System.IO"" #>
<#@ import namespace=""System.Xml"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""System.Runtime.Serialization.Json"" #>
<#@ import namespace=""System.Web"" #>
<#@ import namespace=""System.Linq"" #>
<#@ output extension="".tt.cs"" #>");

            WriteCodeBegin();
            var sc = ReadLines(@"C:\Users\liuzh\Documents\Visual Studio 2015\Projects\ConsoleApplication1\Scripts.cs").ToArray();
            WriteLines(sc.Skip(10).Take(sc.Length - 10 - 2));
            WriteCodeEnd();
            WriteBlockBegin();
            var fc = ReadLines(@"C:\Users\liuzh\Documents\Visual Studio 2015\Projects\ConsoleApplication1\Functions.cs").ToArray();
            WriteLines(fc.Skip(10).Take(fc.Length - 10 - 1));
            WriteBlockEnd();

        }

        private void WriteCodeBegin()
        {
            sw.WriteLine("<#");
        }

        private void WriteCodeEnd()
        {
            sw.WriteLine("#>");
        }

        private void WriteBlockBegin()
        {
            sw.WriteLine("<#+");
        }

        private void WriteBlockEnd()
        {
            sw.WriteLine("#>");
        }

        private void WriteLines(IEnumerable<string> value)
        {
            foreach(var item in value)
            {
                sw.WriteLine(item);
            }
        }

        private IEnumerable<string> ReadLines(string path)
        {
            using(var r = new StreamReader(path))
            {
                string l;
                while((l = r.ReadLine()) != null)
                {
                    yield return l;
                }
            }
        }
    }
}
