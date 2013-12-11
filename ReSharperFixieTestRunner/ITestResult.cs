using System;

namespace ReSharperFixieTestRunner
{
    public interface ITestResult
    {
        bool Pass { get; }
        TimeSpan Duration { get; }
        string Output { get; }
        IException[] Exceptions { get; }
    }
}