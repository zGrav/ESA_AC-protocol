using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace LPO.Cryptography
{
    class MD5CryptoService // Class responsible for hashing data to MD5 and returning the encoded result.
    {
        private MD5CryptoServiceProvider cryptoProvider;
        private Encoding encoder;

        public MD5CryptoService()
        {
            this.cryptoProvider = new MD5CryptoServiceProvider();
            this.encoder = Encoding.ASCII;
        }

        internal string EncodeHash(string input)
        {
            byte[] encodedInput = encoder.GetBytes(input);
            byte[] encodedHash = cryptoProvider.ComputeHash(encodedInput);

            return GetResult(encodedHash);
        }

        internal string EncodeHashFromFile(string filePath)
        {
            byte[] encodedHash;
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192))
            {
                encodedHash = cryptoProvider.ComputeHash(stream);
            }

            return GetResult(encodedHash);
        }

        private string GetResult(byte[] encodedHash)
        {
            return BitConverter.ToString(encodedHash).Replace("-", string.Empty).ToLower();
        }
    }
}
