using System;
using System.Security.Cryptography;
using System.Text;

namespace FlashDriveEncryptor.Services
{
    internal class Cryptographer : ICryptographer, IDisposable
    {
        private const int SaltSize = 16; //Byte
        private const int Iteration = 5;
        private const int AesKeySize = 256; //Bit
        private const int BufferSize = 32;  //The size of the buffer for reading and writing during encryption.

        private const string FileSignature = "ENCv1";


        private readonly byte[] _key;
        private readonly byte[] _salt;

        private readonly ILogger _logger;

        public Cryptographer(ILogger logger, string password, string salt)
        {
            _logger = logger;

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(salt))
            {
                throw new ArgumentException("The encryption key or salt cannot be empty.");
            }

            _salt = Convert.FromBase64String(salt);

            using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, _salt, Iteration, HashAlgorithmName.SHA256))
            {
                _key = deriveBytes.GetBytes(AesKeySize / 8);
            }
        }

        public void EncryptFiles(IEnumerable<string> filePaths)
        {
            if (filePaths == null || filePaths.Count() == 0)
            {
                throw new ArgumentException("The array of file paths for encryption cannot be empty.");
            }

            foreach (string filePath in filePaths)
            {
                try
                {
                    EncryptSingleFile(filePath);
                    _logger.Info($"The file {filePath} successfully encrypted");
                }
                catch (Exception ex)
                {
                    _logger.Error($"File encryption error {filePath}.", ex);
                }
            }
        }

        private void EncryptSingleFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"The file {filePath} not found.");
            }

            string encryptedFilePath = filePath + ".enc";
            string originalExtension = Path.GetExtension(filePath);

            try
            {
                using (FileStream input = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (FileStream output = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write))
                using (Aes aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.GenerateIV();

                    WriteHeaderStream(output, aes.IV, originalExtension);

                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (CryptoStream cs = new CryptoStream(output, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] buffer = new byte[BufferSize];
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

        private void DecryptSingleFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"The file {filePath} not found.");
            }

            string tempFilePath = Path.GetTempFileName();

            try
            {

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void WriteHeaderStream(Stream stream, byte[] iv, string originalExtension)
        {
            byte[] signatureBytes = Encoding.UTF8.GetBytes(FileSignature);
            stream.Write(signatureBytes, 0, signatureBytes.Length);

            stream.Write(_salt, 0, _salt.Length);

            stream.Write(iv, 0, iv.Length);

            byte[] extensionBytes = Encoding.UTF8.GetBytes(originalExtension);
            byte extensionLength = (byte)extensionBytes.Length;
            stream.WriteByte(extensionLength);

            if (extensionLength > 0)
            {
                stream.Write(extensionBytes, 0, extensionLength);
            }
        }
    }
}
