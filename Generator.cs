using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ResourceGenerator
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
            WriteBlock(
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
            var sc = ReadLines(@"../../Scripts.cs").ToArray();
            WriteLines(sc.Skip(10).Take(sc.Length - 10 - 2), -1);
            WriteCodeEnd();

            WriteBlockBegin();
            WriteLine();
            WriteLine($"public string ProductName => \"{Helper.ProductName}\";", 1);
            WriteLine($"public string ProductVersion => \"{Helper.ProductVersion}\";", 1);
            WriteLine();
            WriteBlockEnd();

            WriteBlockBegin();
            var fc = ReadLines(@"../../Functions.cs").ToArray();
            WriteLines(fc.Skip(10).Take(fc.Length - 10 - 1), 0);
            WriteBlockEnd();

        }

        private void WriteBlock(string value)
        {
            sw.WriteLine(value);
        }

        private void WriteLine(string value, int indent)
        {
            if(indent >= 0)
            {
                for(int i = 0; i < indent; i++)
                {
                    sw.Write("    ");
                }
                sw.WriteLine(value);
            }
            else
            {
                var empty = new string(' ', -indent * 4);
                if(value.StartsWith(empty))
                    sw.WriteLine(value.Substring(-indent * 4));
                else
                    sw.WriteLine(value.TrimStart());
            }
        }

        private void WriteLine()
        {
            sw.WriteLine();
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

        private void WriteLines(IEnumerable<string> value, int indent)
        {
            foreach(var item in value)
            {
                WriteLine(item, indent);
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
