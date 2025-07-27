using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashDriveEncryptor.Services
{
    interface ICryptographerFactory
    {
        public ICryptographer CreateCryptographer(string algorithm = "AES");
    }
}
