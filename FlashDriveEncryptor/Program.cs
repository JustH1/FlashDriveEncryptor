using FlashDriveEncryptor.Cryptographer;
using FlashDriveEncryptor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlashDriveEncryptor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddSingleton<IConfigurationProvider, JsonConfigurationProvider>();
            services.AddTransient<IFileProvider, FileProvider>();
            services.AddSingleton<ICryptographerFactory , CryptographerFactory>();
            services.AddSingleton<IView, ConsoleView>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            
            IView? view = serviceProvider.GetService<IView>();

            view?.Start();
        }
    }
}
