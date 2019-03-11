using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using CowsCannotReadLogs.SourceCodeHandling.Extensions;

namespace CowsCannotReadLogs.SourceCodeHandling
{
    public class SnippetCompiler
    {
        private object _instance;

        public OptionsStruct Options { get; set; }

        /// <summary>This property returns a list of <see cref="CompilerError"/> which requires the caller of <see cref="SourceCodeHandling"/> to reference <see cref="System.CodeDom.Compiler"/> which can be considered not good.
        /// </summary>
        public IEnumerable<CompilerError> CompilerErrors { get; private set; } = new List<CompilerError>();

        public SnippetCompiler()
        :this(new OptionsStruct(throwExceptionForCompilationErrors:true))
        {
        }

        public SnippetCompiler(OptionsStruct options)
        {
            Options = options;
        }

        public MethodResultType CallMethod<MethodResultType>(string methodName)
        {
            var methodInfo = _instance.GetType().GetMethod(methodName);
            var result = methodInfo.Invoke(_instance, null);

            return (MethodResultType)result;
        }

        public void Compile(string sourceCode, string namespaceName, string className)
        {
            // https://stackoverflow.com/questions/137933/what-is-the-best-scripting-language-to-embed-in-a-c-sharp-desktop-application/596097?sfb=2#596097        
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
            };

            // Add the dll itself. Then add system.core for Linq. A better solution should be created to choose what assemblies to load.
            options.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            options.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Core.dll");

            var compileResults = provider.CompileAssemblyFromSource(
                options,
                new[] { sourceCode });

            if (compileResults.Errors.Count >= 1)
            {
                if (Options.ThrowExceptionForCompilationErrors)
                {
                    throw new CompilationException(compileResults.Errors);
                }
                else
                {
                    CompilerErrors = compileResults.Errors.ToIEnumerable();
                    return;
                }
            }

            _instance = compileResults.CompiledAssembly.CreateInstance(namespaceName + "." + className);
        }

        public FieldResultType GetField<FieldResultType>(string fieldName)
        {
            var fieldInfo = _instance.GetType().GetField(fieldName);
            var result = _instance.GetType().GetField(fieldName).GetValue(_instance);

            return (FieldResultType)result;
        }

        //public object GetMethod(string methodName)
        //{
        //    //var methodInfo = _instance.GetType().GetMethod(methodName);
        //    var result = _instance.GetType().GetMethod(methodName);
        //    return result;
        //}

        public PropertyResultType GetProperty<PropertyResultType>(string propertyName)
        {
            var propertyInfo = _instance.GetType().GetProperty(propertyName);
            var result = _instance.GetType().GetProperty(propertyName).GetValue(_instance);

            return (PropertyResultType)result;
        }

        public struct OptionsStruct
        {
            public bool ThrowExceptionForCompilationErrors { get; }
            public OptionsStruct(bool throwExceptionForCompilationErrors)
            {
                ThrowExceptionForCompilationErrors = throwExceptionForCompilationErrors;
            }
        }
    }
}
