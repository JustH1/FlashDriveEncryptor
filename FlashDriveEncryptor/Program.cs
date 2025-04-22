using FlashDriveEncryptor.Services;
namespace FlashDriveEncryptor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileProvider fileProvider = new FileProvider(new ConsoleLogger());
            foreach (var item in fileProvider.GetFilesEncryption())
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
