using System.Collections.Generic;

namespace ReSharperFixieRunner.UnitTestProvider
{
    internal class FixieConventionTestClassComparer : IEqualityComparer<FixieConventionTestClass>
    {
        public bool Equals(FixieConventionTestClass x, FixieConventionTestClass y)
        {
            return x.TypeName == y.TypeName;
        }

        public int GetHashCode(FixieConventionTestClass obj)
        {
            return obj.GetHashCode();
        }
    }
}