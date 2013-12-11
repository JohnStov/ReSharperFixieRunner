using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Fixie;
using ReSharperFixieTestRunner;

namespace FixieRemoteRunner
{
    public class ReSharperListener : Listener
    {
        private DateTimeOffset start;

        private readonly TestResult testResult = new TestResult();

        public ITestResult TestResult
        {
            get
            {
                return testResult;
            }
        }

        public void AssemblyStarted(Assembly assembly)
        {
            start = DateTimeOffset.Now;
        }

        public void CasePassed(PassResult result)
        {
            testResult.Pass = true;
            testResult.Output = result.Output;
        }

        public void CaseFailed(FailResult result)
        {
            testResult.Pass = false;
            testResult.Output = result.Output;
            testResult.Exceptions = result.Exceptions.Select(x => new ExceptionInfo(x)).Cast<IException>().ToArray();
        }

        public void AssemblyCompleted(Assembly assembly, AssemblyResult result)
        {
            testResult.Duration = DateTimeOffset.Now - start;
        }


        private static string WriteStackTrace(IEnumerable<Exception> exceptions)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteCompoundStackTrace(exceptions);
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}