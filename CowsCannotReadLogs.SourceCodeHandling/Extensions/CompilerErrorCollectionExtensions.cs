using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace CowsCannotReadLogs.SourceCodeHandling.Extensions
{
    internal static class CompilerErrorCollectionExtensions
    {
        internal static IEnumerable<CompilerError> ToIEnumerable(this CompilerErrorCollection me)
        {
            foreach( var ce in me)
            {
                yield return (CompilerError)ce;
            }
        }
    }
}
