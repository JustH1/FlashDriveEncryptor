namespace FlashDriveEncryptor.Services
{
    internal class FileProvider
    {
        private readonly ILogger _logger;
        private readonly string _diskPath;

        public FileProvider(ILogger logger)
        {
            _logger = logger;
            _diskPath = GetDiskPath();
        }
        public string[] GetFilesEncryption()
        {
            return Directory.GetFiles(_diskPath, ".", SearchOption.AllDirectories);
        }
        private string GetDiskPath()
        {
            return Path.GetPathRoot(Directory.GetCurrentDirectory());
        }
    }
}
