using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Windows;

namespace CowsCannotReadLogs.Client.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static ILogger<MainWindow> Log;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyExceptionHandler);

            var loggerFactory = new LoggerFactory();
            var loggerConfig = new LoggerConfiguration()
                //.WriteTo.Console()
                .WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            loggerFactory.AddSerilog(loggerConfig);
            Log = loggerFactory.CreateLogger<MainWindow>();
            
            Log.LogInformation("Starting application.");
        }

        private void MyExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // I know there is a problem here. What if _log is not set?
            Log.LogError(((Exception)e.ExceptionObject), "Unhandled exception");
            Application.Current.Shutdown();
        }
    }
}
