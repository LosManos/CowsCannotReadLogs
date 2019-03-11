using RecogniserFunction = System.Func<string, bool>;
using ParserFunction = System.Func<string, System.Collections.Generic.IEnumerable<string>>;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace CowsCannotReadLogs.TextReading.UnitTest
{
    partial class TextReaderTest
    {
        private static readonly object[] TwoDifferentRowTypesWithMultiRowGroups =
            new object[] {
                "Two different row types with multi row groups.",
                new[]{
                    new TextReader.RowConverter(
                        "Error message",
                        new RecogniserFunction((s)=>Regex.IsMatch(s, @"\d{8} \d{4} error")),
                        new ParserFunction((s) => Regex.Match(s,@"(\d{8}) (\d{4}) (\w.+?) (.*)").Groups.Skip(1).Select(g=>g.Value))
                    ),
                    new TextReader.RowConverter(
                        "Information message",
                        new RecogniserFunction((s)=>Regex.IsMatch(s, @"\d{8} \d{4} information")),
                        new ParserFunction((s) => Regex.Match(s,@"(\d{8}) (\d{4}) (\w.+?) (.*)").Groups.Skip(1).Select(g=>g.Value))
                    )
                },
                new[]{
                    "20190311 2014 information amessage",
                    "20190311 2015 error mymessage",
                    "  another row",
                    "20190311 2025 information anothermessage",
                    "   Some more information."}
            };

        private static IEnumerable<object[]> CanReadRowsTestData
        {
            get
            {
                yield return new object[] {
                    "Simple row",
                    new[]{
                        new TextReader.RowConverter(
                            "A row",
                            new RecogniserFunction((_)=>true),
                            new ParserFunction((s) => Regex.Match(s,@"(\d{8}) (\d{4}) (\w.+?) (.*)").Groups.Skip(1).Select(g=>g.Value))
                        )
                    },
                    new[]{
                        "20190311 2014 aseverity amessage",
                        "20190311 2015 myseverity mymessage",
                        "20190311 2025 anotherseverity anothermessage" },
                    new[]{
                        new Expected.Group(
                        new Expected.Row("20190311", "2014", "aseverity", "amessage" ) ),
                        new Expected.Group(new Expected.Row("20190311", "2015", "myseverity", "mymessage" ) ),
                        new Expected.Group( new Expected.Row ( "20190311", "2025", "anotherseverity", "anothermessage" ) ) }
                };
                
                yield return new object[] {
                    "Group with two rows.",
                    new[]{
                        new TextReader.RowConverter(
                            "A row",
                            new RecogniserFunction((s)=>Regex.IsMatch(s, @"\d{8} \d{4} ")),
                            new ParserFunction((s) => Regex.Match(s,@"(\d{8}) (\d{4}) (\w.+?) (.*)").Groups.Skip(1).Select(g=>g.Value))
                        )
                    },
                    new[]{
                        "20190311 2014 aseverity amessage",
                        "20190311 2015 myseverity mymessage",
                        "  another row",
                        "20190311 2025 anotherseverity anothermessage" },
                    new[]{
                        new Expected.Group(
                            new Expected.Row ( "20190311", "2014", "aseverity", "amessage" ) ),
                        new Expected.Group(
                            new Expected.Row("20190311", "2015", "myseverity", "mymessage" ),
                            new Expected.Row("  another row" ) ),
                        new Expected.Group(
                            new Expected.Row ( "20190311", "2025", "anotherseverity", "anothermessage" ) ) }
                };
                
                yield return TwoDifferentRowTypesWithMultiRowGroups
                    .AppendExpected(
                        new Expected.Group(
                            new Expected.Row("20190311", "2014", "information", "amessage")),
                        new Expected.Group(
                            new Expected.Row("20190311", "2015", "error", "mymessage"),
                            new Expected.Row("  another row")),
                        new Expected.Group(
                            new Expected.Row("20190311", "2025", "information", "anothermessage"),
                            new Expected.Row("   Some more information."))
                    );
            }
        }

        private static IEnumerable<object[]> CanCreateStatisticsTestData
        {
            get
            {
                yield return TwoDifferentRowTypesWithMultiRowGroups
                    .AppendExpected(
                        new Expected.Statistic(name: "Error message", count: 1),
                        new Expected.Statistic(name: "Information message", count: 2),
                        new Expected.Statistic(name:string.Empty, count:2)
                    );
            }
        }
    }
}
