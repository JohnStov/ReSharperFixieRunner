using System.IO;
using System.Reflection;

namespace FixiePlugin.TestDiscovery
{
    public static class ConventionFinder
    {
        public static ConventionInfo GetConventionInfo(string testAssemblyPath)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyDirectory = Path.GetDirectoryName(executingAssembly.Location);

            var previousDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(executingAssemblyDirectory);

            ConventionInfo info;

            using (var appDomain = new AppDomainWrapper(executingAssemblyDirectory, "FixieTestFinder"))
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
    }
}
