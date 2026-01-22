using Client;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using System.Net.Http;
using System.Windows;

namespace Client
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<IAuthService, Services.AuthService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7218/");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "WpfClient");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddTransient<LoginViewModel>();

            services.AddTransient<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}