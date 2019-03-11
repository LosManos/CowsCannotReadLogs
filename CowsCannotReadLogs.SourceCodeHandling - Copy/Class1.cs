using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CowsCannotReadLogs.SourceCodeHandling
{
    public class Class1
    {
        public void Create(string code)
        {
            //var tree = SyntaxFactory.ParseCompilationUnit(code);

            //IEnumerable<MethodDeclarationSyntax> methods = tree
            //.DescendantNodes()
            //.OfType<MethodDeclarationSyntax>();

            //var msgMethod = methods.Single(m => m.Identifier.Text == "Msg");

            // https://stackoverflow.com/questions/137933/what-is-the-best-scripting-language-to-embed-in-a-c-sharp-desktop-application/596097?sfb=2#596097        
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
            };
            options.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            var compileResults = provider.CompileAssemblyFromSource(
                options,
                new[] { code });
            

            //var prov = CSharpCodeProvider.CreateProvider("CSharp");
            //var compilerResults = prov.CompileAssemblyFromSource(code);

            //throw new NotImplementedException(string.Join(",", methods.Select(m => m.Identifier)));

            throw new NotImplementedException("TBA");
        }
    }
}
