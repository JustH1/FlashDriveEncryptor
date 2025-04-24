using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashDriveEncryptor.Services
{
    internal interface IConfigurationProvider
    {
        public object? this[string key] { get; }
    }
}
