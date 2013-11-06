using System;
using System.IO;
using System.Reflection;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public static class FixieConventionLoader
    {
        public static FixieConventionInfo GetConventionInfo(string testAssemblyPath)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyDirectory = Path.GetDirectoryName(executingAssembly.Location);
            var testAssemblyDirectory = Path.GetDirectoryName(testAssemblyPath);

            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = executingAssemblyDirectory,
                PrivateBinPath = testAssemblyDirectory,
                ShadowCopyFiles = "true",
            };

            var appDomain = AppDomain.CreateDomain("FixieConventionLoader", null, appDomainSetup);

            var conventionLoader = new FixieConventionDomainLoader(testAssemblyPath);

            appDomain.DoCallBack(conventionLoader.LoadTestClasses);

            AppDomain.Unload(appDomain);

            return new FixieConventionInfo(conventionLoader.Classes);
        }
    }
}
