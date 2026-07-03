using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.UILogic.Classes;
using Euchre.UILogic.Interfaces;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Euchre;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(App));
    private static IHost? _host;


    /// <summary>
    /// Gets services.
    /// </summary>
    public static IServiceProvider Services => _host!.Services;

    // Source - https://stackoverflow.com/a/65626626
    // Posted by Mr. Squirrel.Downy
    // Retrieved 2026-07-01, License - CC BY-SA 4.0
    /// <summary>
    /// Gets or sets the global font size for the application, which can be used for scaling UI elements.
    /// </summary>
    public static double GlobalFontSize
    {
        get => (double)Current.Resources["GlobalFontSize"];
        set => Current.Resources["GlobalFontSize"] = value;
    }

    /// <summary>
    /// A dictionary mapping font size names to their corresponding numeric values, used for setting the 
    /// global font size in the application.
    /// </summary>
    private static readonly Dictionary<string, int> PossibleFontSizes = new()
    {
        { "Small", 11 },
        { "Medium", 15 },
        { "Large", 18 },
    };


    protected async override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Set the log directory environment variable BEFORE setting up log4net
        // This allows the log4net config to use environment variables for the path
        var logDirectory = GameSettingsManager.Instance.GetSettingsDirectory();
        Environment.SetEnvironmentVariable("EUCHRE_LOG_DIR", logDirectory, EnvironmentVariableTarget.Process);

        SetupLog4Net();
        AddGlobalExceptionHandlers();

        // Start application services.

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Singleton services.

                services.AddSingleton<IGameStateManager, GameStateManager>();
                services.AddSingleton<IMainWindowController, MainWindowController>();
                services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        // Set the global font size based on the user's settings.

        GlobalFontSize = PossibleFontSizes[GameSettingsManager.Instance.FontSize];

        // Start the host asynchronously to ensure all services are initialized before showing the main window.

        await _host.StartAsync();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// Configures log4net logging for the application using the 'log4net.config' file if it is present in 
    /// the application's base directory. The log file path is set via the EUCHRE_LOG_DIR environment variable.
    /// </summary>
    private static void SetupLog4Net()
    {
        // Configure log4net from file, if present.

        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory();
            var configPath = Path.Combine(baseDir, "log4net.config");
            if (File.Exists(configPath))
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo(configPath));
                Log.Info("log4net configured from file: " + configPath);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"log4net configuration file not found: {configPath}");
            }
        }
        catch (Exception ex)
        {
            // Do not block startup if logging configuration fails.

            System.Diagnostics.Debug.WriteLine($"Failed to configure log4net: {ex}");
        }
    }

    /// <summary>
    /// Set up the exception handlers for unhandled exceptions that may occur in the application.
    /// </summary>
    private void AddGlobalExceptionHandlers()
    {
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    protected async override void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }

    private void App_DispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            Log.Error("Unhandled UI thread exception", e.Exception);

            // Inform the user and allow them to choose whether to exit or continue.
            var result = MessageBox.Show(
                "An unexpected error occurred and the application may be unstable.\n\n" +
                "Details have been written to the log file.\n\n" +
                "Do you want to close the application?",
                "Unexpected Error",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                // Let the app shut down
                Current?.Shutdown();
            }
            else
            {
                // Attempt to continue
                e.Handled = true;
            }
        }
        catch (Exception ex)
        {
            // If even the handler fails, write to debug and attempt an orderly shutdown.
            System.Diagnostics.Debug.WriteLine($"Exception in DispatcherUnhandledException handler: {ex}");
            try { Current?.Shutdown(); } catch { }
        }
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            if (e.ExceptionObject is Exception ex)
            {
                Log.Fatal("Unhandled domain exception", ex);
            }
            else
            {
                Log.Fatal($"Unhandled domain exception (non-Exception object): {e.ExceptionObject}");
            }

            // If the runtime is terminating, try to show a simple message box (best effort).
            if (e.IsTerminating)
            {
                try
                {
                    MessageBox.Show("A fatal error occurred and the application must close. " +
                        "See the log file for details.", "Fatal Error", MessageBoxButton.OK, 
                        MessageBoxImage.Stop);
                }
                catch { /* swallow UI errors during termination */ }
            }
        }
        catch (Exception ex2)
        {
            System.Diagnostics.Debug.WriteLine($"Exception in UnhandledException handler: {ex2}");
        }
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        try
        {
            Log.Error("Unobserved task exception", e.Exception);
            // Mark as observed to prevent the process from being terminated.
            e.SetObserved();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception in UnobservedTaskException handler: {ex}");
        }
    }
}
