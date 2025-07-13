using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ACPF.DataAccess;
using ACPF.UI.ViewModels;

namespace ACPF.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configurar injeção de dependência
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Configurar Entity Framework
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=financas.db"));

                    // Registrar ViewModels
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<TransacaoViewModel>();
                    services.AddTransient<CategoriaViewModel>();
                    services.AddTransient<RelatorioViewModel>();

                    // Registrar Views
                    services.AddTransient<Views.MainWindow>();
                })
                .Build();

            // Inicializar banco de dados
            using (var scope = _host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DatabaseInitializer.InitializeAsync(context).Wait();
            }

            // Criar e mostrar a janela principal
            var mainWindow = _host.Services.GetRequiredService<Views.MainWindow>();
            mainWindow.DataContext = _host.Services.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }
    }
} 