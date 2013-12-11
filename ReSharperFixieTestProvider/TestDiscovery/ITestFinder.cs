namespace FixiePlugin.TestDiscovery
{
    public interface ITestFinder
    {
        ConventionInfo FindTests(string testAssemblyPath);
    }
}
