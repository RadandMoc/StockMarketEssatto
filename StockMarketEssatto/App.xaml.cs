using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockMarketEssatto.Data;
using StockMarketEssatto.Services;
using StockMarketEssatto.ViewModels;
using System.IO;
using System.Windows;

namespace StockMarketEssatto
{
	public partial class App : Application
	{
		public static ServiceProvider ServiceProvider { get; private set; }
		public static IConfiguration Configuration { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			//#if DEBUG
			builder.AddUserSecrets<App>();
			//#endif
			Configuration = builder.Build();

			var serviceCollection = new ServiceCollection();

			ConfigureServices(serviceCollection);

			ServiceProvider = serviceCollection.BuildServiceProvider();

			var mainWindow = new MainWindow();

			mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();

			mainWindow.Show();
		}

		private void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(Configuration);
			services.AddDbContext<StockDbContext>();
			services.AddSingleton<AlphaVantageService>();
			services.AddTransient<MainViewModel>();
			services.AddTransient<AddEditQuoteViewModel>();
		}
	}
}