using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FlashDriveEncryptor.Cryptographer
{
    internal class EncryptionKeyProvider : IDisposable
    {
        private byte[]? _key;

        public byte[] Key { get { return _key; } }

        public EncryptionKeyProvider(string password, string salt, int iteration, int aesKeySize)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(salt))
            {
                throw new ArgumentException("The encryption key or salt cannot be empty.");
            }

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(passwordBytes, saltBytes, iteration, HashAlgorithmName.SHA256))
            {
                _key = deriveBytes.GetBytes(aesKeySize / 8);
            }
        }
        public void Dispose()
        {
            CryptographicOperations.ZeroMemory(_key);
            _key = null;
        }
    }
}
