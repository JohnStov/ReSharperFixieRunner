using System;

namespace FixiePlugin
{
    public class AppDomainWrapper : IDisposable
    {
        private readonly AppDomain appDomain;

        public AppDomainWrapper(string loadPath, string domainName = null)
        {
            if (domainName == null)
                domainName = Guid.NewGuid().ToString();

            appDomain = CreateAppDomain(domainName, loadPath);
        }

        AppDomain CreateAppDomain(string domainName, string loadPath)
        {
            return AppDomain.CreateDomain(domainName, null, loadPath, "", true);
        }

        public object CreateObject(string assemblyName, string typeName)
        {
            return appDomain.CreateInstanceAndUnwrap(assemblyName, typeName);
        }

        public T CreateObject<T>(string assemblyName, string typeName)
        {
            return (T)CreateObject(assemblyName, typeName);
        }

        public void Dispose()
        {
            if (appDomain != null)
                AppDomain.Unload(appDomain);
        }
    }
}
