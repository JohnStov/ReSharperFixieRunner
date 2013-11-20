using System;
using System.IO;
using System.Reflection;

namespace ReSharperFixieTestProvider.UnitTestProvider
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


            var conventionLoader = new FixieConventionDomainLoader();

            var appDomain = AppDomain.CreateDomain("FixieConventionLoader", null, appDomainSetup);
            appDomain.SetData("TestAssemblyPath", testAssemblyPath);
            appDomain.DoCallBack(conventionLoader.LoadTestClasses);
            var ex = (Exception)appDomain.GetData("Exception");
            if (ex != null)
                throw ex;

            var testClasses = (FixieConventionTestClass[])appDomain.GetData("TestClasses");
            AppDomain.Unload(appDomain);

            return new FixieConventionInfo(testClasses);
        }
    }
}
