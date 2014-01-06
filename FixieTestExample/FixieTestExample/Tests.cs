using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Fixie.Conventions;

using Should.Fluent;

namespace FixieTestExample
{
    public class TestsWeeb
    {
        public void Test()
        {
        }

        public void Test3Frob()
        {
            Console.Write("Test3Frob");
            throw new ArgumentException("Noodle");
        }

        [Input(2, 2)]
        [Input(2, 3)]
        public void ParameterisedFrob(int a, int b)
        {
            Console.WriteLine("a = {0}, b = {1}", a, b);
            a.Should().Equal(b);
        }
    }

    public class MyConvention : Convention
    {
        public MyConvention()
        {
            Classes.NameEndsWith("Weeb");   
            Methods.Where(m => m.Name.EndsWith("Frob"));
            Parameters(FromInputAttributes);
        }

        static IEnumerable<object[]> FromInputAttributes(MethodInfo method)
        {
            return method.GetCustomAttributes<InputAttribute>(true).Select(input => input.Parameters);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InputAttribute : Attribute
    {
        public InputAttribute(params object[] parameters)
        {
            Parameters = parameters;
        }

        public object[] Parameters { get; private set; }
    }
}
