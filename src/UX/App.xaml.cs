using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Core.Services;
using Seemon.Vault.Helpers.Extensions;
using Seemon.Vault.Services;
using Seemon.Vault.ViewModels;
using Seemon.Vault.Views;
using Serilog;
using Serilog.Formatting.Compact;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Seemon.Vault
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public static T GetService<T>() where T : class => ((App)Current)._host.Services.GetService(typeof(T)) as T;

        public App()
        {
            var appInfo = new ApplicationInfoService();
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("Version", appInfo.GetVersion())
                .MinimumLevel.Information()
                .WriteTo.Async(a => a.File(
                    Path.Combine(appInfo.GetLogPath(), "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} ({Version}) [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"))
                .WriteTo.Async(a => a.File(
                    new CompactJsonFormatter(), Path.Combine(appInfo.GetLogPath(), "errors.json"),
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                    rollingInterval: RollingInterval.Month))
                .CreateLogger();
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var activationArgs = new Dictionary<string, string>
            {

            };
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration(c =>
                {
                    c.SetBasePath(appLocation);
                    c.AddInMemoryCollection(activationArgs);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddSerilog();
                })
                .ConfigureServices(ConfigureServices)
                .Build();

            await _host.StartAsync();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // Application Host
            services.AddHostedService<ApplicationHostService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
            services.AddSingleton<ICommandLineService, CommandLineService>();
            services.AddSingleton<IPasswordCacheService, PasswordCacheService>();
            services.AddSingleton<IEncryptionService, EncryptionService>();
            services.AddSingleton<IKeyStoreService, KeyStoreService>();
            services.AddSingleton<IPasswordService, PasswordService>();

            // Services
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IWindowManagerService, WindowManagerService>();
            services.AddSingleton<ITaskbarIconService, TaskbarIconService>();
            services.AddSingleton<IHttpService, HttpService>();
            services.AddSingleton<IUpdateService, UpdateService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IPGPService, PGPService>();

            // Views and ViewModels
            services.AddSingleton<IShellWindow, ShellWindow>();
            services.AddSingleton<ShellViewModel>();

            services.AddTransient<WelcomeViewModel>();
            services.AddTransient<WelcomePage>();

            services.AddTransient<AboutViewModel>();
            services.AddTransient<AboutPage>();

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();

            services.AddTransient<LicenseViewModel>();
            services.AddTransient<LicensePage>();

            services.AddTransient<ProfileViewModel>();
            services.AddTransient<ProfileWindow>();

            services.AddTransient<ReleaseNotesViewModel>();
            services.AddTransient<ReleaseNotesWindow>();

            services.AddTransient<PasswordViewModel>();
            services.AddTransient<PasswordWindow>();

            services.AddTransient<NewPasswordViewModel>();
            services.AddTransient<NewPasswordWindow>();

            services.AddTransient<KeyStoreViewModel>();
            services.AddTransient<KeyStorePage>();

            services.AddTransient<GenerateKeyPairViewModel>();
            services.AddTransient<GenerateKeyPairWindow>();

            services.AddTransient<ChangePasswordViewModel>();
            services.AddTransient<ChangePasswordWindow>();

            services.AddTransient<KeyPairViewModel>();
            services.AddTransient<KeyPairPage>();

            services.AddTransient<TaskbarIconViewModel>();

            // Configuration
            services.ConfigureDictionary<ApplicationUrls>(context.Configuration.GetSection("urls"));
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            Log.CloseAndFlush();
            _host.Dispose();
            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
        }
    }
}
