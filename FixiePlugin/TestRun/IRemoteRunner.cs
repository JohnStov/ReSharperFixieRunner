namespace FixiePlugin.TestRun
{
    public interface IRemoteRunner
    {
        ITestResult RunTest(TestSetup setup);
    }
}