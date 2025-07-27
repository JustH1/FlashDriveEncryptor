using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashDriveEncryptor.Cryptographer
{
    internal class FileHeaderHandler : IFileHeaderHandler
    {
        private readonly string _fileSignature;
        private readonly byte[] _salt;
        private readonly int _saltLength;
        public FileHeaderHandler(string fileSignature, string salt, int saltLength)
        {
            _fileSignature = fileSignature;
            _salt = Encoding.UTF8.GetBytes(salt);
            _saltLength = saltLength;
        }
        public void WriteHeaderStream(Stream stream, byte[] iv, string originalExtension)
        {
            byte[] signatureBytes = Encoding.UTF8.GetBytes(_fileSignature);

            if (signatureBytes.Length > _fileSignature.Length)
            {
                throw new Exception("Invalid length of the encryption signature information.");
            }

            stream.Write(signatureBytes, 0, signatureBytes.Length);

            stream.Write(_salt, 0, _saltLength);

            stream.Write(iv, 0, iv.Length);

            byte[] extensionBytes = Encoding.UTF8.GetBytes(originalExtension);
            byte extensionLength = (byte)extensionBytes.Length;
            stream.WriteByte(extensionLength);

            if (extensionLength > 0)
            {
                stream.Write(extensionBytes, 0, extensionLength);
            }
        }
        public bool TryReadHeader(Stream stream, out byte[]? iv, out string? originalExtension)
        {
            iv = null;
            originalExtension = null;

            byte[] signatureBytes = new byte[_fileSignature.Length];

            if (stream.Read(signatureBytes, 0, _fileSignature.Length) != _fileSignature.Length)
            { return false; }

            if (Encoding.UTF8.GetString(signatureBytes) != _fileSignature)
            { return false; }

            byte[] saltBytes = new byte[_saltLength];

            if (stream.Read(saltBytes, 0, _saltLength) != saltBytes.Length)
            { return false; }

            if (!saltBytes.SequenceEqual(_salt))
            { return false; }

            iv = new byte[16];

            if (stream.Read(iv, 0, iv.Length) > iv.Length) { return false; }

            int extensionLength = stream.ReadByte();
            if (extensionLength == -1 || extensionLength == 0) { return false; }

            if (extensionLength > 0)
            {
                byte[] extensionBytes = new byte[extensionLength];
                if (stream.Read(extensionBytes, 0, extensionLength) != extensionLength)
                {
                    return false;
                }
                originalExtension = Encoding.UTF8.GetString(extensionBytes);
            }
            else
            {
                originalExtension = string.Empty;
            }

            return true;
        }
    }
}
