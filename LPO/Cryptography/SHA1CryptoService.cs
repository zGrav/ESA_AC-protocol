using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ESA_AC.Cryptography
{
    class SHA1CryptoService
    {
        private SHA1CryptoServiceProvider cryptoProvider;
        private Encoding encoder;

        public SHA1CryptoService()
        {
            this.cryptoProvider = new SHA1CryptoServiceProvider();
            this.encoder = Encoding.ASCII;
        }

        internal string EncodeHash(string data)
        {
            byte[] toreturn = encoder.GetBytes(data);
            byte[] hash = cryptoProvider.ComputeHash(toreturn);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }
}
