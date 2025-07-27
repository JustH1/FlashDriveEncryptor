using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashDriveEncryptor.Cryptographer
{
    internal interface IFileHeaderHandler
    {
        public void WriteHeaderStream(Stream stream, byte[] iv, string originalExtension);
        public bool TryReadHeader(Stream stream, out byte[]? iv, out string? originalExtension);
    }
}
