using System;
using System.IO;
using System.Reflection;
using ReSharperFixieTestRunner;

namespace ReSharperFixieTestProvider
{
    public static class FixieConventionLoader
    {
        public static FixieConventionInfo GetConventionInfo(string testAssemblyPath)
        {
            try
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                var executingAssemblyDirectory = Path.GetDirectoryName(executingAssembly.Location);

                var previousDirectory = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(executingAssemblyDirectory);

                FixieConventionInfo info;

                using (var appDomain = new AppDomainWrapper(executingAssemblyDirectory, "FixieConventionLoader"))
                {
                    var assemblyName = AssemblyName.GetAssemblyName("RemoteTestFinder.dll").FullName;
                    var remoteFinder = appDomain.CreateObject<ITestFinder>(
                        assemblyName,
                        "RemoteTestFinder.TestFinder");

                    info = remoteFinder.FindTests(testAssemblyPath);
                }

                Directory.SetCurrentDirectory(previousDirectory);
                return info;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
