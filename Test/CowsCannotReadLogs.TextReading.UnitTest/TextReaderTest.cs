using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CowsCannotReadLogs.TextReading.UnitTest
{
    [TestClass]
    public partial class TextReaderTest
    {
        [DataTestMethod]
        [DynamicData(nameof(CanReadRowsTestData))]
        public void CanReadRows(
            string testName,
            TextReader.RowConverter[] rowConverters,
            IEnumerable<string> indataRows,
            Expected.Group[] expectedResult)
        {
            var sut = new TextReader(rowConverters);

            var res = sut.ReadRows(indataRows);

            var actualEnumerator = res.GetEnumerator();
            foreach (var expectedGroup in expectedResult)
            {
                actualEnumerator.MoveNext();
                var actualRows = actualEnumerator.Current;
                actualRows.Should().BeEquivalentTo(expectedGroup.Rows.Select(r=>r.Words), $"TestName:{testName}");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(CanCreateStatisticsTestData))]
        public void CanCreateStatistics(
            string testName,
            TextReader.RowConverter[] rowConverters,
            IEnumerable<string> indataRows,
            Expected.Statistic[] expectedResult)
        {
            var sut = new TextReader(rowConverters);

            var res = sut.CreateStatistics(indataRows);

            foreach (var exp in expectedResult)
            {
                res.NumberOf(exp.Name).Should().Be(exp.Count);
            }
        }
    }
}
