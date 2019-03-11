using CowsCannotReadLogs.Client.Wpf.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CowsCannotReadLogs.Client.Wpf
{
    public partial class MainWindow : Window
    {
        private const string DataPathFile = @"..\..\Settings.xml";

        public ViewModel VM { get; set; } = new ViewModel();

        public MainWindow()
        {
            VM.MainText = "TODO:Too early.";
            DataContext = VM;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fileHandling = new FileHandling.FileHandling();
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestData\Log.log");
            var rows = fileHandling.Read(path);

            var textReading = new TextReading.TextReader(new[]{new TextReading.TextReader.RowConverter(
                "normal",
                new Func<string, bool>((s) => true),
                new Func<string, IEnumerable<string>>((s) =>
                {
                    var items = s.Split(' ');
                    return new[]
                    {
                        items.First(),
                        items.Skip(1).First(),
                        items.Skip(2).First(),
                        string.Join(" ", items.Skip(3))
                    };
                }
            ))});
            var groups = textReading.ReadRows(rows);

            foreach (var group in groups.SelectWithIndex())
            {
                gridMain.AddRow(group.Index, group.Item);
            }
            gridMain.SetWidths();

            VM.MainText = "TODO:Change to MVVM" + Environment.NewLine + string.Join(Environment.NewLine, rows);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                App.Log.LogInformation("Escape was pressed.");
                ExitApplication();
            }
            //if( 
            //    ((e.Key == Key.F10 || e.Key == Key.System) && Keyboard.Modifiers == ModifierKeys.Shift) || // shift-f10
            //     (e.Key == Key.Apps)) // Properties button.
            //{
            //    MessageBox.Show("wow");
            //}
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication();
        }

        private void ExitApplication()
        {
            App.Log.LogInformation("Exiting application.");
            Application.Current.Shutdown();
        }

        private void MenuItemRecogniserFunction_Click(object sender, RoutedEventArgs e)
        {
            var fileHandling = new FileHandling.FileHandling();
            var compiler = Compiler.Create();

            var dlg = new RowConverterWindow(compiler, fileHandling);
            if(dlg.ShowDialog() ?? false)
            {
                // Temporary solution to... to.... visualise.
                MessageBox.Show("Success...." + dlg.Result.RecogniserFunction("x") + ".");
            }
        }

        public class ViewModel : ViewModelBase
        {
            private string _mainText;

            public string MainText
            {
                get => _mainText;
                set => SetProperty(ref _mainText, value);
            }
        }
    }
}
