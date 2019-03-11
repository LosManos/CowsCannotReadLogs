using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace CowsCannotReadLogs.FileHandling.UnitTest
{
    [TestClass]
    public partial class FileHandlingTest
    {
        [TestMethod]
        public void ReadReturnsAllRows()
        {
            //  #   Arrange.
            const string PathFile = @"c:\whatever.log";
            var mockedFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { PathFile,
@"20190318 1916 information System started
20190318 1917 warning Hard drive low on space"}
                });

            var sut = new FileHandling(mockedFileSystem);

            //  #   Act.
            var res = sut.Read(PathFile);

            //  #   Assert.
            res.Should().BeEquivalentTo(
                new[]
                {
                    "20190318 1916 information System started",
                    "20190318 1917 warning Hard drive low on space"
                });
        }

        [DataTestMethod]
        [DynamicData(nameof(ReadDataReturnsStructureTestData))]
        public void ReadDataReturnsStructure(string persistedXmlString, FileHandling.Data expectedResult)
        {
            //  #   Arrange.
            const string PathFile = @"C:\whatever.xml";
            var mockedFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { { PathFile, persistedXmlString } });

            var sut = new FileHandling(mockedFileSystem);

            //  #   Act.
            var res = sut.ReadData(PathFile);

            //  #   Assert.
            res.Should().BeEquivalentTo(expectedResult, o => o.ComparingByMembers<FileHandling.Data>());
        }

        [TestMethod]
        public void SaveIfNoFileExists()
        {
            //  #   Arrange.
            var fileSystem = new MockFileSystem();
            var data = FileHandling.Data.Create(
                0, // Will be updated.
                FileHandling.Data.Item.Create("name", "recogniserfunction", "parserfunction"));
            var sut = new FileHandling(fileSystem);
            const string PathFile = @"C:\what\ever.xml";

            //  #   Act.
            sut.Save(PathFile, data.Items);

            //  #   Assert.
            var persisted = sut.ReadData(PathFile);

            var expectedOutput = FileHandling.Data.Create(
                1,
                data.Items.Single()  );
            persisted.Should().BeEquivalentTo(expectedOutput, opt => opt.ComparingByMembers<FileHandling.Data>());
        }

        [TestMethod]
        public void SaveIfFileExists()
        {
            //  #   Arrange.
            var fileSystem = new MockFileSystem();
            var data = FileHandling.Data.Create(
                1, // Standard version at the time of writing.
                FileHandling.Data.Item.Create("name", "recogniserfunction", "parserfunction"));
            var sut = new FileHandling(fileSystem);
            //  Create file.
            const string PathFile = @"C:\what\ever.xml";
            sut.Save(PathFile, data.Items);

            //  #   Act.
            data = FileHandling.Data.Create(
                1,
                FileHandling.Data.Item.Create("name2", "recogniserfunction2", "parserfunction2"));
            sut.Save(PathFile, data.Items);

            //  #   Assert.
            var persisted = sut.ReadData(PathFile);
            persisted.Should().BeEquivalentTo(data, opt => opt.ComparingByMembers<FileHandling.Data>());
        }
    }
}
