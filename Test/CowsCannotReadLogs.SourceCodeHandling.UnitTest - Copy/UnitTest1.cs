using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CowsCannotReadLogs.SourceCodeHandling.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var input = @"l
namespace
{
    public class MyClass
    {
        public string Msg(){
            return ""msg"";
        ]
    }
}
";
            var sut = new CowsCannotReadLogs.SourceCodeHandling.Class1();

            sut.Create(input);
        }

//        [TestMethod]
//        public void TestMethod2()
//        {
//            var input = @"l
//namespace
//{
//    public class MyClass
//    {
//        public string Msg(){
//            return ""msg"";
//        ]
//    }
//}
//";
//            var sut = new CowsCannotReadLogs.SourceCodeHandling.Class2();

//            sut.Method1(input);
//        }
    }
}
