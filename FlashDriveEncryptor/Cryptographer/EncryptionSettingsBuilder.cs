using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashDriveEncryptor.Cryptographer
{
    internal class EncryptionSettingsBuilder
    {
        private EncryptionSettings _encryptionSettings;

        public EncryptionSettingsBuilder()
        {
            _encryptionSettings = new EncryptionSettings();
        }
        public EncryptionSettingsBuilder WithSaltLength(int saltLength)
        {
            _encryptionSettings.SaltLength = saltLength;
            return this;
        }
        public EncryptionSettingsBuilder WithIteration(int iteration)
        {
            _encryptionSettings.Iteration = iteration;
            return this;
        }
        public EncryptionSettingsBuilder WithKeySize(int keySize)
        {
            _encryptionSettings.KeySize = keySize;
            return this;
        }
        public EncryptionSettingsBuilder WithReadBufferSize(int readBufferSize)
        {
            _encryptionSettings.ReadBufferSize = readBufferSize;
            return this;
        }
        public EncryptionSettingsBuilder WithFileSignature(string fileSignature)
        {
            _encryptionSettings.FileSignature = fileSignature;
            return this;
        }
        public EncryptionSettingsBuilder WithSalt(string salt)
        {
            _encryptionSettings.Salt = salt;
            return this;
        }
        public EncryptionSettings Build()
        {
            return _encryptionSettings;
        }
    }
}
