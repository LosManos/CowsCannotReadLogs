using System.Collections;
using System.Collections.Generic;
using RecogniserFunction = System.Func<string, bool>;
using ParserFunction = System.Func<string, System.Collections.Generic.IEnumerable<string>>;
using System.Linq;

namespace CowsCannotReadLogs.TextReading
{
    partial class TextReader
    {
        /// <summary>This struct is a list of <see cref="Row"/>.
        /// </summary>
        public struct Group : IEnumerable<Row>
        {
            private List<Row> rowList;

            internal Group(params Row[] rows)
            {
                rowList = new List<Row>();
                foreach (var row in rows)
                {
                    rowList.Add(row);
                }
            }

            public IEnumerator<Row> GetEnumerator()
            {
                rowList = rowList ?? new List<Row>();
                return rowList.GetEnumerator();
            }

            internal void Add(Row row)
            {
                rowList = rowList ?? new List<Row>();
                rowList.Add(row);
            }

            internal void Clear()
            {
                rowList = rowList ?? new List<Row>();
                rowList.Clear();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>This struct is a list of Words (strings).
        /// </summary>
        public struct Row : IEnumerable<string>
        {
            private IList<string> wordList;

            internal Row(string word)
                : this(new[] { word })
            {
            }

            internal Row(string[] words)
            {
                wordList = new List<string>();
                foreach (var word in words)
                {
                    wordList.Add(word);
                }
            }

            internal Row(IEnumerable<string> words)
                : this(words.ToArray())
            {
            }

            public  IEnumerator<string> GetEnumerator()
            {
                wordList = wordList ?? new List<string>();
                return wordList.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>This struct contains 3 things.
        /// A Name to describe its purpose and differ it from other RowConverters.  A blank name is set to mean that thet RowConverter does not match any known Row.
        /// A RecogniserFunction that takes a string and returns a boolean wether it recognises it or not.
        /// A ParserFunction that takes a string and divides it into Words.
        /// </summary>
        public struct RowConverter
        {
            public RowConverter(string name, RecogniserFunction recogniserFunction, ParserFunction parserFunction)
            {
                Name = name;
                RecogniserFunction = recogniserFunction;
                ParserFunction = parserFunction;
            }

            public string Name { get; }
            public RecogniserFunction RecogniserFunction { get; }
            public ParserFunction ParserFunction { get; }
        }

        public struct RowStatistics
        {
            public RowStatistics(IEnumerable<(string RowConverterName, long Count)> statistics)
            {
                Statistics = statistics;
            }

            public IEnumerable<(string RowConverterName, long Count)> Statistics { get; }

            public long NumberOf(string rowConverterName)
            {
                return Statistics.Single(s => s.RowConverterName == rowConverterName).Count;
            }
        }
    }
}
