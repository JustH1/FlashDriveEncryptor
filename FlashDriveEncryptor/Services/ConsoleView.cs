using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FlashDriveEncryptor.Services
{
    internal class ConsoleView : IView
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ICryptographer _cryptographer;
        private readonly IFileProvider _fileProvider;
        private readonly Dictionary<string, Action<string>> _commands;
        public ConsoleView(IConfigurationProvider jsonConfigurationProvider, ICryptographerFactory cryptographerFactory, IFileProvider fileProvider) 
        {
            _configurationProvider = jsonConfigurationProvider;
            _cryptographer = cryptographerFactory.CreateCryptographer();
            _fileProvider = fileProvider;

            _commands = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["help"] = (arg) => PrintHelpInfo(),
                ["exit"] = (arg) => Environment.Exit(0),
                ["enc"]  = (arg) => Encrypt(arg),
                ["dec"]  = (arg) => Decrypt(arg)                
            };
        }

        public void Start()
        {
            PrintLogo();

            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Current configuration:");
            Console.ResetColor();

            PrintConfiguration();
            Console.WriteLine();

            while (true) 
            {
                Console.Write("FDE > ");
                string? input = Console.ReadLine();

                string[] inputStrings = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                
                if (inputStrings != null && inputStrings.Length != 0)
                {
                    string command = inputStrings[0];
                    string? argument = inputStrings.Length > 1 ? inputStrings[1] : null;

                    if (_commands.TryGetValue(command, out Action<string> handler))
                    {
                        handler(argument);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid command. Enter \"help\" to get help.");
                }
                
            }
        }
        private void Encrypt(string argument)
        {
            IEnumerable<string> filePaths = _fileProvider.GetFileEncryptionPaths();
            _cryptographer.EncryptFiles(filePaths, argument);
        }
        private void Decrypt(string argument)
        {
            IEnumerable<string> filePaths = _fileProvider.GetFileEncryptionPaths();
            _cryptographer.DecryptFiles(filePaths, argument);
        }
        private void PrintHelpInfo()
        {
            Console.WriteLine("help              Displays background information.");
            Console.WriteLine("enc <password>    Encrypts the directory specified in the configuration file.");
            Console.WriteLine("dec <password>    Encrypts the directory specified in the configuration file.");
            Console.WriteLine("exit              Exit the program.");
        }
        private void PrintConfiguration()
        {
            Dictionary<string, object>? configuration = _configurationProvider.GetFullConfiguration();

            if (configuration != null)
            {
                foreach (string key in configuration.Keys)
                {
                    Console.WriteLine($"{key} : {configuration[key]}");
                }
            }
        }
        private void PrintLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            string[] logo = {
                "███████╗██████╗ ███████╗",
                "██╔════╝██╔══██╗██╔════╝",
                "█████╗  ██║  ██║█████╗  ",
                "██╔══╝  ██║  ██║██╔══╝  ",
                "██║     ██████╔╝███████╗",
                "╚═╝     ╚═════╝ ╚══════╝"
            };

            foreach (string line in logo)
            {
                Console.WriteLine(line);
            }

            Console.ResetColor();
        }
    }
}
