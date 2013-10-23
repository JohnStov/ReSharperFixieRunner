using System;
using System.Collections.Generic;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public class FixieClassFilter
    {
        public FixieClassFilter(object classFilter)
        {
            
        }

        public IEnumerable<Type> Filter(IEnumerable<Type> classes)
        {
            yield break;
        }
    }
}