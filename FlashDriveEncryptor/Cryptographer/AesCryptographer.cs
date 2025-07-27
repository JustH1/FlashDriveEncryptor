using FlashDriveEncryptor.Services;
using System;
using System.Security.Cryptography;
using System.Text;

namespace FlashDriveEncryptor.Cryptographer
{
    internal class AesCryptographer : ICryptographer
    {
        private readonly ILogger _logger;
        private readonly IFileHeaderHandler _fileHeaderHandler;
        private readonly EncryptionSettings _encryptionSettings;

        public AesCryptographer(ILogger logger, EncryptionSettings encryptionSettings)
        {
            _logger = logger;
            _fileHeaderHandler = new FileHeaderHandler(encryptionSettings.FileSignature, encryptionSettings.Salt, encryptionSettings.SaltLength);
            _encryptionSettings = encryptionSettings;
        }

        public void EncryptFiles(IEnumerable<string> filePaths, string password)
        {
            using (EncryptionKeyProvider encryptionKeyProvider = new EncryptionKeyProvider(password, _encryptionSettings.Salt
                ,_encryptionSettings.Iteration, _encryptionSettings.KeySize))
            {
                if (filePaths == null || filePaths.Count() == 0)
                {
                    throw new ArgumentException("The array of file paths for encryption cannot be empty.");
                }

                foreach (string filePath in filePaths)
                {
                    try
                    {
                        EncryptSingleFile(filePath, encryptionKeyProvider);
                        _logger.Info($"The file {filePath} successfully encrypted.");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"File encryption error {filePath}.", ex);
                    }
                }
            }
        }
        private void EncryptSingleFile(string filePath, EncryptionKeyProvider encryptionKeyProvider)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"The file {filePath} not found.");
            }

            if (filePath.EndsWith(".enc"))
            {
                throw new ArgumentException($"The file is already encrypted.");
            }

            string encryptedFilePath = filePath + ".enc";
            string originalExtension = Path.GetExtension(filePath);

            try
            {
                using (FileStream input = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (FileStream output = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write))
                using (Aes aes = Aes.Create())
                {
                    aes.Key = encryptionKeyProvider.Key;
                    aes.GenerateIV();

                    _fileHeaderHandler.WriteHeaderStream(output, aes.IV, originalExtension);

                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (CryptoStream cs = new CryptoStream(output, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] buffer = new byte[_encryptionSettings.ReadBufferSize];
                        int bytesRead;
                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cs.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                File.Delete(filePath);
            }
            catch
            {
                if (File.Exists(encryptedFilePath))
                {
                    File.Delete(encryptedFilePath);
                }
                throw;
            }
        }

        public void DecryptFiles(IEnumerable<string> filePaths, string password)
        {
            using (EncryptionKeyProvider encryptionKeyProvider = new EncryptionKeyProvider(password, _encryptionSettings.Salt,
                _encryptionSettings.Iteration, _encryptionSettings.KeySize))
            {
                if (filePaths == null || filePaths.Count() == 0)
                {
                    throw new ArgumentException("The array of file paths for decryption cannot be empty.");
                }

                foreach (var filePath in filePaths)
                {
                    try
                    {
                        DecryptSingleFile(filePath, encryptionKeyProvider);
                        _logger.Info($"The file {filePath} successfully decrypted.");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"File encryption error {filePath}.", ex);
                    }
                }
            }
        }
        private void DecryptSingleFile(string filePath, EncryptionKeyProvider encryptionKeyProvider)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"The file {filePath} not found.");
            }

            string decryptFilePath = String.Empty;

            if (filePath.EndsWith(".enc"))
            {
                decryptFilePath = filePath.Substring(0, filePath.Length - 4);
            }
            else
            {
                throw new Exception("The file does not have the .enc format.");
            }

            try
            {
                byte[]? iv = null;
                string? originalExtension = null;

                using (FileStream input = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (!_fileHeaderHandler.TryReadHeader(input, out iv, out originalExtension))
                    {
                        throw new InvalidDataException("The file is not a valid encrypted file.");
                    }

                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = encryptionKeyProvider.Key;
                        aes.IV = iv ?? throw new Exception("The initialize vector not found.");

                        using (ICryptoTransform decryptor = aes.CreateDecryptor())
                        using (CryptoStream cs = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
                        using (FileStream output = new FileStream(decryptFilePath, FileMode.Create, FileAccess.Write))
                        {
                            byte[] buffer = new byte[_encryptionSettings.ReadBufferSize];
                            int bytesRead;
                            while ((bytesRead = cs.Read(buffer, 0, _encryptionSettings.ReadBufferSize)) > 0)
                            {
                                output.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }

                File.Delete(filePath);
            }
            catch
            {
                if (File.Exists(decryptFilePath))
                {
                    File.Delete(decryptFilePath);
                }
                throw;
            }
        }
    }
}
