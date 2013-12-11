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

    public interface IException
    {
        string Type { get; }
        string Message { get; }
        string StackTrace { get; }
    }
}