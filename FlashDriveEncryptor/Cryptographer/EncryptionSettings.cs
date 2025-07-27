namespace FlashDriveEncryptor.Cryptographer
{
    internal class EncryptionSettings
    {
        public int SaltLength { get; set; } = 16; //Byte
        public int Iteration { get; set; } = 10000;
        public int KeySize { get; set; } = 256; //Bit
        public int ReadBufferSize { get; set; } = 4096; //The size of the buffer for reading and writing during encryption.
        public string FileSignature { get; set; } = "ENCv1";
        public string Salt { get; set; } = "etfnsrutlfceduy4";
    }
}
