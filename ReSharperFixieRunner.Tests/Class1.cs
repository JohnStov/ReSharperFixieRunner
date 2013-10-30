using JetBrains.Metadata.Reader.API;
using JetBrains.Util;
using NSubstitute;
using ReSharperFixieRunner.UnitTestProvider;
using Xunit;

namespace ReSharperFixieRunner.Tests
{
    public class FixieTestMetadataExplorerTest
    {
        [Fact]
        public void CreateExplorer()
        {
            var explorer = new FixieTestMetadataExplorer(null, null, null);

            var assembly = Substitute.For<IMetadataAssembly>();
            assembly.Location.Returns(FileSystemPath.Parse(@"C:\Users\John\Documents\GitHub\ReSharperFixieRunner\FixieTestExample\FixieTestExample\bin\Debug\FixieTestExample.dll"));

            explorer.ExploreAssembly(null, assembly, null, null);

        }
    }
}
