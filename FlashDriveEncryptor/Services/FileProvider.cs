using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace FlashDriveEncryptor.Services
{
    internal class FileProvider : IFileProvider
    {
        private readonly ILogger _logger;
        private readonly string _path;

        public FileProvider(ILogger logger)
        {
            _logger = logger;
            _path = "D:\\Info";
        }
        public IEnumerable<string> GetFilesEncryption()
        {
            return GetAllAccessibleFiles(_path);
        }
        private IEnumerable<string> GetAllAccessibleFiles(string currentDirectory)
        {
            List<string> filePaths = new List<string>();

            try
            {
                //Getting the paths of the available files
                foreach (string file in Directory.GetFiles(currentDirectory))
                {
                    try
                    {
                        filePaths.Add(file);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        _logger.Error($"Access to the file {file} is prohibited.");
                    }
                }

                //Recursive processing of subdirectories
                foreach (string directory in Directory.GetDirectories(currentDirectory))
                {
                    try
                    {
                        filePaths.AddRange(GetAllAccessibleFiles(directory));
                    }
                    catch (UnauthorizedAccessException)
                    {
                        _logger.Error($"Access to the directory {directory} is prohibited.");
                    }
                    catch (DirectoryNotFoundException)
                    {
                        _logger.Error($"The directory {directory} was not found.");
                    }
                }

            }
            catch (UnauthorizedAccessException)
            {
                _logger.Error("Access to the main directory is prohibited.");
            }
            catch (DirectoryNotFoundException)
            {
                _logger.Error("The main directory was not found.");
            }
            catch (Exception ex)
            {
                _logger.Error("", ex);
            }

            return filePaths;

        }
    }
}

