using System;
using FixiePlugin.TestRun;

namespace RemoteTestRunner
{
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