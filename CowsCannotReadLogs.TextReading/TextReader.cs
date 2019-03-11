using System.Collections.Generic;
using System.Linq;

namespace CowsCannotReadLogs.TextReading
{
    public partial class TextReader
    {
        #region Object state (fields and properties).

        private readonly RowConverter[] _rowConverters;

        #endregion

        #region Constructors.

        public TextReader(RowConverter[] rowConverters)
        {
            _rowConverters = rowConverters;
        }

        #endregion

        #region Public methods.

        /// <summary>This method returns statistics about the data.
        /// </summary>
        /// <param name="indataRows"></param>
        /// <returns></returns>
        public RowStatistics CreateStatistics(IEnumerable<string> indataRows)
        {
            var res = new Dictionary<string, long>();
            long rowCount = 0;
            foreach (var row in indataRows)
            {
                ++rowCount;
                var rowConverter = FindMatchingRowConverter(row);
                var name = rowConverter.Name ?? string.Empty;
                if (res.ContainsKey(name)){
                    res[name] += 1;
                }
                else
                {
                    res.Add(name, 1);
                }
            }
            return new RowStatistics(res.Select(r => (r.Key, r.Value)));
        }

        /// <summary>This method returns a list the rows in the data.
        /// Well... it does not return a list of rows really.
        /// It returns a list of Groups.
        /// Each such Group contains Rows.
        /// Each such Row contains Words.
        /// Each Group is yielded so groups are returned as they are discovered.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public IEnumerable<Group> ReadRows(IEnumerable<string> rows)
        {
            var group = new Group();

            var enumerator = rows.GetEnumerator();
            enumerator.MoveNext();
            var row = enumerator.Current;

            var lastUsedFunctionPair = FindMatchingRowConverter(row);
            group.Add(new Row( lastUsedFunctionPair.ParserFunction(row)));

            while (enumerator.MoveNext())
            {
                row = enumerator.Current;
                var rowConverter = FindMatchingRowConverter(row);
                if (string.IsNullOrEmpty( rowConverter.Name))
                {
                    group.Add(new Row( row ));
                }
                else
                {
                    yield return group;
                    group.Clear();
                    group.Add(new Row( rowConverter.ParserFunction(row)));
                }
            }
            yield return group;
        }

        #endregion

        #region Helper methods.

        private RowConverter FindMatchingRowConverter(string row)
        {
            var res = _rowConverters.FirstOrDefault(f => f.RecogniserFunction(row));
            return res;
        }

        #endregion
    }
}
