using FlashDriveEncryptor.Services;
namespace FlashDriveEncryptor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            JsonConfigurationProvider jsonConfigurationProvider = new JsonConfigurationProvider(new ConsoleLogger());
        }
    }
}
