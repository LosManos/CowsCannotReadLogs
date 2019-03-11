using Microsoft.VisualStudio.TestTools.UnitTesting; 
using System;
using FluentAssertions;
using System.Linq;

namespace CowsCannotReadLogs.SourceCodeHandling.UnitTest
{
    [TestClass]
    public class SnippetCompilerTest
    {
        [TestMethod]
        public void CanCallSimpleMethodWithReturnValue()
        {
            //  #   Arrange.
            var input = @"
namespace MyNameSpace
{
    public class MyClass
    {
        public string Msg(){
            return ""msg"";
        }
    }
}
";
            var sut = new SnippetCompiler();
            sut.Compile(input, "MyNameSpace", "MyClass");

            //  #   Act.
            var res = sut.CallMethod<string>("Msg");

            //  #   Arrange.
            Assert.AreEqual("msg", res);
        }

        [TestMethod]
        public void CanRetrieveLambdaField()
        {
            //  #   Arrange.
            var input = @"
namespace MyNameSpace
{
    using System;
    public class MyClass
    {
        public Func<string, string> MyLambdaFunctionField = ((s) => ""[-"" + s + ""-]"");
    }
}
";
            var sut = new SnippetCompiler();
            sut.Compile(input, "MyNameSpace", "MyClass");

            //  #   Act.
            var res = sut.GetField<Func<string,string>>("MyLambdaFunctionField");

            //  #   Assert.
            Assert.AreEqual("[-Success!-]", res("Success!"));
        }

        [TestMethod]
        public void CanRetrieveLambdaProperty()
        {
            //  #   Arrange.
            var input = @"
namespace MyNameSpace
{
    using System;
    public class MyClass
    {
        public Func<string, string> MyLambdaFunctionProperty { 
            get { return new Func<string, string>((s) => ""[-"" + s + ""-]""); } 
            private set {}
        }
    }
}
";
            var sut = new SnippetCompiler();
            sut.Compile(input, "MyNameSpace", "MyClass");

            //  #   Act.
            var res = sut.GetProperty<Func<string,string>>("MyLambdaFunctionProperty");

            //  #   Assert.
            Assert.AreEqual("[-Success!-]", res("Success!"));
        }

//        [TestMethod]
//        public void CanRetrieveMethod()
//        {
//            //  #   Arrange.
//            var input = @"
//namespace MyNameSpace
//{
//    using System;
//    public class MyClass
//    {
//        public string MyMethod( string argument ){
//            return ""[-"" + argument + ""-]"";
//        }
//    }
//}
//";
//            var sut = new SnippetCompiler();
//            sut.Create(input, "MyNameSpace", "MyClass");

//            //  #   Act.
//            var res = sut.GetMethod("MyMethod");

//            //  #   Assert.
//            Assert.AreEqual("[-Success!-]", ((Func<string, string>)res)("Success!"));
//        }

        [TestMethod]
        public void CanSetWhetherToThrowExceptionWhenCompilationFails()
        {
            //  #   Arrange.
            var sut = new SnippetCompiler();
            Action action = () => sut.Compile("this string does not compile", "whatevernamespace", "whateverclassname");

            //  #   Act.
            //  #   Assert.
            action.Should().Throw<CompilationException>();

            //  #   Arrange.
            sut.Options = new SnippetCompiler.OptionsStruct(
                throwExceptionForCompilationErrors : false
            );

            //  #   Act.
            action();

            //  #   Assert.
            sut.CompilerErrors.Any().Should().Be(true, "We should have compilation errors but no exception.");
        }

        [TestMethod]
        public void ThrowsCompilationExceptionWithContent()
        {
            //  #   Arrange.
            var sut = new SnippetCompiler();
            Action action = () => sut.Compile("this string does not compile", "whatevernamespace", "whateverclassname");

            //  #   Act.
            //  #   Assert.
            action.Should().Throw<CompilationException>()
                .Where(exc => exc.CompilerErrors.Count() == 1);
        }

        /// <summary>This class is for testing out the syntax with VS's compiler.
        /// </summary>
        public class MyClass
        {
            public string Msg()
            {
                return "msg";
            }
            public Func<string, string> MyLambdaFunctionField = ((s) => "[-" + s + " -]");
            public Func<string, string> MyLambdaFunctionProperty
            {
                get { return new Func<string, string>((s) => "[-" + s + " -]"); }
                private set { }
            }
        }

    }
}
