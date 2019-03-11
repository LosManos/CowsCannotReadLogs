using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows;
using CowsCannotReadLogs.FileHandling;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TryCompileExpression = System.Linq.Expressions.Expression<System.Func<CowsCannotReadLogs.Client.Wpf.ICompiler, CowsCannotReadLogs.Client.Wpf.TryCompileResult>>;

namespace CowsCannotReadLogs.Client.Wpf.Test
{
    [TestClass]
    public class RowConverterWindowTest
    {
        [TestMethod]
        public void ButttonCompile_CompileCode()
        {
            var sut = new RowConverterWindow();
            sut.UT_BtnCompile_Click(null, new RoutedEventArgs());

            sut.UT_VM.CompilerMessage.Should().Contain("'MyNameSpace.MyClass.RecogniserFunction.get': not all code paths return a value");
        }

        [TestMethod]
        public void ButtonOk_SaveAndCloseWindowIfCodeCompiles()
        {
            //  #   Arrange.
            const string RecogniserSourceCode = "return new Func<string,bool>((s)=>false);";
            var RecogniserLambda = new Func<string, bool>((s) => false);
            const string ParserSourceCode = "return new Func<string,IEnumerable<string>>((s)=>null);";
            var ParserLambda = new Func<string, IEnumerable<string>>((s) => null);

            var compiler = new Mock<ICompiler>();
            TryCompileExpression tryCompile = c => c.TryCompile(RecogniserSourceCode, ParserSourceCode);
            compiler.Setup(tryCompile)
                .Returns(TryCompileResult.CreateSuccess(
                    RecogniserLambda,
                    ParserLambda));

            var fileHandling = new Mock<IFileHandling>();
            fileHandling.Setup(f => f.ReadData(It.IsAny<string>()))
                .Returns(FileHandling.FileHandling.Data.Create(1, new[]{
                    FileHandling.FileHandling.Data.Item.Create(
                        "any name",
                        RecogniserSourceCode,
                        ParserSourceCode) }));

            var sut = new RowConverterWindow(compiler.Object, fileHandling.Object);

            var ic = new InstrumentClass();
            sut.Instrument(ic);

            //  #   Action.
            sut.UT_BtnRowConverterWindowOk_Click(null, new RoutedEventArgs());

            //  #   Assert
            compiler.Verify(tryCompile, Times.Once);
            ic.DialogResultHasBeenSet.Should().BeTrue();
            ic.DialogResultHasBeenSetTo.Should().BeTrue();
            ic.CloseHasBeenCalled.Should().BeTrue();
        }

        [TestMethod]
        public void ButtonOk_NotCloseIfCodeDoesNotCompile()
        {
            //  #   Arrange.
            const string compilerErrors = "compiler error";

            var compiler = new Mock<ICompiler>();
            TryCompileExpression tryCompile = x => x.TryCompile(It.IsAny<string>(), It.IsAny<string>());
            compiler.Setup(tryCompile)
                .Returns(TryCompileResult.CreateFailing(new[] { compilerErrors }));
            var fileHandling = new Mock<FileHandling.IFileHandling>();
            fileHandling.Setup(f => f.ReadData(It.IsAny<string>()))
                .Returns(FileHandling.FileHandling.Data.Create(1, new[]{
                    FileHandling.FileHandling.Data.Item.Create(
                        "any name",
                        "whatever",
                        "whatever") }));

            var sut = new RowConverterWindow(compiler.Object, fileHandling.Object);
            var ic = new InstrumentClass();
            sut.Instrument(ic);

            //  #   Act.
            sut.UT_BtnRowConverterWindowOk_Click(null, null); // new RoutedEventArgs());

            //  #   Assert.
            compiler.VerifyAll();
            compiler.Verify(tryCompile, Times.Once);
            sut.UT_VM.CompilerMessage.Should().Contain(compilerErrors);
            ic.DialogResultHasBeenSet.Should().BeFalse();
            ic.CloseHasBeenCalled.Should().BeFalse();
        }
    }

    internal class InstrumentClass
    {
        internal bool CloseHasBeenCalled { get; set; }
        internal bool DialogResultHasBeenSet { get; set; }
        internal bool? DialogResultHasBeenSetTo { get; set; }
    }

    internal static class WindowExtension
    {
        internal static void Instrument(this RowConverterWindow window, InstrumentClass ic)
        {
            window.SetDialogResult = new Action<Window, bool?>((_, dialogResult) =>
            {
                ic.DialogResultHasBeenSet = true;
                ic.DialogResultHasBeenSetTo = dialogResult;
            });
            window.CallClose = new Action<Window>((_) =>
            {
                ic.CloseHasBeenCalled = true;
            });
        }
    }
}
