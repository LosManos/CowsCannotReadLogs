using System.Collections.Generic;

namespace CowsCannotReadLogs.FileHandling.UnitTest
{
    public partial class FileHandlingTest
    {
        private static IEnumerable<object[]> ReadDataReturnsStructureTestData
        {
            get
            {
                yield return new object[] {
@"<Data Version=""2"">
    <Item>
        <Name>First</Name>
        <RecogniserFunction>rec func</RecogniserFunction>
        <ParserFunction>par func</ParserFunction>
    </Item>
</Data>",
                    FileHandling.Data.Create(
                        version : 2,
                        items : new[]{
                            FileHandling.Data.Item.Create("First", "rec func", "par func")}
                        )
                };
            }
        }
    }
}
