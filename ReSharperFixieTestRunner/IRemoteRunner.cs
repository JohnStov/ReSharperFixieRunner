namespace ReSharperFixieTestRunner
{
    public interface IRemoteRunner
    {
        ITestResult RunTest(TestSetup setup);
    }
}