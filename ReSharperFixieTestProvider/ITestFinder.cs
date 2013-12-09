namespace ReSharperFixieTestProvider
{
    public interface ITestFinder
    {
        FixieConventionInfo FindTests(string testAssemblyPath);
    }

}
