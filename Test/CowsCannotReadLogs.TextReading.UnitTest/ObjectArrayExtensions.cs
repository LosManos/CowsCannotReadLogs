using System.Linq;

namespace CowsCannotReadLogs.TextReading.UnitTest
{
    internal static class ObjectArrayExtensions
    {
        internal static object[] AppendExpected(this object[] me, params Expected.Group[] groups)
        {
            return me.Append(groups).ToArray();
        }

        internal static object[] AppendExpected(this object[] me, params Expected.Statistic[] statistics)
        {
            return me.Append(statistics).ToArray();
        }
    }
}
