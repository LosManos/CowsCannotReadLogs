using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using CowsCannotReadLogs.SourceCodeHandling.Extensions;

namespace CowsCannotReadLogs.SourceCodeHandling
{

    [Serializable]
    public class CompilationException : Exception, ISourceCodeHandlingException
    {
        /// <summary>This property returns a list of <see cref="CompilerError"/> which requires the caller of <see cref="SourceCodeHandling"/> to reference <see cref="System.CodeDom.Compiler"/> which can be considered not good.
        /// </summary>
        public IEnumerable<CompilerError> CompilerErrors { get; private set; }

        public CompilationException() { }
        public CompilationException(string message) : base(message) { }
        public CompilationException(string message, Exception inner) : base(message, inner) { }
        protected CompilationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        /// <summary>This is the preferred constructor since it takes all parameters.
        /// </summary>
        /// <param name="compilerErrors"></param>
        internal CompilationException(CompilerErrorCollection compilerErrors)
            :this("Compilation error.")
        {
            CompilerErrors = compilerErrors.ToIEnumerable();
            //var localCompilerErrors = new List<CompilerError>(compilerErrors.Count);
            //foreach(var compilerError in compilerErrors)
            //{
            //    localCompilerErrors.Add((CompilerError)compilerError);
            //}
            //CompilerErrors = localCompilerErrors;
        }
    }
}
