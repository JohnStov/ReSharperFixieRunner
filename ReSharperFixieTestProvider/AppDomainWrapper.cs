using System;

namespace ReSharperFixieTestProvider
{
    public class AppDomainWrapper : IDisposable
    {
        private readonly AppDomain appDomain;

        public AppDomainWrapper(string domainName = null)
        {
            if (domainName == null)
                domainName = Guid.NewGuid().ToString();

            appDomain = CreateAppDomain(domainName);
        }

        AppDomain CreateAppDomain(string domainName)
        {
            return AppDomain.CreateDomain(domainName);
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
