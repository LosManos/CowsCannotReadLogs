using System.Collections.Generic;

namespace CowsCannotReadLogs.TextReading.UnitTest
{
    public class Expected
    {
        public struct Row
        {
            public string[] Words { get; }
            public Row(params string[] words)
            {
                Words = words;
            }
        }

        public struct Group
        {
            public Row[] Rows { get; }
            public Group(params Row[] rows)
            {
                Rows = rows;
            }
        }

        private readonly List<Row> rows = new List<Row>();

        Expected WithRow( params string[] words)
        {
            rows.Add(new Row(words));
            return this;
        }

        public struct Statistic
        {
            public Statistic(string name, long count)
            {
                Name = name;
                Count = count;
            }

            public string Name { get; }
            public long Count { get; }
        }
    }
}
