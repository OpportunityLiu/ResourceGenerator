using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Linq;
using System.Web;

namespace ResourceGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = Debugger.LocalizedStringsDebugView.Current;
            Directory.CreateDirectory(@"../../Output/");
            using(var w = new StreamWriter(@"../../Output/ResourceGenerator.tt"))
            {
                new Generator(w).Generate();
                Console.WriteLine("T4 template generated at /Output/ResourceGenerator.tt");
            }
            using(var w = new StreamWriter(@"../../Output/ResourceGenerator.tt.cs"))
            {
                new Functions(w).Run();
                Console.WriteLine("T4 template output generated at /Output/ResourceGenerator.tt.cs");
            }


        }
    }
}


