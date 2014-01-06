using System;

namespace FixiePlugin
{
    public static class RequiredFixieVersion
    {
        private static readonly Version requiredVersion = new Version(0, 0, 1, 120);

        public static Version RequiredVersion { get { return requiredVersion; } }
    }
}
