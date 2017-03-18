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
            var sc = ReadLines(@"../../Scripts.cs").ToArray();
            var fc = ReadLines(@"../../Functions.cs").ToArray();
            WriteBlock(
@"<#@ template debug=""false"" hostspecific=""true"" language=""C#"" #>
<#@ assembly name=""System"" #>
<#@ assembly name=""System.Core"" #>
<#@ assembly name=""System.Xml"" #>
<#@ assembly name=""System.Web"" #>
<#@ assembly name=""System.Runtime.Serialization"" #>
<#@ assembly name=""EnvDTE"" #>");

            var usings = fc.TakeWhile(s => s.StartsWith("using"))
                .Select(s => s.Split(' ', ';')[1])
                .Select(s => $@"<#@ import namespace=""{s}"" #>").ToArray();
            WriteLines(usings, 0);

            WriteLine(@"<#@ output extension="".tt.cs"" #>", 0);

            WriteCodeBegin();
            WriteLines(sc.Skip(6).Take(sc.Length - 9), -2);
            WriteCodeEnd();

            WriteBlockBegin();
            WriteLine();
            WriteLine($"public string ProductName => \"{Helper.ProductName}\";", 1);
            WriteLine($"public string ProductVersion => \"{Helper.ProductVersion}\";", 1);
            WriteLine();
            WriteBlockEnd();

            WriteBlockBegin();
            WriteLines(fc.Skip(usings.Length + 5).Take(fc.Length - 7 - usings.Length), -1);
            WriteBlockEnd();

        }

        private void WriteBlock(string value)
        {
            this.sw.WriteLine(value);
        }

        private void WriteLine(string value, int indent)
        {
            if(indent >= 0)
            {
                for(var i = 0; i < indent; i++)
                {
                    this.sw.Write("    ");
                }
                this.sw.WriteLine(value);
            }
            else
            {
                var empty = new string(' ', -indent * 4);
                if(value.StartsWith(empty))
                    this.sw.WriteLine(value.Substring(-indent * 4));
                else
                    this.sw.WriteLine(value.TrimStart());
            }
        }

        private void WriteLine()
        {
            this.sw.WriteLine();
        }

        private void WriteCodeBegin()
        {
            this.sw.WriteLine("<#");
        }

        private void WriteCodeEnd()
        {
            this.sw.WriteLine("#>");
        }

        private void WriteBlockBegin()
        {
            this.sw.WriteLine("<#+");
        }

        private void WriteBlockEnd()
        {
            this.sw.WriteLine("#>");
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
