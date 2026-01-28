using System.Net.Http;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;

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
            services.AddHttpClient<IAuthService, AuthService>(client => client.ConfigureDefault());
            services.AddHttpClient<IPatientService, PatientService>(client => client.ConfigureDefault());

            services.AddTransient<MainWindow>();
            services.AddTransient<PatientsPage>();
            services.AddSingleton<MainAppWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}