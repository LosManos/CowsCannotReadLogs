using CowsCannotReadLogs.Client.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CowsCannotReadLogs.Client.Wpf
{
    public partial class RowConverterWindow : Window
    {
        private ViewModel VM { get; set; } = new ViewModel();

        //private readonly Func<string, string, Compiler.TryCompileResult> tryCompileFunction;
        private readonly FileHandling.IFileHandling fileHandling;
        private readonly ICompiler compiler;

        //TODO:Set the data file's name in one and only one place. Also:This path does not work with a compiled solution.
        private const string DataPathFile = @"..\..\Settings.xml";

        internal string SourceCode { get; private set; }

        internal DialogueResult Result { get; private set; }

        #region Constructors.

        internal RowConverterWindow()
        {
            DataContext = VM;

            InitializeComponent();
        }

        internal RowConverterWindow(
            ICompiler compiler,
            FileHandling.IFileHandling fileHandling)
            : this()
        {
            var data = fileHandling.ReadData(DataPathFile);
            VM.Items = data.Items;
            VM.ActiveItem = VM.Items.FirstOrDefault();

            this.fileHandling = fileHandling;
            this.compiler = compiler;
        }

        #endregion

        private void BtnCompile_Click(object sender, RoutedEventArgs e)
        {
            var compiler = Compiler.Create();
            var result = compiler.TryCompile(VM.RecogniserSourceCode, VM.ParserSourceCode);
            if (result.Result)
            {
                VM.CompilerMessage = "Success." + Environment.NewLine + DateTime.Now.ToString("u");
            }
            else
            {
                VM.CompilerMessage =
                    string.Join(Environment.NewLine, result.CompilerErrors) +
                    Environment.NewLine +
                    Environment.NewLine +
                    Compiler.CreateCodeToCompile(VM.RecogniserSourceCode, VM.ParserSourceCode); }
        }

        private void BtnRowConverterWindowOk_Click(object sender, RoutedEventArgs e)
        {
            var compileResult = compiler.TryCompile(VM.RecogniserSourceCode, VM.ParserSourceCode);

            if (compileResult.Result)
            {
                Result = DialogueResult.Create
                (
                    compileResult.RecogniserFunction,
                    compileResult.ParserFunction
                );

                SaveScripts(fileHandling, VM.Items);

                //DialogResult = true;
                SetDialogResult(this, true);
                CallClose(this);
            }
            else
            {
                VM.CompilerMessage =
                    string.Join(Environment.NewLine, compileResult.CompilerErrors);
            }
        }

        /// <summary>This property is a method used for setting the window's DialogResult.
        /// It's raison d'etre is for making unit testing possible as we cannot set DialogResult in a unit test.
        /// The internal setter is only used for testing.
        /// If we try to we get a "DialogResult can be set only after Window is created and shown as dialog."
        /// </summary>
        internal Action<Window, bool?> SetDialogResult { private get; set; } = new Action<Window, bool?>((w, b) => w.DialogResult = b);

        internal Action<Window> CallClose { private get; set; } = new Action<Window>((w) => w.Close());

        private static void SaveScripts(FileHandling.IFileHandling fileHandling, IEnumerable<FileHandling.FileHandling.Data.Item> items)
        {
            fileHandling.Save(DataPathFile, items);
        }

        #region Methods and stuff for automatic tests.

        /// <summary>This proeprty exists only to make automatic testing possible.
        /// </summary>
        internal ViewModel UT_VM => VM;
        /// <summary>This proeprty exists only to make automatic testing possible.
        /// </summary>
        internal Action<object, RoutedEventArgs> UT_BtnCompile_Click => BtnCompile_Click;
        /// <summary>This proeprty exists only to make automatic testing possible.
        /// </summary>
        internal Action<object, RoutedEventArgs> UT_BtnRowConverterWindowOk_Click => BtnRowConverterWindowOk_Click;

        #endregion

        /// <summary>This class is the window's view model.
        /// The properties have to be public to make MVVM work.
        /// </summary>
        internal class ViewModel : ViewModelBase
        {
            // TODO: Move FileHanding stuff out of the ViewModel.
            private IEnumerable<FileHandling.FileHandling.Data.Item> _items;
            private string _recogniseSourceCode;
            private string _parseSourceCode;
            private string _compilerMessage;
            private FileHandling.FileHandling.Data.Item _activeItem;

            public IEnumerable<FileHandling.FileHandling.Data.Item> Items
            {
                get => _items;
                set => SetProperty(ref _items, value);
            }

            public FileHandling.FileHandling.Data.Item ActiveItem {
                get => _activeItem;
                set
                { 
                    SetProperty(ref _activeItem, value);
                    RecogniserSourceCode = value.RecogniserFunction;
                    ParserSourceCode = value.ParserFunction;
                }
            }

            public string RecogniserSourceCode
            {
                get => _recogniseSourceCode;
                set => SetProperty(ref _recogniseSourceCode, value);
            }

            public string ParserSourceCode
            {
                get => _parseSourceCode;
                set => SetProperty(ref _parseSourceCode, value);
            }

            public string CompilerMessage
            {
                get => _compilerMessage;
                set => SetProperty(ref _compilerMessage, value);
            }
        }

        internal struct DialogueResult
        {
            internal Func<string, bool> RecogniserFunction { get; private set; }

            internal Func<string, IEnumerable<string>> ParserFunction { get; private set; }

            private DialogueResult(
                Func<string, bool> recogniserFunction,
                Func<string, IEnumerable<string>> parserFunction)
            {
                RecogniserFunction = recogniserFunction;
                ParserFunction = parserFunction;
            }

            internal  static DialogueResult Create(
                Func<string, bool> recogniserFunction,
                Func<string, IEnumerable<string>> parserFunction)
            {
                return new DialogueResult(recogniserFunction, parserFunction);
            }
        }
    }
}
