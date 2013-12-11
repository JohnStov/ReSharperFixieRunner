using System;
using FixiePlugin.TestRun;

namespace RemoteTestRunner
{
    public class TestResult : MarshalByRefObject, ITestResult
    {
        public bool Pass { get; set; }
        public TimeSpan Duration { get; set; }
        public string Output { get; set; }
        public IException[] Exceptions { get; set; }
    }
}