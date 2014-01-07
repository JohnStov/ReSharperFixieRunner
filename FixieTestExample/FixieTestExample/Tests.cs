using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fixie;
using Fixie.Conventions;

using Should.Fluent;

namespace FixieTestExample
{
    public class TestsWeeb
    {
        public void Test()
        {
        }

        public void Test2Frob()
        {
            Console.Write("Test3Frob");
            1.Should().Equal(1);
        }

        [Input(2, 2)]
        [Input(2, 3)]
        public void ParameterisedFrob(int a, int b)
        {
            Console.WriteLine("a = {0}, b = {1}", a, b);
            a.Should().Equal(b);
        }

        [Skip]
        public void SkipFrob()
        {}
    }

    public class MyConvention : Convention
    {
        public MyConvention()
        {
            Classes.NameEndsWith("Weeb");   
            Methods.Where(m => m.Name.EndsWith("Frob"));
            Parameters(FromInputAttributes);
            CaseExecution.Skip(@case => @case.Method.HasOrInherits<SkipAttribute>());
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

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class SkipAttribute : Attribute { }
}
