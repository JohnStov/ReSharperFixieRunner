using System.IO;
using System.Reflection;

namespace FixiePlugin.TestDiscovery
{
    public static class LocalTestFinder
    {
        public static TestInfo GetConventionInfo(string testAssemblyPath)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyDirectory = Path.GetDirectoryName(executingAssembly.Location);

            var previousDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(executingAssemblyDirectory);

            TestInfo info;

            using (var appDomain = new AppDomainWrapper(executingAssemblyDirectory, "FixieTestFinder"))
            {
                var assemblyName = Assembly.GetExecutingAssembly().FullName;
                var className = typeof(RemoteTestFinder).FullName;
                var remoteFinder = appDomain.CreateObject<RemoteTestFinder>(
                    assemblyName,
                    className);

                info = remoteFinder.FindTests(testAssemblyPath);
            }

            Directory.SetCurrentDirectory(previousDirectory);
            return info;
        }
    }
}
