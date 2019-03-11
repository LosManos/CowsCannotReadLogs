using System;
using System.Collections.Generic;
using System.Linq;

namespace CowsCannotReadLogs.Client.Wpf
{
    internal interface ICompiler {
        TryCompileResult TryCompile(
           string recogniserFunctionSourceCode,
           string parserFunctionSourceCode);
   }

    internal class Compiler : ICompiler
    {
        private const string CodeTemplate = @"
        namespace MyNameSpace
        {
            using System;
            using System.Collections.Generic;
            using System.Linq;

public class MyClass
            {
                public Func<string,bool> RecogniserFunction{
                    get{
                            $RecogniserFunction$
                        }
                    }
                public Func<string,IEnumerable<string>> ParserFunction{
                    get{
                        $ParserFunction$
                    }
                }
            }
        }
        ";

        #region Constructors.

        internal static Compiler Create()
        {
            return new Compiler();
        }

        private Compiler() { }

        #endregion

        /// <summary>This methos complies two code snippets.
        /// They are inserted into a full class, which is compiled, and then pulled out of it as lambdas.
        /// </summary>
        /// <param name="recogniserFunctionSourceCode"></param>
        /// <param name="parserFunctionSourceCode"></param>
        /// <returns></returns>
        internal TryCompileResult TryCompile(
            string recogniserFunctionSourceCode,
            string parserFunctionSourceCode)
        {
            var snippetCompiler = new SourceCodeHandling.SnippetCompiler(
                new SourceCodeHandling.SnippetCompiler.OptionsStruct(throwExceptionForCompilationErrors: false));
            snippetCompiler.Compile(
                CreateCodeToCompile(recogniserFunctionSourceCode, parserFunctionSourceCode),
                "MyNameSpace",
                "MyClass");
            if (snippetCompiler.CompilerErrors.Any())
            {
                return TryCompileResult.CreateFailing(
                    snippetCompiler.CompilerErrors.Select(e => e.ErrorText));
            }
            else
            {
                return TryCompileResult.CreateSuccess(
                    snippetCompiler.GetProperty<Func<string, bool>>("RecogniserFunction"),
                    snippetCompiler.GetProperty<Func<string, IEnumerable<string>>>("ParserFunction")
                    );
            }
        }

        internal static string CreateCodeToCompile(string recogniserFunctionSourceCode, string parserFunctionSourceCode)
        {
            return CodeTemplate
                    .Replace("$RecogniserFunction$", recogniserFunctionSourceCode)
                    .Replace("$ParserFunction$", parserFunctionSourceCode);
        }

        /// <summary>This method exists only for making it possible to mock for making it possible to unit test.
        /// </summary>
        /// <param name="recogniserFunctionSourceCode"></param>
        /// <param name="parserFunctionSourceCode"></param>
        /// <returns></returns>
        TryCompileResult ICompiler.TryCompile(string recogniserFunctionSourceCode, string parserFunctionSourceCode)
        {
            return TryCompile(recogniserFunctionSourceCode, parserFunctionSourceCode);
        }
    }

    internal struct TryCompileResult
    {
        internal bool Result { get; private set; }
        internal Func<string, bool> RecogniserFunction { get; private set; }
        internal Func<string, IEnumerable<string>> ParserFunction { get; private set; }
        internal IEnumerable<string> CompilerErrors { get; private set; }
        private static TryCompileResult Create(bool result, Func<string, bool> recogniserFunction, Func<string, IEnumerable<string>> parserFunction, IEnumerable<string> compilerErrors)
        {
            return new TryCompileResult
            {
                Result = result,
                ParserFunction = parserFunction,
                RecogniserFunction = recogniserFunction,
                CompilerErrors = compilerErrors
            };
        }

        internal static TryCompileResult CreateFailing(IEnumerable<string> compilerErrors)
        {
            return Create(
                false,
                null,
                null,
                compilerErrors);
        }

        internal static TryCompileResult CreateSuccess(Func<string, bool> recogniserFunction, Func<string, IEnumerable<string>> parserFunction)
        {
            return Create(
                true,
                recogniserFunction,
                parserFunction,
                null);
        }
    }
}
