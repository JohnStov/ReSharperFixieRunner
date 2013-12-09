using System;

namespace ReSharperFixieTestRunner
{
    public class TestResult : MarshalByRefObject, ITestResult
    {
        public bool Pass { get; set; }
        public TimeSpan Duration { get; set; }
        public string Output { get; set; }
        public string StackTrace { get; set; }
    }
}