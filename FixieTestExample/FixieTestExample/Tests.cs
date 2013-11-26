using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fixie.Conventions;

namespace FixieTestExample
{
    public class TestsWeeb
    {
        public void Test()
        {
        }

        public void Test3Frob()
        {
            Console.WriteLine("Test3Frob");
            throw new Exception("Test3Frob");
        }
    }

    public class MyConvention : Convention
    {
        public MyConvention()
        {
            Classes.NameEndsWith("Weeb");
            Methods.Where(m => m.Name.EndsWith("Frob"));
        }
    }
}
