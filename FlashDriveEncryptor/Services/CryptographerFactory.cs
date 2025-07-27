using FlashDriveEncryptor.Cryptographer;

namespace FlashDriveEncryptor.Services
{
    internal class CryptographerFactory : ICryptographerFactory
    {
        private readonly IConfigurationProvider _config;
        private readonly ILogger _logger;
        public CryptographerFactory(IConfigurationProvider configurationProvider, ILogger logger)
        {
            _config = configurationProvider;
            _logger = logger;
        }
        //Add data validation from the settings and get rid of the magic strings
        public ICryptographer CreateCryptographer(string algorithm = "AES")
        {
            EncryptionSettingsBuilder builder = new EncryptionSettingsBuilder();

            builder.WithSaltLength(Convert.ToInt32(_config["saltLength"]))
                .WithIteration(Convert.ToInt32(_config["iteration"]))
                .WithKeySize(Convert.ToInt32(_config["keySize"]))
                .WithReadBufferSize(Convert.ToInt32(_config["readBufferSize"]))
                .WithFileSignature(Convert.ToString(_config["fileSignature"]))
                .WithSalt(Convert.ToString(_config["salt"]));

            return algorithm.ToUpper() switch
            {
                "AES" => new AesCryptographer(_logger, builder.Build()),
                _ => throw new NotSupportedException($"Algorithm {algorithm} is not supported")
            }; 
        }
    }
}
