using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Fixie;
using ReSharperFixieTestRunner;

namespace FixieRemoteRunner
{
    public class ReSharperListener : Listener
    {
        private DateTimeOffset start;
        private TestResult testResult = new TestResult();

        public ITestResult TestResult { get { return testResult; } }

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
            testResult.StackTrace = WriteStackTrace(result.Exceptions);
        }

        public void AssemblyCompleted(Assembly assembly, AssemblyResult result)
        {
            testResult.Duration = DateTimeOffset.Now - start;
        }


        private static string WriteStackTrace(IEnumerable<Exception> exceptions)
        {
            var builder = new StringBuilder();

            bool flag = true;
            foreach (var exception in exceptions)
            {
                if (flag)
                {
                    builder.AppendLine(exception.Message);
                    builder.AppendLine(exception.StackTrace);
                }
                else
                {
                    builder.AppendLine();
                    builder.AppendLine();
                    builder.AppendFormat("===== Secondary Exception: {0} =====", exception.GetType().FullName);
                    builder.AppendLine();
                    builder.AppendLine(exception.Message);
                    builder.Append(exception.StackTrace);
                }
                Exception innerException = exception;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                    builder.AppendLine();
                    builder.AppendLine();
                    builder.AppendFormat("------- Inner Exception: {0} -------", innerException.GetType().FullName);
                    builder.AppendLine();
                    builder.AppendLine(innerException.Message);
                    builder.Append(innerException.StackTrace);
                }
                flag = false;
            }

            return builder.ToString();
        }
    }
}