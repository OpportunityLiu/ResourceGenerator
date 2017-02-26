using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Linq;
using System.Web;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main()
        {
            using(var w = new StreamWriter(@"C:\Users\liuzh\Documents\Visual Studio 2015\Projects\ConsoleApplication1\Strings\TextTemplate1.tt"))
            {
                new Generator(w).Generate();
            }
            using(var w = new StreamWriter(@"C:\Users\liuzh\Documents\Visual Studio 2015\Projects\ConsoleApplication1\Strings\TextTemplate1.tt.cs"))
            {
                new Functions(w).Run();
            }
        }

    }
}


