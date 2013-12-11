using System;

namespace ReSharperFixieTestRunner
{
    public class TestResult : MarshalByRefObject, ITestResult
    {
        public bool Pass { get; set; }
        public TimeSpan Duration { get; set; }
        public string Output { get; set; }
        public IException[] Exceptions { get; set; }
    }

    public class ExceptionInfo : MarshalByRefObject, IException
    {
        public ExceptionInfo(Exception ex)
        {
            Type = ex.GetType().FullName;
            Message = ex.Message;
            StackTrace = ex.StackTrace;
        }

        public string Type { get; private set; }
        public string Message { get; private set; }
        public string StackTrace { get; private set; }
    }
}